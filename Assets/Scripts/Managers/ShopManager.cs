using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    private GameObject _shopPanel;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    private void Awake()
    {
        DoorTileCell.OnDoorTileEnter += ActivateShop;
    }

    private void ActivateShop(DoorTileCell doorTile)
    {
        if (doorTile.Room.Type == RoomData.RoomType.SHOP)
        {
            _shopPanel.SetActive(true);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
