using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseAttackCard : BaseCard
{
    public BaseAttackCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass, 
                        int aeraOfEffect,int damages) 
        : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect)
    {
        _damages = damages;
    }

    [SerializeField] private int _damages;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //StartCoroutine(TestCo());
    }
    
    public override void DrawTilemap(Dictionary<Vector2, int> availableNeighbours, Tilemap tilemap, RuleTile ruleTile)
    {
        if (availableNeighbours.ContainsKey(GetStartingTile().Position))
        {
            availableNeighbours.Remove(GetStartingTile().Position);
        }
        
        base.DrawTilemap(availableNeighbours, tilemap, ruleTile);
    }
}
