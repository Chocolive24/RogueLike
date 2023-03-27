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

    private Dictionary<Vector3, TileCell> _tiles;
    private Dictionary<Vector2, TileBase> _test;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public Vector2 Size => new Vector2(_width, _height);
    public Tilemap WorldTilemap => _worldTilemap;
    public Dictionary<Vector3, TileCell> Tiles => _tiles;

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
        
    }
    
    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector3, TileCell>();
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var randomRuleTile = Random.Range(0, 6) == 0 ? _wallRuleTile : _groundRuleTile;
                
                _worldTilemap.SetTile(_worldTilemap.WorldToCell(new Vector3(x, y, 0)), randomRuleTile);
        
                // Stock the Cell position !
                var spawnedTile = _worldTilemap.GetInstantiatedObject(_worldTilemap.WorldToCell
                    (new Vector3(x, y, 0))).GetComponent<TileCell>();
        
                spawnedTile.name = $"Tile {x}, {y}";
                
                spawnedTile.Init(x, y);
        
                _tiles[WorldToCellCenter(spawnedTile.transform.position)] = spawnedTile;
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

    public TileCell GetTileAtPosition(Vector3 pos)
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

    public Vector3 WorldToCellCenter(Vector3 position)
    {
        return _worldTilemap.GetCellCenterLocal(_worldTilemap.WorldToCell(position));
    }
}
