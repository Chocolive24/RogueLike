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

    [SerializeField] private int _damage;
    
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
        if (availableNeighbours.ContainsKey(GridManager.Instance.WorldTilemap.WorldToCell(
                GetStartingTile().transform.position)))
        {
            availableNeighbours.Remove(GridManager.Instance.WorldTilemap.WorldToCell(
                GetStartingTile().transform.position));
        }
        
        base.DrawTilemap(availableNeighbours, tilemap, ruleTile);
    }
}
