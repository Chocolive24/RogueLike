using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class DeckController : MonoBehaviour
{
    [SerializeField] protected List<BaseCard> _deck;

    [SerializeField] protected HeroClass _heroClass;
    
    [SerializeField] private TextMeshProUGUI _carNbrTxt;

    [SerializeField] protected int _size;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public List<BaseCard> Deck => _deck;
    public HeroClass HeroClass => _heroClass;

    public int Size
    {
        get => _size;
        set => _size = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _carNbrTxt.text = _deck.Count.ToString();
    }

    protected void InstantiateBasicCard(List<ScriptableCard> scriptableCards, int cardNbr)
    {
        for (int i = 0; i < cardNbr; i++)
        {
            var card = CardsManager.Instance.InstantiateCard(scriptableCards, Rarety.Basic);
            card.transform.parent = gameObject.transform;
            
            card.gameObject.SetActive(false);

            _deck.Add(card);
        }

        _size = _deck.Count;
    }

    public void DrawACard()
    {
        if (_deck.Count >= 1)
        {
            BaseCard rndCard = _deck[Random.Range(0, _deck.Count)];

            for (int i = 0; i < CardPlayedManager.Instance.AvailableCardSlots.Length; i++)
            {
                if (CardPlayedManager.Instance.AvailableCardSlots[i] == true)
                {
                    rndCard.gameObject.SetActive(true);
                    rndCard.HandIndex = i;
                    rndCard.transform.position = CardPlayedManager.Instance.CardSlots[i].position;
                    CardPlayedManager.Instance.AvailableCardSlots[i] = false;
                    _deck.Remove(rndCard);
                    return;
                }
            }
        }
    }
    
    // TODO AddCard() -> add a new card in the deck and update the size (_size = _deck.Count)
    
    // private void OnMouseDown()
    // {
    //     DrawACard();
    // }
}
