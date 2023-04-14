using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseAttackCard : BaseCard
{
    public BaseAttackCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, 
                        int aeraOfEffect,int damage) 
        : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect)
    {
        _damage = damage;
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private int _damage;
    
    // References ------------------------------------------------------------------------------------------------------

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public int Damage => _damage;

    // Methods ---------------------------------------------------------------------------------------------------------
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        _cardEffectTxt.text = "Deal " + _damage + "\n damage \n";
    }
    
    public override void DrawTilemap(Dictionary<Vector3, int> availableNeighbours, Tilemap tilemap, RuleTile ruleTile)
    {
        if (availableNeighbours.ContainsKey(_gridManager.CurrentRoomTilemap.WorldToCell(
                GetStartingTile().transform.position)))
        {
            availableNeighbours.Remove(_gridManager.CurrentRoomTilemap.WorldToCell(
                GetStartingTile().transform.position));
        }
        
        base.DrawTilemap(availableNeighbours, tilemap, ruleTile);
    }
}
