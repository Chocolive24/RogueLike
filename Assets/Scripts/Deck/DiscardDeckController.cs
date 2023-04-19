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
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
