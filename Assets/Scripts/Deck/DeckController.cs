using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class DeckController : MonoBehaviour
{
    [SerializeField] protected List<BaseCard> _deck;

    [SerializeField] protected HeroClass _heroClass;
    
    [SerializeField] protected TextMeshProUGUI _carNbrTxt;

    [SerializeField] protected int _size;

    [SerializeField] protected Button _button;
    
    // References ------------------------------------------------------------------------------------------------------
    protected UnitsManager _unitsManager;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public List<BaseCard> Deck => _deck;
    public HeroClass HeroClass => _heroClass;

    public int Size
    {
        get => _size;
        set => _size = value;
    }

    protected virtual void Awake()
    {
        UnitsManager.OnHeroSpawn += SetDeck;
    }

    protected virtual void SetDeck(UnitsManager obj)
    {
        UpdateCardTxtNbr();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _unitsManager = UnitsManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public virtual void SetButtonInteractavity(bool interactable)
    {
        _button.interactable = interactable;
    }

    public void InstantiateBasicCard(List<ScriptableCard> scriptableCards, int cardNbr)
    {
        for (int i = 0; i < cardNbr; i++)
        {
            var card = CardsManager.Instance.InstantiateCard(scriptableCards, Rarety.Basic);
            
            AddCard(card);
        }
    }
    
    public void AddCard(BaseCard card)
    {
        card.transform.parent = gameObject.transform;

        card.gameObject.SetActive(false);

        card.IsCollected = true;

        _deck.Add(card);

        _size = _deck.Count;
        
        UpdateCardTxtNbr();

        card.OnDrawn += UnitsManager.Instance.HeroPlayer.AddCardToHand;
        card.OnPerformed += UnitsManager.Instance.HeroPlayer.RemoveCardFromHand;
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
                    
                    UpdateCardTxtNbr();
                    return;
                }
            }
        }
    }

    public void UpdateCardTxtNbr()
    {
        _carNbrTxt.text = _deck.Count.ToString();
    }
}
