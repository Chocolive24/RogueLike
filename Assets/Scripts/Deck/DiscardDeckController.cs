using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DiscardCardType
{
    Movement,
    Main
}

public class DiscardDeckController : MonoBehaviour
{
    [SerializeField] protected List<BaseCard> _discardDeck;

    [SerializeField] protected HeroClass _heroClass;
    
    [SerializeField] private TextMeshProUGUI _carNbrTxt;

    [SerializeField] private DiscardCardType _discardCardType;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<DiscardDeckController> OnDiscarFull; 

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public List<BaseCard> DiscardDeck
    {
        get => _discardDeck;
        set => _discardDeck = value; 
    }
   
    public HeroClass HeroClass => _heroClass;
    public DiscardCardType DiscardCardType => _discardCardType;

    // -----------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        UnitsManager.OnHeroSpawn += SetDiscardDeck;
    }

    private void SetDiscardDeck(UnitsManager arg1, BaseHero hero)
    {
        _carNbrTxt.text = _discardDeck.Count.ToString();
        
        if (!hero.MainDeck)
        {
            if (_discardCardType == DiscardCardType.Main)
            {
                hero.MainDiscardDeck = this;
            }
        }
        if (!hero.MovDiscardDeck)
        {
            if (_discardCardType == DiscardCardType.Movement)
            {
                hero.MovDiscardDeck = this;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddACard(BaseCard card, DeckController deck)
    {
        _discardDeck.Add(card);
        
        if (_discardDeck.Count == deck.Size)
        {
            OnDiscarFull?.Invoke(this);
        }
        
        _carNbrTxt.text = _discardDeck.Count.ToString();
    }
    
    public void ShuffleCardsBackToDeck(DeckController deckController)
    {
        foreach (var card in _discardDeck)
        {
            deckController.Deck.Add(card);
        }
        
        deckController.UpdateCardTxtNbr();
        _discardDeck.Clear();
    }
}
