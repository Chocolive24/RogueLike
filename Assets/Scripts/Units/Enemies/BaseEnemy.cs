using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseEnemy : BaseUnit
{
    [SerializeField] private int _distanceFromThePlayer;

    //private List<TileCell> _inRangeTiles;
    //private List<TileCell> _walkableInRangeTiles;

    private Dictionary<Vector3, int> _availableTiles;

    private Tilemap _movementTilemap;

    private bool _isSelected = false;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    // public List<TileCell> InRangeTiles
    // {
    //     get => _inRangeTiles;
    //     set => _inRangeTiles = value;
    // }

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

    // -----------------------------------------------------------------------------------------------------------------
    
    private void Awake()
    {
        _availableTiles = new Dictionary<Vector3, int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // foreach (var tile in _availableTilesContainer)
        // {
        //     Destroy(tile.gameObject);
        // }
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
