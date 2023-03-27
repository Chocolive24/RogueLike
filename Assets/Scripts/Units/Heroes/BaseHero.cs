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
    [SerializeField] protected DeckController _movementDeck;
    [SerializeField] protected DeckController _mainDeck;
    protected DiscardDeckController _movDiscardDeck;
    protected DiscardDeckController _mainDiscardDeck;
    [SerializeField] protected HeroClass _heroClass;

    [SerializeField] protected int _maxMana;
    
    [SerializeField] protected float _exploreSpeed = 10f;
    [SerializeField] protected float _battleSpeed = 8f;
    
    protected int _currentMana;

    protected Vector3? _targetPos = null;
    
    private Dictionary<Vector3, int> _path;
    private int _currentTargetIndex;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int MaxMana { get => _maxMana; }

    public int CurrentMana
    {
        get => _currentMana;
        set => _currentMana = value;
    }

    public Dictionary<Vector3, int> Path
    {
        get => _path;
        set => _path = value;
    }

    public bool IsInBattle => GameManager.Instance.State == GameState.BATTLE;
    
    // -----------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnOnGameStateChange;
    }

    private void GameManagerOnOnGameStateChange(GameState obj)
    {
        _targetPos = null;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        InitializeDecks();

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
        if (_targetPos.HasValue)
        {
            _exploreSpeed = IsInBattle ? _battleSpeed : _exploreSpeed;

            // Move the player to the target position
            transform.position = Vector3.MoveTowards(transform.position, _targetPos.Value,
                _exploreSpeed * Time.deltaTime);

            // The distance between the player and the target point would never be exactly equal to 0.
            // So we check with an Epsilon value if the player as reached the target position.
            // Then we set his position to the target position in in order to be precise.
            if (Vector3.Distance(transform.position, _targetPos.Value) <= 0.01f)
            {
                transform.position = _targetPos.Value;
                _targetPos = null;

                if (IsInBattle)
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
            if (GameManager.Instance.State == GameState.EXPLORING)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                _targetPos = new Vector3(mousePos.x, mousePos.y, 0);
            }
        }
    }

    public void HandleBattleMove()
    {
        if (_currentTargetIndex < _path.Count) 
        {
            _targetPos = GridManager.Instance.WorldToCellCenter(_path.Keys.Last());
            _path.Remove(_path.Keys.Last());
        }
        else
        {
            _targetPos = null;
        }
    }
}
