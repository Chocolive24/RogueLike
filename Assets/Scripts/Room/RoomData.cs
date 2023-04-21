using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public RoomData(BoundsInt bounds)
    {
        _bounds = bounds;
    }
    
    private BoundsInt _bounds;
    private List<Vector3Int> _roomNeighbours;
    private int _nbrOfIteration = 0;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public BoundsInt Bounds
    {
        get => _bounds;
        set => _bounds = value;
    }

    public List<Vector3Int> GetDoorPositions()
    {
        List<Vector3Int> doorPositions = new List<Vector3Int>();

        for(int i = 0; i < Neighbourhood.CardinalNeighbours.Count; i++)
        {
            var neighbour = Neighbourhood.CardinalNeighbours[i];
            
            Vector3Int gridNeighbour = new Vector3Int(
                (int)neighbour.x * _bounds.size.x, (int)neighbour.y * _bounds.size.y, 0);

            Vector3Int roomNeighbourPos = _bounds.position + gridNeighbour;

            if (_roomNeighbours != null)
            {
                if (_roomNeighbours.Contains(roomNeighbourPos))
                {
                    doorPositions.Add(DoorPositions[i]);
                }
            }
            else
            {
                Debug.Log("NULL ROOMNEIGHBOUR");
            }
        }

        return doorPositions;
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

    public int NbrOfIteration
    {
        get => _nbrOfIteration;
        set => _nbrOfIteration = value;
    }

    // Methods ---------------------------------------------------------------------------------------------------------
    public void AddRoomNeighbourPosition(Vector3Int neighbourPos)
    {
        if (_roomNeighbours == null)
        {
            _roomNeighbours = new List<Vector3Int>();
        }
        
        _roomNeighbours.Add(neighbourPos);
    }
}
