using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseMoveCard : BaseCard
{
    public BaseMoveCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass,
                        int aeraOfEffect)
        : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect)
    {
        
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    // public override void DrawTilemap(List<TileCell> inRangeTiles, List<TileCell> aoeTiles,
    //                                 Tilemap tilemap, RuleTile ruleTile, TileCell tile)
    // {
    //     var aoeTile = Instantiate(tile, GetStartingTile().transform.position, Quaternion.identity);
    //             
    //     _aoeTiles.Add(aoeTile);
    //     
    //     base.DrawTilemap(inRangeTiles, aoeTiles, tilemap, ruleTile, tile);
    // }
}
