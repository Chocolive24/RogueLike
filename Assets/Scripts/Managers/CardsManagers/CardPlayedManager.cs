using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CardPlayedManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    private static CardPlayedManager _instance;
    public static CardPlayedManager Instance { get { return _instance; } }

    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private Texture2D _baseMouseCursor;

    private BaseCard _currentCard;
    
    #region Card Emplacements

    [Header("Card Emplacements")]
    [SerializeField] public GameObject _cardLocation;
    [SerializeField] public GameObject _cardLimit;
    [SerializeField] private Transform[] _cardSlots;
    [SerializeField] private bool[] _availableCardSlots;

    #endregion

    #region Decks

    [Header("Paladin's Discard Decks")]
    [SerializeField] private DiscardDeckController _paladinMovDiscDeckContr;
    [SerializeField] private DiscardDeckController _paladinMainDiscDeckContr;


    #endregion

    #region Has a Card on it Booleans

    private bool _hasACardOnIt;
    private bool _hasAMoveCardOnIt = false;
    private bool _hasAnAttackCardOnIt = false;
    private bool _hasADefendCardOnIt = false;

    #endregion
    
    // References ------------------------------------------------------------------------------------------------------
    private TilemapsManager _tilemapsManager;
    private UnitsManager _unitsManager;
    private GridManager _gridManager;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public BaseCard CurrentCard
    {
        get => _currentCard;
        set => _currentCard = value;
    }
    public bool HasACardOnIt => _hasAMoveCardOnIt || _hasAnAttackCardOnIt || _hasADefendCardOnIt;
    public bool HasAMoveCardOnIt => _hasAMoveCardOnIt;
    public bool HasAnAttackCardOnIt => _hasAnAttackCardOnIt;
    public bool HasADefendCardOnIt => _hasADefendCardOnIt;

    public Transform[] CardSlots => _cardSlots;
    public bool[] AvailableCardSlots
    {
        get => _availableCardSlots;
        set => _availableCardSlots = value;
    }

    #endregion
    
    // -----------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        _availableCardSlots = new bool[_cardSlots.Length];

        for (int i = 0; i < _availableCardSlots.Length; i++)
        {
            _availableCardSlots[i] = true;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _tilemapsManager = TilemapsManager.Instance;
        _unitsManager = UnitsManager.Instance;
        _gridManager = GridManager.Instance;
        
        Cursor.SetCursor(_baseMouseCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    // TODO MAKE A CLICK SYSTEM LIKE INSCRYPTION !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    private void OnTriggerEnter2D(Collider2D col)
    {
        // TODO Change Cursor to down arrow
        
        if (col.CompareTag("Card") && !HasACardOnIt)
        {
            _currentCard = col.GetComponent<BaseCard>();
            
            if (col.GetComponent<BaseMoveCard>())
            {
                _hasAMoveCardOnIt = true;
            }
            else if (col.GetComponent<BaseAttackCard>())
            {
                _hasAnAttackCardOnIt = true;
            }

            DrawCurrentCardTilemap(col.GetComponent<BaseCard>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // TODO Change the mouse cursor
        
        ClearCurrentCardTilemap();
        
        ClearCardLocation();
    }

    private void DrawCurrentCardTilemap(BaseCard card)
    {
        if (!card.AoeTilemap)
        {
            card.AoeTilemap = _tilemapsManager.InstantiateTilemap(card.name + " aoe");
            
            card.GetAvailableTiles(); 
        
            card.DrawTilemap(card.AvailableTiles, card.AoeTilemap, _tilemapsManager.GetRuleTile(card));
        }
    }

    /// <summary>
    /// Manage the actions to do when a card has been played.
    /// </summary>
    public void HandlePlayedCard()
    {
        _unitsManager.SelectedHero.CurrentMana -= _currentCard.ManaCost;
        
        // Free the current card slot.
        _availableCardSlots[_currentCard.HandIndex] = true;
        
        ClearCurrentCardTilemap();

        MoveCardToHisDiscardPile();

        ClearCardLocation();
        
        // TODO Change the mouse cursor
    }
    
    private void ClearCurrentCardTilemap()
    {
        Destroy(_currentCard.AoeTilemap.gameObject);

        if (_currentCard.CardType == CardType.MoveCard)
        {
            BaseMoveCard card = _currentCard.GetComponent<BaseMoveCard>();
            
            foreach (var item in card.Path)
            {
                var tile = _gridManager.GetTileAtPosition(item.Key);
                tile.Arrow.SetActive(false);
            }
        }
    }
    
    private void MoveCardToHisDiscardPile()
    {
        if (_currentCard.CardType == CardType.MoveCard)
        {
            if (_currentCard.HeroClass == HeroClass.PALADIN)
            {
                _paladinMovDiscDeckContr.DiscardDeck.Add(_currentCard);
            }
        }
        else if (_currentCard.CardType == CardType.Attackcard)
        {
            if (_currentCard.HeroClass == HeroClass.PALADIN)
            {
                _paladinMainDiscDeckContr.DiscardDeck.Add(_currentCard);
            }
        }
        
        _currentCard.gameObject.SetActive(false);
    }
    
    private void ClearCardLocation()
    {
        _currentCard = null;
        
        _hasAMoveCardOnIt = false;
        _hasAnAttackCardOnIt = false;
        _hasADefendCardOnIt = false;
    }
}