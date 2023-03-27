using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.UIElements;
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
    public Dictionary<Vector3, int> GetAvailableTiles(Vector3 startPos, int range, GameObject gameObject)
    {
        var queue = new Queue<Vector3>();
        var distances = new Dictionary<Vector3, int> { { startPos, 0 } };

        queue.Enqueue(startPos);

        bool countEnemies = gameObject.GetComponent<BaseAttackCard>();

        while (queue.Count > 0)
        {
            var currentPos = queue.Dequeue();
            
            foreach (var direction in new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right })
            {
                var neighbor = GridManager.Instance.WorldToCellCenter(currentPos + direction);

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

    private bool IsPositionAvailable(Vector3 position, bool countEnemies)
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

    /// <summary>
    /// Draw the tiles corresponding to the area of effect of the current object
    /// </summary>
    /// <param name="availableTiles"></param> In range and available tile neighbours.
    /// <param name="tilemap"></param> The Tilemap on we want to draw our tiles
    /// <param name="ruleTile"></param> The RuleTile to apply to the tilemap
    public void DrawTilemap(Dictionary<Vector3, int> availableTiles, Tilemap tilemap, RuleTile ruleTile)
    {
        foreach (var availableTile in availableTiles)
        {
            var pos = tilemap.WorldToCell(new Vector3(availableTile.Key.x, availableTile.Key.y, 0));
            
            tilemap.SetTile(pos, ruleTile);
        }
    }

    public Dictionary<Vector3, int> GetPath(Vector3 startPos, Dictionary<Vector3, int> availableTiles)
    {
        var queue = new Queue<Vector3>();
        Dictionary<Vector3, int> path = new Dictionary<Vector3, int>();
    
        queue.Enqueue(startPos);
        path[startPos] = availableTiles[startPos];
    
        while (queue.Count > 0)
        {
            var currentPos = queue.Dequeue();

            // We don't need the tile with the distance 0 because it's our startPos, so we stop the loop before
            // accessing the 0 distance tile.
            if (availableTiles[currentPos] == 1)
            {
                break;
            }

            // Get the neighbor with the shortest distance from the current position
            var neighbor = GetShortestNeighbor(currentPos, availableTiles, path);

            // If we found a neighbor, add it to the path and queue for further processing
            if (neighbor != null)
            {
                var neighborPos = GridManager.Instance.WorldToCellCenter(neighbor.Value.Key);
                var neighborDistance = neighbor.Value.Value;
    
                path[neighborPos] = neighborDistance;
                queue.Enqueue(neighborPos);
            }
        }
    
        return path;
    }
    
    // The KeyValuePair type is a struct, which is a value type that can't be null.
    // By adding the ? after the type name, we're making it nullable, which means it can have a null value.
    private KeyValuePair<Vector3, int>? GetShortestNeighbor(Vector3 currentPos, 
                                                            Dictionary<Vector3, int> availableTiles, 
                                                            Dictionary<Vector3, int> path)
    {
        int distance = int.MaxValue;
        KeyValuePair<Vector3, int>? bestNeighbor = null;
    
        foreach (var direction in new Vector3[] 
                     { Vector3.up, Vector3.down, Vector3.left, Vector3.right })
        {
            var neighborPos = currentPos + direction;
    
            if (availableTiles.ContainsKey(neighborPos) && !path.ContainsKey(neighborPos))
            {
                var neighborDistance = availableTiles[neighborPos];
    
                if (neighborDistance < distance)
                {
                    distance = neighborDistance;
                    bestNeighbor = new KeyValuePair<Vector3, int>(neighborPos, distance);
                }
            }
        }
    
        return bestNeighbor;
    }

    
    public void DrawPathTilemap(Dictionary<Vector3, int> path, Tilemap tilemap, RuleTile ruleTile)
    {
        foreach (var availableTile in path)
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
