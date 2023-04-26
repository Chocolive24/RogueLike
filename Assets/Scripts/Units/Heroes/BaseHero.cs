using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

public enum HeroClass
{
    PALADIN,
    NOT_THE_HERO,
}

public class BaseHero : BaseUnit
{
    // Attributes ------------------------------------------------------------------------------------------------------

    #region Tweakable Values

    [SerializeField] protected HeroClass _heroClass;
    [SerializeField] protected IntReference _maxMana;
    [SerializeField] protected IntReference _exploreSpeed;
    [SerializeField] protected IntReference _battleSpeed;

    #endregion

    #region Decks

    protected DeckController _movementDeck;
    protected DeckController _mainDeck;
    protected DiscardDeckController _movDiscardDeck;
    protected DiscardDeckController _mainDiscardDeck;

    #endregion

    // #region Pathfinding Attributes
    //
    // protected Vector3? _targetPos1 = null;
    // protected Dictionary<Vector3, int> _path1;
    // protected List<Vector3> _pathPositions1;
    // protected int _currentTargetIndex1 = 0;
    //
    // #endregion

    protected List<BaseCard> _cardHand = new List<BaseCard>();

    protected int _currentMana;

    protected int _golds;

    private bool _canPlay;

    // References ------------------------------------------------------------------------------------------------------
    private HeroData _heroData;
    
    // private GameManager _gameManager;
    // private GridManager _gridManager;
    private CardPlayedManager _cardPlayedManager;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<BaseHero> OnMovement; 

    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public IntReference MaxMana { get => _maxMana; }

    public int CurrentMana
    {
        get => _currentMana;
        set => _currentMana = value;
    }

    public bool CanPlay => _canPlay;

    // public Dictionary<Vector3, int> Path
    // {
    //     get => _path1;
    //     set => _path1 = value;
    // }
    
    #endregion
    
    // -----------------------------------------------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnGameStateChange += GameManagerOnOnGameStateChange;
        BattleManager.OnPlayerTurnStart += StartTurn;
        BattleManager.OnPlayerTurnEnd += EndTurn;
        BattleManager.OnBattleEnd += PutAllCardsInDecks;
        TileCell.OnTileSelected += FindExploringPath;

        _cardHand = new List<BaseCard>();
    }

    protected override void SetData()
    {
        _heroData = (HeroData)_unitData;
        
        base.SetData();
        
        _heroClass = _heroData.HeroClass;
        _maxMana = _heroData.MaxMana;
        _exploreSpeed = _heroData.ExploreSpeed;
        _battleSpeed = _heroData.BattleSpeed;
    }

    private void FindExploringPath(TileCell tile)
    {
        if (!_gameManager.IsInBattleState)
        {
            Vector3 tileTargetPos = tile.transform.position;

            _exploringPath = FindPath(_gridManager.WorldToCellCenter(transform.position), tileTargetPos, 
                false, false, false);
        
            if (_exploringPath.Count > 0)
            {
                _targetPos = _exploringPath.First();
            }
            else
            {
                _targetPos = null;
            }
        }
    }

    private void GameManagerOnOnGameStateChange()
    {
        _targetPos = null;
    }
    
    private void StartTurn(BattleManager obj)
    { 
        //_unitsManager.SetSelectedHero(this);
        _currentMana = _maxMana.Value;
        _canPlay = true;
        
        _mainDeck.SetButtonInteractavity(true);
        _movementDeck.SetButtonInteractavity(true);
    }

    private void EndTurn(BattleManager obj)
    {
        _canPlay = false;
        _mainDeck.SetButtonInteractavity(false);
        _movementDeck.SetButtonInteractavity(false);
        //_unitsManager.SetSelectedHero(null);
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _cardPlayedManager = CardPlayedManager.Instance;
        
        InitializeDecks();
        
        _currentMana = _maxMana.Value;
    }

    // Update is called once per frame
    protected override void Update()
    {
        _speed = _gameManager.IsInBattleState ? _battleSpeed : _exploreSpeed;
        
        base.Update();
        
        MoveOnGrid();
        
        ShuffleCardsBackToDeck();
        
        Debug.Log(_cardHand.Count);
    }

    public override void FindAvailablePathToTarget(Vector3 targetPos, int minimumPathCunt, 
        bool countHeroes, bool countEnemies, bool countWalls)
    {
        _availableTiles = _cardPlayedManager.CurrentCard.AvailableTiles;
        base.FindAvailablePathToTarget(targetPos, 0, countHeroes, countEnemies, countWalls);

        if (_path.Count> 0)
        {
            OnMovement?.Invoke(this);
        }
    }


    protected override void StopThePath()
    {
        base.StopThePath();
        OnMovement?.Invoke(this);
    }

    private void ShuffleCardsBackToDeck()
    {
        if (_movDiscardDeck.DiscardDeck.Count >= _movementDeck.Size)
        {
            _movDiscardDeck.ShuffleCardsBackToDeck(_movementDeck);
        }
        else if (_mainDiscardDeck.DiscardDeck.Count >= _mainDeck.Size)
        {
            _mainDiscardDeck.ShuffleCardsBackToDeck(_mainDeck);
        }
    }
    
    protected void InitializeDecks()
    {
        var foundDecks = FindObjectsOfType<DeckController>();
        var foundDiscardDecks = FindObjectsOfType<DiscardDeckController>();

        foreach (var deck in foundDecks)
        {
            if (deck.HeroClass == _heroClass)
            {
                if (deck.TryGetComponent(out MovementDeckController moveDeck))
                {
                    _movementDeck = moveDeck;
                }
                else if (deck.TryGetComponent(out MainDeckContoller mainDeck))
                {
                    _mainDeck = mainDeck;
                }
            }
        }
        
        foreach (var discardDeck in foundDiscardDecks)
        {
            if (discardDeck.HeroClass == HeroClass.PALADIN)
            {
                if (discardDeck.DiscardCardType == DiscardCardType.Movement)
                {
                    _movDiscardDeck = discardDeck;
                }
                else if(discardDeck.DiscardCardType == DiscardCardType.Main)
                {
                    _mainDiscardDeck = discardDeck;
                }
            }
        }
    }

    public void HandleExploreMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // if (!_gameManager.IsInBattleState)
            // {
            //     Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //
            //     _targetPos = new Vector3(mousePos.x, mousePos.y, 0);
            // }
        }
    }
    
    private void PutAllCardsInDecks(BattleManager obj)
    {
        foreach (var card in _cardHand)
        {
            if (card.CardType == CardType.Attackcard)
            {
                _mainDeck.AddCard(card);
            }
            else if (card.CardType == CardType.MoveCard)
            {
                _movementDeck.AddCard(card);
            }

            _cardPlayedManager.ResetSlots();
        }

        _cardHand.Clear();
        
        _movDiscardDeck.ShuffleCardsBackToDeck(_movementDeck);
        _mainDiscardDeck.ShuffleCardsBackToDeck(_mainDeck);
    }

    public void AddCardToHand(BaseCard card)
    {
        _cardHand.Add(card);
    }

    public void RemoveCardFromHand(BaseCard card)
    {
        _cardHand.Remove(card);
    }
}
