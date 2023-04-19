using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Rarety
{
    Basic = 10,
    Rare = 7,
    Epic = 3,
    Legendary = 1
}

public enum CardType
{
    MoveCard,
    Attackcard,
    DefendCard
}

public abstract class BaseCard : MonoBehaviour
{
    public BaseCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, 
                    int aeraOfEffect)
    {
        _name = name;
        _manaCost = manaCost;
        _rarety = rarety;
        _cardType = cardType;
        _heroClass = heroClass;
        _aeraOfEffect = aeraOfEffect;
    }

    // Attributes ------------------------------------------------------------------------------------------------------

    #region Tweakable Values

    // Constructor Values
    [SerializeField] protected string _name;
    [SerializeField] protected int _manaCost;
    [SerializeField] protected Rarety _rarety;
    [SerializeField] protected CardType _cardType;
    [SerializeField] protected HeroClass _heroClass;
    [SerializeField] protected int _aeraOfEffect;

    // Other Values
    [SerializeField] private float _moveTime = 0.2f;
    
    #endregion

    #region TextMeshPro Attributes

    protected TextMeshPro _manaNbrTxt;
    protected TextMeshPro _cardEffectTxt;

    protected TextMeshPro[] _textes;

    #endregion

    #region Tile Attributes

    protected Tilemap _aoeTilemap;
    
    protected Dictionary<Vector3, int> _availableTiles;

    #endregion

    #region State Pattern Attributes

    protected StateMachine _stateMachine;

    protected CollectibleCardState _collectibleCardState;
    protected InDeckCardState _inDeckCardState;
    protected DrawnCardState _drawnCardState;
    protected PerfomCardState _perfomCardState;
    protected DiscardedCardState _discardedCardState;

    #endregion

    private bool _isCollected = false;
    
    protected int _handIndex;

    protected bool _canDrawTilemap = true;

    protected bool _hasBeenPlayed = false;

    protected BoxCollider2D _boxCollider2D;

    // References ------------------------------------------------------------------------------------------------------

    #region Managers

    protected CardPlayedManager _cardPlayedManager;
    protected GridManager _gridManager;
    protected TilemapsManager _tilemapsManager;
    protected UIBattleManager _uiBattleManager;
    protected UnitsManager _unitsManager;
    
    #endregion
    
    // Events ----------------------------------------------------------------------------------------------------------
    public event Action<BaseCard> OnDrawn;
    public event Action<BaseCard> OnPerformed; 
    public event Action<BaseCard> OnCollected;

    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public Rarety Rarety => _rarety;
    public int ManaCost
    {
        get => _manaCost;
        set => _manaCost = value;
    }
    public CardType CardType => _cardType;
    public HeroClass HeroClass => _heroClass;
    public int AeraOfEffect => _aeraOfEffect;
    public int HandIndex { get => _handIndex; set => _handIndex = value; }

    public Dictionary<Vector3, int> AvailableTiles => _availableTiles;

    public Tilemap AoeTilemap
    {
        get => _aoeTilemap;
        set => _aoeTilemap = value;
    }

    public bool IsCollected
    {
        get => _isCollected;
        set => _isCollected = value;
    }

    public bool HasBeenPlayed
    {
        get => _hasBeenPlayed;
        set => _hasBeenPlayed = value;
    }

    #endregion
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        _availableTiles = new Dictionary<Vector3, int>();
        
        _boxCollider2D = GetComponent<BoxCollider2D>();

        GetTextes();

        BaseHero.OnMovement += HandleTilemap;
    }
    
    private void GetTextes()
    {
        // Get all TextMeshPro Components in children from the highest in the hierarchy to the lowest.
        // So the element 0 would be the manaNbrTxt and the 1 would be the cardEffectTxt.
        _textes = GetComponentsInChildren<TextMeshPro>();

        _manaNbrTxt = _textes[0];
        _cardEffectTxt = _textes[1];

        _manaNbrTxt.GetComponent<MeshRenderer>().sortingLayerName = "Card";
        _manaNbrTxt.GetComponent<MeshRenderer>().sortingOrder = 2;
        _cardEffectTxt.GetComponent<MeshRenderer>().sortingLayerName = "Card";
        _cardEffectTxt.GetComponent<MeshRenderer>().sortingOrder = 2;

        _canDrawTilemap = true;
        
        _manaNbrTxt.text = _manaCost.ToString();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ReferenceManagers();
        
        CreateStatePattern();
    }

    private void ReferenceManagers()
    {
        _cardPlayedManager = CardPlayedManager.Instance;
        _gridManager = GridManager.Instance;
        _tilemapsManager = TilemapsManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
        _unitsManager = UnitsManager.Instance;
    }

    private void CreateStatePattern()
    {
        _collectibleCardState = new CollectibleCardState(this);
        _inDeckCardState = new InDeckCardState();
        _drawnCardState = new DrawnCardState(this);
        _perfomCardState = new PerfomCardState(this);
        _discardedCardState = new DiscardedCardState(this);

        _stateMachine = new StateMachine();

        _stateMachine.AddTransition(_collectibleCardState, _inDeckCardState, () => _isCollected);
        _stateMachine.AddTransition(_inDeckCardState, _drawnCardState, () => gameObject.activeSelf);
        _stateMachine.AddTransition(_drawnCardState, _perfomCardState, () => CheckIfIsPlayed());
        _stateMachine.AddTransition(_perfomCardState, _inDeckCardState, () => CheckIfHasPerformed());
        
        _stateMachine.SetState(_collectibleCardState);
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        _stateMachine.Tick();
    }   
    
    private void OnMouseDown()
    {
        if (_isCollected)
        {
            PlayCard();
        }
        else
        {
            _isCollected = true;
            OnCollected?.Invoke(this);
        }
        
    }

    public void PlayCard()
    {
        if (_cardPlayedManager.CurrentCard == this)
        {
            transform.position = _cardPlayedManager.CardSlots[_handIndex].position;
        }

        if (CheckIfCanBePlayed())
        {
            transform.position = _cardPlayedManager._cardLocation.transform.position;

            _cardPlayedManager.CurrentCard = this;
        }
    }

    public abstract void ActivateCardEffect(TileCell tile);

    protected virtual void OnMouseEnter()
    {
        if (!_aoeTilemap && _isCollected && _canDrawTilemap)
        {
            _aoeTilemap = _tilemapsManager.InstantiateTilemap(_name + " aoe");
            
            this.GetAvailableTiles();
            
            this.DrawTilemap(_availableTiles, _aoeTilemap, _tilemapsManager.GetRuleTile(this));
        }
    }

    protected virtual void OnMouseExit()
    {
        if (_cardPlayedManager.CurrentCard != this && _aoeTilemap)
        {
            Destroy(_aoeTilemap.gameObject);
        }
    }
    
    private bool CheckIfCanBePlayed()
    {
        if (_unitsManager.HeroPlayer.CurrentMana == 0)
        {
            StopCoroutine(_uiBattleManager.NotEnoughManaCo());
            StartCoroutine(_uiBattleManager.NotEnoughManaCo());
        }
        
        return !_cardPlayedManager.HasACardOnIt && 
               _unitsManager.HeroPlayer.CurrentMana > 0 && _unitsManager.HeroPlayer.CanPlay;
    }
    
    protected virtual bool CheckIfIsPlayed()
    {
        return transform.position == _cardPlayedManager._cardLocation.transform.position;
    }

    protected abstract bool CheckIfHasPerformed();
    
    
    public virtual void GetAvailableTiles()
    {
        _availableTiles = _tilemapsManager.GetAvailableTilesInRange(
            _gridManager.WorldToCellCenter(GetStartingTile().transform.position),
            _aeraOfEffect, false, true);
    }

    public virtual void DrawTilemap(Dictionary<Vector3, int> availableNeighbours, 
                                    Tilemap tilemap, RuleTile ruleTile)
    {
        _tilemapsManager.DrawTilemap(availableNeighbours, tilemap, ruleTile);
    }

    // TODO Would not work with cards that not start from the player !!!!
    public virtual TileCell GetStartingTile()
    {
        foreach (var item in _gridManager.Tiles)
        {
            if (item.Value.OccupiedUnit)
            {
                if (item.Value.OccupiedUnit.Faction == Faction.Hero)
                {
                    return item.Value;
                }
            }
        }

        return null;
    }
    
    private void HandleTilemap(BaseHero obj)
    {
        _canDrawTilemap = !_canDrawTilemap;
    }
    
    private IEnumerator InterpolateMoveCo(Vector3 startPos, Vector3 endPos) 
    {
        float countTime = 0;
        
        while( countTime <= _moveTime ) 
        { 
            float percentTime = countTime / _moveTime;
            transform.position = Vector3.Lerp(startPos, endPos, percentTime);
            
            yield return null; // wait for next frame
            countTime += Time.deltaTime;
        }
        
        // because of the frame rate and the way we are running Lerp(),
        // the last timePercent in the loop may not = 1
        // therefore, this line ensures we end exactly where desired.
        transform.position = endPos;
    }

    public void EnterDrawn()
    {
        OnDrawn?.Invoke(this);
        gameObject.SetActive(true);
        _hasBeenPlayed = false;
    }

    public void ExitPerform()
    {
        OnPerformed?.Invoke(this);
        _cardPlayedManager.HandlePlayedCard();
        gameObject.SetActive(false);
    }

    public void EnterDiscarded()
    {
        gameObject.SetActive(false);
    }

    public abstract void ResetProperties();
}