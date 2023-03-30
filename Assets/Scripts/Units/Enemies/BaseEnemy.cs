using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseEnemy : BaseUnit
{
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private int _distanceFromThePlayer;
    
    private Dictionary<Vector3, int> _availableTiles;

    private Tilemap _movementTilemap;

    private bool _isSelected = false;
    protected bool _hasFinishedTheTurn = false;

    // References ------------------------------------------------------------------------------------------------------
    private TilemapsManager _tilemapsManager;
    
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
        _availableTiles = new Dictionary<Vector3, int>();
    }

    protected override void Start()
    {
        base.Start();
        _tilemapsManager = TilemapsManager.Instance;
    }

    public void FindPathToTarget(Vector3 targetPos)
    {
        _path = _tilemapsManager.FindPath(transform.position, targetPos);

        _path.Remove(_path.Last());

        _targetPos = _path.First();
    }

    protected override void Update()
    {
        base.Update();

        if (transform.position == _targetPos)
        {
            _hasFinishedTheTurn = true;
        }
        else
        {
            _hasFinishedTheTurn = false;
        }
    }

    public int CalculateDistanceFromThePlayer()
    {
        Vector3 playerPos = Vector3.zero;
        
        foreach (var item in GridManager.Instance.Tiles)
        {
            if (item.Value.OccupiedUnit != null)
            {
                // TODO need to work with every hero.
                if (item.Value.OccupiedUnit.GetComponent<Hero1>())
                {
                    playerPos = item.Key;
                }
            }
        }

        return (int)(Mathf.Abs(transform.position.x - playerPos.x) + 
                     Mathf.Abs((transform.position.y - playerPos.y)));
    }
}
