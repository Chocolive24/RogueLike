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

    private HashSet<RoomData> _rooms;
    private HashSet<RoomData> _endRooms;
    private HashSet<Vector3Int> _occupiedPositions;
    
    private HashSet<Vector2Int> _groundPositions;
    private HashSet<Vector2Int> _wallPositions;

    private Vector2Int _roomGridSize;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] private Tilemap _dungeonTilemap;

    [SerializeField] private RuleTile _groundRuleTile, _wallRuleTile, _doorRuleTile;
    
    // Debug ---------------------
    //[SerializeField] private GameObject _endRoomDebug;

    // Methods ---------------------------------------------------------------------------------------------------------
    public void Generate()
    {
        _roomGridSize.x = _roomGridRatio.x * _roomSize.x;
        _roomGridSize.y = _roomGridRatio.y * _roomSize.y;
        
        _rooms = new HashSet<RoomData>();
        _endRooms = new HashSet<RoomData>();
        _occupiedPositions = new HashSet<Vector3Int>();

        int crashCounter = 0;
        
        do
        {
            if (crashCounter == 1)
            {
                Debug.Log("stop");
            }

            GenerateRooms();
            crashCounter++;
        } while (_rooms.Count != _minNbrOfRooms && crashCounter < 10000);
       
        Debug.Log("Distance CRASH " + crashCounter);
        
        // Draw tiles
        PaintDungeon(_dungeonTilemap);
    }

    private void GenerateRooms()
    {
        Vector3Int startGridPos = new Vector3Int(_roomGridSize.x / 2, _roomGridSize.y / 2, 0);

        RoomData startRoom = new RoomData(new BoundsInt(startGridPos, _roomSize));

        Queue<RoomData> roomQueue = new Queue<RoomData>();
        roomQueue.Enqueue(startRoom);

        if (_rooms.Count == 0)
        {
            _rooms.Add(startRoom);
        }
        
        _occupiedPositions.Add(startGridPos);
        
        int crashCounter = 0;
        
        int nbrOfIteration = 0;
        
        while (roomQueue.Count > 0 && crashCounter < 1000)
        {
            RoomData currentRoom = roomQueue.Dequeue();
            
            int neighbourAdded = 0;
            
            foreach (var neighbour in Neighbourhood.CardinalNeighbours)
            {
                Vector3Int gridNeighbour = new Vector3Int(
                    (int)neighbour.x * _roomSize.x, (int)neighbour.y * _roomSize.y, 0);

                Vector3Int roomNeighbourPos = currentRoom.Bounds.position + gridNeighbour;

                if (CheckForAbandon(roomNeighbourPos, _occupiedPositions))
                {
                    continue;
                }

                RoomData newRoom = new RoomData(new BoundsInt(roomNeighbourPos, _roomSize));
                newRoom.NbrOfIteration = nbrOfIteration;
                
                roomQueue.Enqueue(newRoom);
                _rooms.Add(newRoom);
                _occupiedPositions.Add(roomNeighbourPos);

                if (currentRoom.Bounds.position == startGridPos)
                {
                    var test = _rooms.First();
                    test.AddRoomNeighbourPosition(roomNeighbourPos);
                }
                else
                {
                    currentRoom.AddRoomNeighbourPosition(roomNeighbourPos);
                }
                
                newRoom.AddRoomNeighbourPosition(currentRoom.Bounds.position);
                
                neighbourAdded++;
                nbrOfIteration++;
            }

            if (neighbourAdded == 0)
            {
                _endRooms.Add(currentRoom);
            }
            
            crashCounter++;
        }
        
        //_endRoomDebug.transform.position = _endRooms.OrderBy(x => x.NbrOfIteration).Last().Bounds.center;
    }

    private bool CheckForAbandon(Vector3Int position, HashSet<Vector3Int> occupiedPositions)
    {
        // Out of grid limit.
        if (!IsPositionInGrid(position))
        {
            return true;
        }

        // The position has already been visited so there is a room.
        if (occupiedPositions.Contains(position))
        {
            return true;
        }

        // There is more than 1 Room neighbour at this position.
        if (CountNeighbours(position) > 1)
        {
            return true;
        }
        
        // There is already enough Rooms.
        if (_rooms.Count >= _minNbrOfRooms)
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

    private int CountNeighbours(Vector3Int position)
    {
        int nbrNeighbour = 0;

        foreach (var neighbour in Neighbourhood.CardinalNeighbours)
        {
            Vector3Int gridNeighbour = new Vector3Int(
                (int)neighbour.x * _roomSize.x, (int)neighbour.y * _roomSize.y, 0);

            Vector3Int positionToAnalyse = position + gridNeighbour;

            if (_occupiedPositions.Contains(positionToAnalyse))
            {
                nbrNeighbour++;
            }
        }

        return nbrNeighbour;
    }
    
    private void PaintDungeon(Tilemap tilemap)
    {
        _dungeonTilemap.ClearAllTiles();
        
        foreach (var room in _rooms)
        {
            _groundPositions = GetGroundPositions(room);

            _wallPositions = GetWallsPositions(room);
            
            PaintRoom(tilemap, _groundPositions, _wallPositions, room.GetDoorPositions());
        }
    }

    private void PaintRoom(Tilemap tilemap, HashSet<Vector2Int> groundPositions, HashSet<Vector2Int> wallPositions,
                            List<Vector3Int> roomNeighbourPos)
    {
        PaintTilesFromAListOfPositions(tilemap, _groundRuleTile, groundPositions);
        
        PaintTilesFromAListOfPositions(tilemap, _wallRuleTile, wallPositions);

        foreach (var doorPos in roomNeighbourPos)
        {
            tilemap.SetTile(doorPos, _doorRuleTile);
        }
    }

    private void PaintTilesFromAListOfPositions(Tilemap tilemap, RuleTile ruleTile, HashSet<Vector2Int> positions)
    {
        foreach (var pos in positions)
        {
            Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
            tilemap.SetTile(tilePos, ruleTile);
        }
    }
    
    private HashSet<Vector2Int> GetGroundPositions(RoomData room)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int x = room.Bounds.xMin + 1; x < room.Bounds.xMax - 1; x++)
        {
            for (int y = room.Bounds.yMin + 1; y < room.Bounds.yMax - 1; y++)
            {
                floor.Add(new Vector2Int(x, y));
            }
        }

        return floor;
    }
    
    private HashSet<Vector2Int> GetWallsPositions(RoomData room)
    {
        HashSet<Vector2Int> walls = new HashSet<Vector2Int>();

        for (int x = room.Bounds.xMin; x < room.Bounds.xMax; x++)
        {
            walls.Add(new Vector2Int(x, room.Bounds.yMin));
            walls.Add(new Vector2Int(x, room.Bounds.yMax - 1));
        }

        for (int y = room.Bounds.yMin; y < room.Bounds.yMax; y++)
        {
            walls.Add(new Vector2Int(room.Bounds.xMin, y));
            walls.Add(new Vector2Int(room.Bounds.xMax - 1, y));
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
