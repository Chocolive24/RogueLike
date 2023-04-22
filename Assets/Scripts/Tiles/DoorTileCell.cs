using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorTileCell : TileCell
{
    // Attributes ------------------------------------------------------------------------------------------------------
    private bool _isOpen;

    private RoomData _room;
    private RoomData _nextRoom;

    private Neighbourhood.Direction _direction = Neighbourhood.Direction.NULL;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<DoorTileCell> OnDoorTileEnter;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public RoomData Room
    {
        get => _room;
        set => _room = value;
    }

    public RoomData NextRoom
    {
        get => _nextRoom;
        set => _nextRoom = value;
    }

    public Neighbourhood.Direction Direction
    {
        get => _direction;
        set => _direction = value;
    }

    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        SetDoorOpen(true);
    }

    public override void SetUnit(BaseUnit unit)
    {
        base.SetUnit(unit);

        OnDoorTileEnter?.Invoke(this);
    }

    public void SetDoorOpen(bool isOpen)
    {
        _isOpen = isOpen;
        _isWalkable = _isOpen;
    }

    public Vector3 GetNextRoomSpawnPos()
    {
        switch (_direction)
        {
            case Neighbourhood.Direction.UP:
                return transform.position + (2 * Vector3.up);
            case Neighbourhood.Direction.RIGHT:
                return transform.position + (2 * Vector3.right);
            case Neighbourhood.Direction.LEFT:
                return transform.position + (2 * Vector3.left);
            case Neighbourhood.Direction.DOWN:
                return transform.position + (2 * Vector3.down);
            case Neighbourhood.Direction.NULL:
                return Vector3.zero;
        }

        return Vector3.zero;
    }
    
    public RoomData GetRoomNeighbour()
    {
        var position = GetNextRoomSpawnPos();

        Vector3Int nextRoomSpawnPos = new Vector3Int((int)position.x, (int)position.y, 0);
        
        return _room.GetRoomNeighbourByAPosition(nextRoomSpawnPos);
    }
}
