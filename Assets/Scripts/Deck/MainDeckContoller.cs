using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDeckContoller : DeckController
{
    [Header("Number of First Cards in the Deck")]
    [SerializeField] private int _basicAttCardNbr = 4;
    [SerializeField] private int _basicDefCardNbr = 4;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        InstantiateBasicCard(CardsManager.Instance.ScrBasicAttackCards, _basicAttCardNbr);
        base.Start();
    }
}
