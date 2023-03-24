using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] protected float _speed = 1f;
    
    protected int _currentMana;

    protected Vector3 _targetPos;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int MaxMana { get => _maxMana; }

    public int CurrentMana
    {
        get => _currentMana;
        set => _currentMana = value;
    }

    // -----------------------------------------------------------------------------------------------------------------
    
    // Start is called before the first frame update
    protected void Start()
    {
        InitializeDecks();

        _currentMana = _maxMana;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (_movDiscardDeck.DiscardDeck.Count >= _movementDeck.Size)
        {
            _movDiscardDeck.ShuffleCardsBackToDeck(_movementDeck.Deck);
        }
        else if (_mainDiscardDeck.DiscardDeck.Count >= _mainDeck.Size)
        {
            _mainDiscardDeck.ShuffleCardsBackToDeck(_mainDeck.Deck);
        }

        if (GameManager.Instance.State == GameState.EXPLORING)
        {
            // Move the player to the Mouse position
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, 
                _speed * Time.deltaTime);
        
            // Stop the player and set his position to the Mouse position if the Distance between
            // the 2 points is less or equal to 0.01f.
            if (Vector3.Distance(transform.position, _targetPos) <= 0.01f)
            {
                transform.position = _targetPos;
            }
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
                    Debug.Log(_movDiscardDeck);
                }
                else if(discardDeck.DiscardCardType == DiscardCardType.Main)
                {
                    _mainDiscardDeck = discardDeck;
                    Debug.Log(_mainDiscardDeck);
                }
            }
        }
    }

    public void HandleMove(InputAction.CallbackContext ctx)
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
}
