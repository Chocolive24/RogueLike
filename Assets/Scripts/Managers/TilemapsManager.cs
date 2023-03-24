using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapsManager : MonoBehaviour
{
    // Singleton -----------------------------------------------------------------------------------------------
    private static TilemapsManager _instance;
    public static TilemapsManager Instance { get { return _instance; } }
    
    // Tilemaps and Tiles --------------------------------------------------------------------------------------
    [SerializeField] private Tilemap _movementTilemap;
    [SerializeField] private Tilemap _attackTilemap;
    [SerializeField] private RuleTile _movementRuleTile;
    [SerializeField] private RuleTile _attackRuleTile;
    [SerializeField] private TileCell _movementTile;
    [SerializeField] private TileCell _attackTile;

    [SerializeField] private GameObject _tilemapPrefab;

    // Getters and Setters -------------------------------------------------------------------------------------
    public Tilemap AttackTilemap => _attackTilemap;
    public RuleTile AttackRuleTile => _attackRuleTile;
    public TileCell AttackTile => _attackTile;

    // ---------------------------------------------------------------------------------------------------------
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Tilemap InstantiateTilemap(string name)
    {
        var tilemmap = Instantiate(_tilemapPrefab, Vector3.zero, Quaternion.identity);

        tilemmap.name = name;
        
        var grid = FindObjectOfType<Grid>();

        if (grid)
        {
            tilemmap.transform.parent = grid.transform;
        }

        return tilemmap.GetComponent<Tilemap>();
    }
    
    /// <summary>
    /// Dijkstra Research within a specified range.
    /// Returns a Dictionary which has for Key the Tile's position and for Value the Tile's Distance.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="range"></param>
    /// <param name="gameObject">Check if the gameObject is a BaseAttackCard, if so, we need to add the enemies
    ///                          Tiles to the availableTiles.</param>
    /// <returns></returns>
    public Dictionary<Vector2, int> GetAvailableTiles(Vector2 startPos, int range, GameObject gameObject)
    {
        var queue = new Queue<Vector2>();
        var distances = new Dictionary<Vector2, int> { { startPos, 0 } };

        queue.Enqueue(startPos);

        bool countEnemies = gameObject.GetComponent<BaseAttackCard>();

        while (queue.Count > 0)
        {
            var currentPos = queue.Dequeue();
            
            foreach (var direction in new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right })
            {
                var neighbor = currentPos + direction;
                
                if (IsPositionAvailable(neighbor, countEnemies) && !distances.ContainsKey(neighbor))
                {
                    if (distances[currentPos] + 1 <= range)
                    {
                        distances.Add(neighbor, distances[currentPos] + 1);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
        
        return distances;
    }

    private bool IsPositionAvailable(Vector2 position, bool countEnemies)
    {
        if (GridManager.Instance.GetTileAtPosition(position) != null)
        {
            TileCell tile = GridManager.Instance.GetTileAtPosition(position);
            
            if (countEnemies && tile.OccupiedUnit != null)
            {
                return tile.OccupiedUnit.Faction == Faction.Enemy;
            }

            return tile.Walkable;
        }

        return false;
    }
    
    // public virtual List<TileCell> GetInRangeTiles(TileCell startingTile, int range)
    // {
    //     // TODO il me faut des valeurs de distance (Dijkstra) pour pouvoir savoir si je peux aller sur les cases ou non
    //     // TODO en fonction de mes points de déplacement sur mon character.
    //
    //     _inRangeTiles = new List<TileCell>();
    //     int stepcount = 0;
    //     
    //     _inRangeTiles.Add(startingTile);
    //     
    //     _storedNeighbours = new List<TileCell>();
    //     _storedNeighbours.Add(startingTile);
    //
    //     while (stepcount < range)
    //     {
    //         _ancientNeighboursList = new List<TileCell>();
    //
    //         foreach (var storedNeighbour in _storedNeighbours)
    //         {
    //             List<Vector2> neighboursPos = new List<Vector2>();
    //
    //             neighboursPos = GetNeighboursPos(storedNeighbour);
    //
    //             foreach (var neighbourPos in neighboursPos)
    //             {
    //                 if ((neighbourPos.x >= 0 && neighbourPos.x < GridManager.Instance.Size.x) &&
    //                     (neighbourPos.y >= 0 && neighbourPos.y < GridManager.Instance.Size.y))
    //                 {
    //                     _ancientNeighboursList.Add(GridManager.Instance.GetTileAtPosition(neighbourPos));
    //                 }
    //             }
    //
    //             //CheckIfNeighboursAreInGrid();
    //             
    //             // if ((north.x >= 0 && north.x < GridManager.Instance.Size.x) &&
    //             //     (north.y >= 0 && north.y < GridManager.Instance.Size.y))
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(north));
    //             // }
    //             // if ((south.x >= 0 && south.x < GridManager.Instance.Size.x) &&
    //             //     (south.y >= 0 && south.y < GridManager.Instance.Size.y))
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(south));
    //             // }
    //             // if ((east.x >= 0 && east.x < GridManager.Instance.Size.x) &&
    //             //     (east.y >= 0 && east.y < GridManager.Instance.Size.y))
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(east));
    //             // }
    //             // if ((west.x >= 0 && west.x < GridManager.Instance.Size.x) &&
    //             //     (west.y >= 0 && west.y < GridManager.Instance.Size.y))
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(west));
    //             // }
    //             
    //             
    //             // if (north.x < GridManager.Instance.WorldTilemap.localBounds.max.x &&
    //             //     north.y < GridManager.Instance.WorldTilemap.localBounds.max.y &&
    //             //     north.x > GridManager.Instance.WorldTilemap.localBounds.min.x &&
    //             //     north.y > GridManager.Instance.WorldTilemap.localBounds.min.y)
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(north));
    //             // }
    //             //
    //             // if (south.x < GridManager.Instance.WorldTilemap.localBounds.max.x &&
    //             //     south.y < GridManager.Instance.WorldTilemap.localBounds.max.y &&
    //             //     south.x > GridManager.Instance.WorldTilemap.localBounds.min.x &&
    //             //     south.y > GridManager.Instance.WorldTilemap.localBounds.min.y)
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(south));
    //             // }
    //             //
    //             // if (east.x < GridManager.Instance.WorldTilemap.localBounds.max.x &&
    //             //     east.y < GridManager.Instance.WorldTilemap.localBounds.max.y &&
    //             //     east.x > GridManager.Instance.WorldTilemap.localBounds.min.x &&
    //             //     east.y > GridManager.Instance.WorldTilemap.localBounds.min.y)
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(east));
    //             // }
    //             //
    //             // if (west.x < GridManager.Instance.WorldTilemap.localBounds.max.x &&
    //             //     west.y < GridManager.Instance.WorldTilemap.localBounds.max.y &&
    //             //     west.x > GridManager.Instance.WorldTilemap.localBounds.min.x &&
    //             //     west.y > GridManager.Instance.WorldTilemap.localBounds.min.y)
    //             // {
    //             //     _neighbours.Add(GridManager.Instance.GetTileAtPosition(west));
    //             // }
    //         }
    //         
    //         _inRangeTiles.AddRange(_ancientNeighboursList);
    //         _storedNeighbours = _ancientNeighboursList.Distinct().ToList();
    //         stepcount++;
    //     }
    //
    //     return _inRangeTiles.Distinct().ToList();
    //     //return _aoeTiles.Distinct().ToList();
    //    //_availableNeighbours = _aoeTiles.Distinct().ToList();
    // }

    /// <summary>
    /// Draw the tiles that correspond to the area of effect of the current object
    /// </summary>
    /// <param name="availableTiles"></param> In range and available tile neighbours.
    /// <param name="aoeTiles"></param> The list that will contains theses tiles.
    /// <param name="tilemap"></param> The Tilemap on we want to draw our tiles
    /// <param name="ruleTile"></param> The RuleTile to apply to the tilemap
    /// <param name="tile"></param> The tile prefab
    public void DrawTilemap(Dictionary<Vector2, int> availableTiles, Tilemap tilemap, RuleTile ruleTile)
    {
        foreach (var availableTile in availableTiles)
        {
            var pos = tilemap.WorldToCell(new Vector3(availableTile.Key.x, availableTile.Key.y, 0));
            
            tilemap.SetTile(pos, ruleTile);
        }
    }
    
    public Tilemap GetTilemap(BaseCard baseCard)
    {
        if (baseCard.CardType == CardType.MoveCard)
        {
            return _movementTilemap;
        }
        if (baseCard.CardType == CardType.Attackcard)
        {
            return _attackTilemap;
        }

        return null;
    }

    public RuleTile GetRuleTile(BaseCard baseCard)
    {
        if (baseCard.CardType == CardType.MoveCard)
        {
            return _movementRuleTile;
        }
        if (baseCard.CardType == CardType.Attackcard)
        {
            return _attackRuleTile;
        }

        return null;
    }
    
    public TileCell GetTile(BaseCard baseCard)
    {
        if (baseCard.CardType == CardType.MoveCard)
        {
            return _movementTile;
        }
        if (baseCard.CardType == CardType.Attackcard)
        {
            return _attackTile;
        }

        return null;
    }
}
