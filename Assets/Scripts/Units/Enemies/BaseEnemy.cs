using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseEnemy : BaseUnit
{
    // Attributes ------------------------------------------------------------------------------------------------------
    private Tilemap _movementTilemap;

    private bool _isSelected = false;
    protected bool _hasFinishedTheTurn = false;

    
    // References ------------------------------------------------------------------------------------------------------

    // Events ----------------------------------------------------------------------------------------------------------

    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public Dictionary<Vector3, int> AvailableTiles
    {
        get => _availableTiles;
        set => _availableTiles = value;
    }

    public Tilemap MovementTilemap
    {
        get => _movementTilemap;
        set => _movementTilemap = value;
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => _isSelected = value;
    }

    public bool HasFinishedTheTurn
    {
        get => _hasFinishedTheTurn;
        set => _hasFinishedTheTurn = value;
    }

    #endregion
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        
    }

    protected override void Start()
    {
        base.Start();
    }

    

    protected override void Update()
    {
        base.Update();

        // if (transform.position == _targetPos)
        // {
        //     _hasFinishedTheTurn = true;
        // }
        // else
        // {
        //     _hasFinishedTheTurn = false;
        // }
    }

    public override void FindAvailablePathToTarget(Vector3 targetPos)
    {
        _availableTiles = _tilemapsManager.GetAvailableTiles(transform.position, _movement, this.gameObject);
        
        base.FindAvailablePathToTarget(targetPos);

        if (_path.Count > 1)
        {
            _path.Remove(_path.Last());
        }
        
    }

    public int CalculateDistanceFromThePlayer()
    {
        _path = _tilemapsManager.FindPath(transform.position, _unitsManager.SelectedHero.transform.position);
        return _path.Count;
    }
}
