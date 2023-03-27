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
    Basic,
    Common,
    Rare,
    Epic,
    Legendary
}

public enum CardType
{
    MoveCard,
    Attackcard,
    DefendCard
}

public abstract class BaseCard : MonoBehaviour
{
    public BaseCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, int aeraOfEffect)
    {
        _name = name;
        _manaCost = manaCost;
        _rarety = rarety;
        _cardType = cardType;
        _heroClass = heroClass;
        _aeraOfEffect = aeraOfEffect;
    }

    [SerializeField] protected string _name;
    [SerializeField] protected int _manaCost;
    [SerializeField] protected Rarety _rarety;
    [SerializeField] protected CardType _cardType;
    [SerializeField] protected HeroClass _heroClass;
    [SerializeField] protected int _aeraOfEffect;

    protected Tilemap _aoeTilemap;
    
    protected int _handIndex;

    protected BoxCollider2D _boxCollider2D;

    [SerializeField] private float _moveTime = 0.2f;
    
    protected TextMeshPro _manaNbrTxt;
    protected TextMeshPro _cardEffectTxt;

    protected TextMeshPro[] _textes;

    // Lists for the aoe Tilemap ------------------------------------------------------------------
    protected Dictionary<Vector3, int> _availableTiles;

    // Getters and Setters ------------------------------------------------------------------------
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

    // --------------------------------------------------------------------------------------------

    private void Awake()
    {
        _availableTiles = new Dictionary<Vector3, int>();
        
        _boxCollider2D = GetComponent<BoxCollider2D>();

        // Get all TextMeshPro Components in children from the highest in the hierarchy to the lowest.
        // So the element 0 would be the manaNbrTxt and the 1 would be the cardEffectTxt.
        _textes = GetComponentsInChildren<TextMeshPro>();

        _manaNbrTxt = _textes[0];
        _cardEffectTxt = _textes[1];
        
        _manaNbrTxt.GetComponent<MeshRenderer>().sortingLayerName = "Card";
        _manaNbrTxt.GetComponent<MeshRenderer>().sortingOrder = 2;
        _cardEffectTxt.GetComponent<MeshRenderer>().sortingLayerName = "Card";
        _cardEffectTxt.GetComponent<MeshRenderer>().sortingOrder = 2;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _manaNbrTxt.text = _manaCost.ToString();
    }   
    
    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
        transform.rotation = Quaternion.identity;
    }

    private void OnMouseUp()
    {
        if (CheckIfCanBePlayed())
        {
            StartCoroutine(InterpolateMoveCo(transform.position, 
                CardPlayedManager.Instance._cardLocation.transform.position));
            
            CardPlayedManager.Instance.CurrentCard = this;
        }
        else 
        {
            StartCoroutine(InterpolateMoveCo(transform.position, 
                CardPlayedManager.Instance.CardSlots[_handIndex].position));
        }
    }

    protected virtual void OnMouseEnter()
    {
        if (!_aoeTilemap)
        {
            _aoeTilemap = TilemapsManager.Instance.InstantiateTilemap(_name + " aoe");
            
            this.GetAvailableTiles();
            
            this.DrawTilemap(_availableTiles, _aoeTilemap, TilemapsManager.Instance.GetRuleTile(this));
        }
    }

    protected virtual void OnMouseExit()
    {
        if (CardPlayedManager.Instance.CurrentCard != this && _aoeTilemap)
        {
            Destroy(_aoeTilemap.gameObject);
        }
    }
    
    private bool CheckIfCanBePlayed()
    {
        bool isOutsideCardLimit = transform.position.y - (_boxCollider2D.bounds.size.y / 2) >
                                CardPlayedManager.Instance._cardLimit.transform.position.y;

        if (UnitsManager.Instance.SelectedHero.CurrentMana == 0)
        {
            StopCoroutine(UIBattleManager.Instance.NotEnoughManaCo());
            StartCoroutine(UIBattleManager.Instance.NotEnoughManaCo());
        }
        
        return isOutsideCardLimit && !CardPlayedManager.Instance.HasACardOnIt && 
               UnitsManager.Instance.SelectedHero.CurrentMana > 0;
    }
    
    public virtual void GetAvailableTiles()
    {
        _availableTiles = TilemapsManager.Instance.GetAvailableTiles(
            GridManager.Instance.WorldToCellCenter(GetStartingTile().transform.position),
            _aeraOfEffect, this.gameObject);
    }

    public virtual void DrawTilemap(Dictionary<Vector3, int> availableNeighbours, Tilemap tilemap, RuleTile ruleTile)
    {
        TilemapsManager.Instance.DrawTilemap(availableNeighbours, tilemap, ruleTile);
    }

    // TODO Would not work with cards that not start from the player !!!!
    public virtual TileCell GetStartingTile()
    {
        foreach (var item in GridManager.Instance.Tiles)
        {
            //
            // TODO CHANGE TO THE HERO CLASS OR SOMETHING ELSE THAN FACTION
            //
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
}