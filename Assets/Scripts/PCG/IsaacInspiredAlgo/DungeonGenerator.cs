using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    
    // The size of the grid where we generate our rooms.
    [SerializeField] private Vector2Int _roomGridRatio;
    
    // The size of a simple room.
    [SerializeField] private Vector3Int _roomSize;

    [SerializeField] private int _minNbrOfRooms = 7, _maxNbrOfRooms = 10;

    private HashSet<BoundsInt> _roomsBounds;
    private HashSet<BoundsInt> _endRooms;
    
    private HashSet<Vector2Int> _groundPositions;
    private HashSet<Vector2Int> _wallPositions;

    private Vector2Int _roomGridSize;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] private Tilemap _dungeonTilemap;

    [SerializeField] private RuleTile _groundRuleTile;
    [SerializeField] private RuleTile _wallRuleTile;
    
    // Debug ---------------------
    [SerializeField] private GameObject _endRoomDebug;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void Generate()
    {
        DrunkardRoomAlgo();
        
        // Draw tiles
        PaintDungeon(_dungeonTilemap, _groundPositions, _wallPositions);
    }

    private void DrunkardRoomAlgo()
    {
        _roomGridSize.x = _roomGridRatio.x * _roomSize.x;
        _roomGridSize.y = _roomGridRatio.y * _roomSize.y;
        
        Vector3Int startGridPos = new Vector3Int(_roomGridSize.x / 2, _roomGridSize.y / 2, 0);
        
        BoundsInt startRoom = new BoundsInt(startGridPos, _roomSize);

        Queue<Vector3Int> gridPosQueue = new Queue<Vector3Int>();
        gridPosQueue.Enqueue(startGridPos);

        _roomsBounds = new HashSet<BoundsInt>();
        _roomsBounds.Add(startRoom);

        _endRooms = new HashSet<BoundsInt>();
        
        List<Vector3Int> visitedPositions = new List<Vector3Int>();
        
        int crashCounter = 0;
        
        while (gridPosQueue.Count > 0 && crashCounter < 1000)
        {
            Vector3Int currentGridPos = gridPosQueue.Dequeue();
            
            int neighbourAdded = 0;
            
            BoundsInt newRoom = new BoundsInt();
            
            foreach (var neighbour in Neighbourhood.CardinalNeighbours)
            {
                Vector3Int gridNeighbour = new Vector3Int(
                    (int)neighbour.x * _roomSize.x, (int)neighbour.y * _roomSize.y, 0);

                Vector3Int position = currentGridPos + gridNeighbour;

                if (CheckForAbandon(position, visitedPositions))
                {
                    continue;
                }

                gridPosQueue.Enqueue(position);
                
                visitedPositions.Add(position);

                neighbourAdded++;
                
                
                // TODO CREER DES OBBJETS DE TYPE ROOM (avec attribut bounds dedans + ajouter les voisiins a l'attributs roomNeighbours)
                newRoom = new BoundsInt(position, _roomSize);
                _roomsBounds.Add(newRoom);
            }

            if (neighbourAdded == 0)
            {
                _endRooms.Add(newRoom);
            }
            
            crashCounter++;
        }

        _endRoomDebug.transform.position = _endRooms.Last().center;
        
        Debug.Log(_roomsBounds.Count);
        Debug.Log(crashCounter);
    }

    private bool CheckForAbandon(Vector3Int position, List<Vector3Int> visitedPositions)
    {
        if (!IsPositionInGrid(position))
        {
            return true;
        }

        if (visitedPositions.Contains(position))
        {
            return true;
        }

        if (_roomsBounds.Count >= _minNbrOfRooms)
        {
            return true;
        }

        // 50 % chance to abandon.
        if (Random.Range(0, 2) == 1)
        {
            return true;
        }

        return false;
    }

    private bool IsPositionInGrid(Vector3Int position)
    {
        int rightPos = position.x + _roomSize.x;
        int upPos = position.y + _roomSize.y;

        return position.x >= 0 && position.y >= 0 && rightPos <= _roomGridSize.x && upPos <= _roomGridSize.y;
    }

    private void PaintDungeon(Tilemap tilemap, HashSet<Vector2Int> groundPositions, HashSet<Vector2Int> wallPositions)
    {
        _dungeonTilemap.ClearAllTiles();
        
        foreach (var room in _roomsBounds)
        {
            groundPositions = GetGroundPosition(room);

            wallPositions = GetWallsPositions(room);
            
            PaintRoom(tilemap, groundPositions, wallPositions);
        }
    }

    private void PaintRoom(Tilemap tilemap, HashSet<Vector2Int> groundPositions, HashSet<Vector2Int> wallPositions)
    {
        PaintTilesFromAListOfPositions(tilemap, _groundRuleTile, groundPositions);
        
        PaintTilesFromAListOfPositions(tilemap, _wallRuleTile, wallPositions);
    }

    private void PaintTilesFromAListOfPositions(Tilemap tilemap, RuleTile ruleTile, HashSet<Vector2Int> positions)
    {
        foreach (var pos in positions)
        {
            Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
            tilemap.SetTile(tilePos, ruleTile);
        }
    }
    
    private HashSet<Vector2Int> GetGroundPosition(BoundsInt room)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int x = room.xMin + 1; x < room.xMax - 1; x++)
        {
            for (int y = room.yMin + 1; y < room.yMax - 1; y++)
            {
                floor.Add(new Vector2Int(x, y));
            }
        }

        return floor;
    }
    
    private HashSet<Vector2Int> GetWallsPositions(BoundsInt bigRoom)
    {
        HashSet<Vector2Int> walls = new HashSet<Vector2Int>();

        for (int x = bigRoom.xMin; x < bigRoom.xMax; x++)
        {
            walls.Add(new Vector2Int(x, bigRoom.yMin));
            walls.Add(new Vector2Int(x, bigRoom.yMax - 1));
        }

        for (int y = bigRoom.yMin; y < bigRoom.yMax; y++)
        {
            walls.Add(new Vector2Int(bigRoom.xMin, y));
            walls.Add(new Vector2Int(bigRoom.xMax - 1, y));
        }

        return walls;
    }
    
    private Vector3Int GetCenterPositionToGrid(Vector3Int position)
    {
        Vector3Int centeredPosition = position;
        
        centeredPosition.x -= _roomSize.x / 2;
        centeredPosition.y -= _roomSize.y / 2;

        return centeredPosition;
    }
}
