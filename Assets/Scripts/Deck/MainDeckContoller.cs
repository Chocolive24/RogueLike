using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDeckContoller : DeckController
{
    [Header("Number of First Cards in the Deck")]
    [SerializeField] private int _basicAttCardNbr = 4;
    [SerializeField] private int _basicDefCardNbr = 4;
    
    protected override void SetDeck(UnitsManager obj)
    {
        base.SetDeck(obj);
        InstantiateBasicCard(CardsManager.Instance.ScrBasicAttackCards, _basicAttCardNbr);
    }
}
