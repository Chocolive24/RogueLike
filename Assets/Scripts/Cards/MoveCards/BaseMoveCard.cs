using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseMoveCard : BaseCard
{
    public BaseMoveCard(string name, int manaCost, Rarety rarety, CardType cardType, HeroClass heroClass,
                        int aeraOfEffect)
        : base(name, manaCost, rarety, cardType, heroClass, aeraOfEffect)
    {
        
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    private Dictionary<Vector3, int> _path;
    private Tilemap _pathTilemap;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public Dictionary<Vector3, int> Path
    {
        get => _path;
        set => _path = value;
    }

    public Tilemap PathTilemap
    {
        get => _pathTilemap;
        set => _pathTilemap = value;
    }

    #endregion
    
    // Methods ---------------------------------------------------------------------------------------------------------
    
    protected override void Start()
    {
        base.Start();
        _path = new Dictionary<Vector3, int>();
    }

    protected override void Update()
    {
        base.Update();
        _cardEffectTxt.text = "Move " + _aeraOfEffect + "\n squares \n";
    }

    public override void GetAvailableTiles()
    {
        _availableTiles = _tilemapsManager.GetAvailableTilesInRange(
            _gridManager.WorldToCellCenter(GetStartingTile().transform.position),
            _aeraOfEffect, false, false);
    }
}
