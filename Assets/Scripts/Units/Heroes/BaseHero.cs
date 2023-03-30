using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] protected int _maxMana;
    [SerializeField] protected float _exploreSpeed = 10f;
    [SerializeField] protected float _battleSpeed = 8f;

    #endregion

    #region Decks

    protected DeckController _movementDeck;
    protected DeckController _mainDeck;
    protected DiscardDeckController _movDiscardDeck;
    protected DiscardDeckController _mainDiscardDeck;

    #endregion

    #region Pathfinding Attributes

    protected Vector3? _targetPos1 = null;
    protected Dictionary<Vector3, int> _path1;
    protected List<Vector3> _pathPositions1;
    protected int _currentTargetIndex1 = 0;

    #endregion
    
    protected int _currentMana;
    
    // References ------------------------------------------------------------------------------------------------------
    // private GameManager _gameManager;
    // private GridManager _gridManager;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public int MaxMana { get => _maxMana; }

    public int CurrentMana
    {
        get => _currentMana;
        set => _currentMana = value;
    }

    public Dictionary<Vector3, int> Path
    {
        get => _path1;
        set => _path1 = value;
    }
    
    #endregion
    
    // -----------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnOnGameStateChange;

        _pathPositions1 = new List<Vector3>();
    }

    private void GameManagerOnOnGameStateChange()
    {
        _targetPos1 = null;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        InitializeDecks();
        
        // _gameManager1 = GameManager.Instance;
        // _gridManager1 = GridManager.Instance;

        _currentMana = _maxMana;
    }

    // Update is called once per frame
    protected void Update()
    {
        HandleMove();
        
        HandleShuffleCardsBackToDeck();
    }
    
    private void HandleMove()
    {
        if (_targetPos1.HasValue)
        {
            float speed = _gameManager.IsInBattleState ? _battleSpeed : _exploreSpeed;

            // Move the player to the target position
            transform.position = Vector3.MoveTowards(transform.position, _targetPos1.Value,
                speed * Time.deltaTime);

            // The distance between the player and the target point would never be exactly equal to 0.
            // So we check with an Epsilon value if the player as reached the target position.
            // Then we set his position to the target position in order to be precise.
            if (Vector3.Distance(transform.position, _targetPos1.Value) <= 0.01f)
            {
                transform.position = _targetPos1.Value;
                _targetPos1 = null;

                if (_gameManager.IsInBattleState)
                {
                    HandleBattleMove();
                }
            }
        }
    }

    private void HandleShuffleCardsBackToDeck()
    {
        if (_movDiscardDeck.DiscardDeck.Count >= _movementDeck.Size)
        {
            _movDiscardDeck.ShuffleCardsBackToDeck(_movementDeck.Deck);
        }
        else if (_mainDiscardDeck.DiscardDeck.Count >= _mainDeck.Size)
        {
            _mainDiscardDeck.ShuffleCardsBackToDeck(_mainDeck.Deck);
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
            if (!_gameManager.IsInBattleState)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                _targetPos1 = new Vector3(mousePos.x, mousePos.y, 0);
            }
        }
    }

    public void HandleBattleMove()
    {
        if (_pathPositions1.Count == 0)
        {
            foreach (var item in _path1)
            {
                _pathPositions1.Add(item.Key);
            }
            
            _pathPositions1.Reverse();
        }
        
        if (_currentTargetIndex1 < _pathPositions1.Count) 
        {
            _targetPos1 = _gridManager.WorldToCellCenter(_pathPositions1[_currentTargetIndex1]);
            _currentTargetIndex1++;
        }
        else
        {
            _targetPos1 = null;
            _currentTargetIndex1 = 0;
            _pathPositions1.Clear();
        }
    }
}
