using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomData
{
    public RoomData(BoundsInt bounds, int enemySpawnWeight, bool hasEnemiesToFight)
    {
        _bounds = bounds;
        _enemySpawnWeight = enemySpawnWeight;
        _hasEnemiesToFight = hasEnemiesToFight;

        _roomNeighbours = new Dictionary<Vector3Int, RoomData>();
        
        _tilePositions = new List<Vector3Int>();
        SetAllTilePositions();

        _doors = new Dictionary<Vector3, Neighbourhood.Direction>();

        //GenerateEnemiesSpawnPoints();
    }
    
    // Attributes ------------------------------------------------------------------------------------------------------
    #region Positions Attributes

    private BoundsInt _bounds;
    private List<Vector3Int> _tilePositions;
    private Dictionary<Vector3Int, RoomData> _roomNeighbours;
    private int _nbrOfIteration = 0;

    private Dictionary<Vector3, Neighbourhood.Direction> _doors;

    #endregion;

    #region Enemies Attributes
    
    private int _enemySpawnWeight;

    private bool _hasEnemiesToFight;
    

    #endregion

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public BoundsInt Bounds
    {
        get => _bounds;
        set => _bounds = value;
    }

    public int EnemySpawnWeight => _enemySpawnWeight;

    public int NbrOfIteration
    {
        get => _nbrOfIteration;
        set => _nbrOfIteration = value;
    }

    public List<Vector3Int> DoorPositions => new List<Vector3Int>()
    {
        // Up Door.
        new (_bounds.xMin + (_bounds.size.x / 2), _bounds.yMax - 1, 0),
        // Right Door
        new (_bounds.xMax - 1, _bounds.yMin + (_bounds.size.y / 2), 0),
        // Left Door
        new (_bounds.xMin, _bounds.yMin + (_bounds.size.y / 2), 0),
        // Down Door
        new (_bounds.xMin + (_bounds.size.x / 2), _bounds.yMin, 0),
    };

    public Dictionary<Vector3, Neighbourhood.Direction> Doors => _doors;

    public List<Vector3Int> TilePositions => _tilePositions;

    // Methods ---------------------------------------------------------------------------------------------------------
    private void SetAllTilePositions()
    {
        _tilePositions.Clear();

        for (int x = _bounds.xMin + 2; x < _bounds.xMax - 2; x++)
        {
            for (int y = _bounds.yMin + 2; y < _bounds.yMax - 2; y++)
            {
                _tilePositions.Add(new Vector3Int(x, y, 0));
            }
        }
    }
    
    // Debug enemies TEst ----------------------------------------------------------------------------------------------
    
    // private void GenerateEnemiesSpawnPoints()
    // {
    //     int remainginWeight = _enemySpawnWeight;
    //
    //     while (remainginWeight > 0)
    //     {
    //         var weight = GetRandomEnemyWeight();
    //
    //         // TODO ne pas s'occuper des positions ici. le unitmanger le fera au début du jeu.
    //         
    //         var spawnPos = _tilePositions[Random.Range(0, _tilePositions.Count)];
    //         
    //         _enemiesToSpawnData[spawnPos] = weight;
    //         
    //         remainginWeight -= weight;
    //     }
    // }
    //
    // private int GetRandomEnemyWeight()
    // {
    //     return _enemiesData.OrderBy(o => Random.value).First().Weight;
    // }

    // End of Debug Part -----------------------------------------------------------------------------------------------

    public Vector3Int GetARandomTilePosition()
    {
        return _tilePositions[Random.Range(0, _tilePositions.Count)];
    }
    
    public List<Vector3Int> GetDoorPositions()
    {
        List<Vector3Int> doorPositions = new List<Vector3Int>();
        
        foreach (var neighbour in Neighbourhood.CardinalNeighbours)
        {
            Vector3Int gridNeighbour = new Vector3Int(
                (int)neighbour.Value.x * _bounds.size.x, (int)neighbour.Value.y * _bounds.size.y, 0);

            Vector3Int roomNeighbourPos = _bounds.position + gridNeighbour;

            if (_roomNeighbours != null)
            {
                if (_roomNeighbours.ContainsKey(roomNeighbourPos))
                {
                    Vector3 doorPos = DoorPositions[(int)neighbour.Key];
                    
                    doorPositions.Add(new Vector3Int((int)doorPos.x, (int)doorPos.y, 0));
                    _doors[doorPos] = neighbour.Key;
                }
            }
        }
        
        return doorPositions;
    }
    
    public void AddRoomNeighbour(Vector3Int neighbourPos, RoomData roomNeighbour)
    {
        _roomNeighbours[neighbourPos] = roomNeighbour;
    }

    public RoomData GetRoomNeighbourByAPosition(Vector3Int position)
    {
        foreach (var room in _roomNeighbours)
        {
            if (room.Value.IsPositionInBounds(position))
            {
                return room.Value;
            }
        }

        return null;
    }

    public bool IsPositionInBounds(Vector3Int position)
    {
        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
        {
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
            {
                if (position.x == x && position.y == y)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
