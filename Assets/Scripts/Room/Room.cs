using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    BASIC,
    START,
    END,
    FREE_FIGHT,
    OBLIGATORY_FIGHT,
    SHOP,
}

public class Room : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private RoomType _type;
    
    private List<BaseEnemy> _enemies;

    private List<Room> _roomNeighbours;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<Room> OnRoomEnter;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public RoomType Type => _type;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void AddRoomNeighbour(Room roomNeighbour)
    {
        if (_roomNeighbours == null)
        {
            _roomNeighbours = new List<Room>();
        }
        
        _roomNeighbours.Add(roomNeighbour);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<BaseHero>())
        {
            OnRoomEnter?.Invoke(this);
        }
    }
}
