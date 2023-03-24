using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }
    
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private int _width, _height;
    [SerializeField] private TileCell _ground, _wall;
    [SerializeField] private Transform _camTrans;
    [SerializeField] private Tilemap _worldTilemap;
    [SerializeField] private RuleTile _groundRuleTile, _wallRuleTile;

    private Dictionary<Vector2, TileCell> _tiles;
    private Dictionary<Vector2, TileBase> _test;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public Vector2 Size => new Vector2(_width, _height);
    public Tilemap WorldTilemap => _worldTilemap;
    public Dictionary<Vector2, TileCell> Tiles => _tiles;

    // Methods ---------------------------------------------------------------------------------------------------------
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

    private void Start()
    {
        // for (int x = 0; x < _width; x++)
        // {
        //     for (int y = 0; y < _height; y++)
        //     {
        //         _worldTilemap.SetTile(_worldTilemap.WorldToCell(new Vector3(x, y, 0)), _groundRuleTile);
        //
        //         var tile = _worldTilemap.GetInstantiatedObject(_worldTilemap.WorldToCell
        //             (new Vector3(x, y, 0)));
        //
        //         tile.GetComponent<TileCell>().Init(x, y);
        //         
        //         // transform.position = 0.5, 0.5, 0
        //         // WorldToCell(transform.position) = 0, 0, 0
        //         // Debug.Log(tile.transform.position);
        //         // Debug.Log(_worldTilemap.WorldToCell(tile.transform.position));
        //         
        //         
        //     }
        // }
    }

    // public void GenerateGrid()
    // {
    //     _tiles = new Dictionary<Vector2, TileCell>();
    //
    //     for (int x = 0; x < _width; x++)
    //     {
    //         for (int y = 0; y < _height; y++)
    //         {
    //             var randomTile = Random.Range(0, 6) == 3 ? _wall : _ground;
    //             
    //             var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
    //             spawnedTile.name = $"Tile {x}, {y}";
    //             
    //             spawnedTile.Init(x, y);
    //
    //             _tiles[new Vector2(x, y)] = spawnedTile;
    //         }
    //     }
    //
    //     _camTrans.transform.position = new Vector3((float)_width / 2f - 0.5f, 
    //         (float)_height / 2f - 0.5f -1f, -10);
    //     
    //     BattleManager.Instance.UpdateBattleState(BattleState.SPAWN_HEROES);
    // }

    public void GenerateGrid()
    {
        // _tiles = new Dictionary<Vector2, TileCell>();
        //
        //
        // for (int i = 0; i < _worldTilemap.gameObject.transform.childCount; i++)
        // {
        //     var spawnedTile = _worldTilemap.gameObject.transform.GetChild(i);
        //
        //     Vector2 pos = _worldTilemap.CellToWorld( new Vector3Int(
        //         (int)spawnedTile.transform.position.x, (int)spawnedTile.transform.position.y, 0));
        //     
        //     _tiles[pos] = spawnedTile.GetComponent<TileCell>();
        // }
        //
        // foreach (var item in _tiles)
        // {
        //     Debug.Log(item);
        // }
        
        // {
        //     BoundsInt bounds = _worldTilemap.cellBounds;
        //     TileBase[] allTiles = _worldTilemap.GetTilesBlock(bounds);
        //
        //     for (int x = 0; x < bounds.size.x; x++) {
        //         for (int y = 0; y < bounds.size.y; y++) {
        //             TileBase tile = allTiles[x + y * bounds.size.x];
        //             if (tile != null) {
        //                 Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
        //                 _tiles[new Vector2(x, y)] = ;
        //             } else {
        //                 Debug.Log("x:" + x + " y:" + y + " tile: (null)");
        //             }
        //         }
        //     }   
        // }
        
        _tiles = new Dictionary<Vector2, TileCell>();
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var randomRuleTile = Random.Range(0, 6) == 0 ? _wallRuleTile : _groundRuleTile;
                
                _worldTilemap.SetTile(_worldTilemap.WorldToCell(new Vector3(x, y, 0)), randomRuleTile);
        
                var spawnedTile = _worldTilemap.GetInstantiatedObject(_worldTilemap.WorldToCell
                    (new Vector3(x, y, 0))).GetComponent<TileCell>();
        
                spawnedTile.name = $"Tile {x}, {y}";
                
                spawnedTile.Init(x, y);
        
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
        
        _camTrans.transform.position = new Vector3((float)_width / 2f - 0.5f, 
                     (float)_height / 2f - 0.5f -1f, -10);
        
        BattleManager.Instance.UpdateBattleState(BattleState.SPAWN_HEROES);
    }

    public TileCell GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < _width / 2 && t.Value.Walkable).OrderBy
            (t => Random.value).First().Value;
    }

    public TileCell GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > _width / 2 && t.Value.Walkable).OrderBy
            (t => Random.value).First().Value;
    }

    public TileCell GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out TileCell tileCell))
        {
            return tileCell;
        }
        else
        {
            return null;
        }
    }

    
}
