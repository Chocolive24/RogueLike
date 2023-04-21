using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorTileCell : TileCell
{
    // Attributes ------------------------------------------------------------------------------------------------------
    private bool _isOpen;
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<DoorTileCell> OnDoorTileEneter;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        SetDoorOpen(true);
    }

    public override void SetUnit(BaseUnit unit)
    {
        base.SetUnit(unit);
        
        OnDoorTileEneter?.Invoke(this);
    }

    public void SetDoorOpen(bool isOpen)
    {
        _isOpen = isOpen;
        _isWalkable = _isOpen;
    }
}
