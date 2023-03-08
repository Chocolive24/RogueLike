using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerClass
{
    PALADIN,
    MAGE,
    CHASSEUR
}

public class Player : Character
{
    public Player(int hp, int attack, int shield, PlayerClass playerClass) : base(hp, attack, shield)
    {
        _playerClass = playerClass;
    }

    [SerializeField] private PlayerClass _playerClass;
    
    // Start is called before the first frame update
    void Start()
    {
        DisplayStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void DisplayStats()
    {
        Debug.Log("Player's stats : " + _playerClass  + " ");
        base.DisplayStats();

    }
}
