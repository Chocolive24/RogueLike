using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private GameObject _exploringCam;
    [SerializeField] private GameObject _battleCam;

    [SerializeField] private float _battleCamYoffset = 1.225f;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        GridManager.OnDungeonGenerate += SetCamerasSpawnPoint;
        BattleManager.OnBattleStart += ChangeCamera;
        BattleManager.OnBattleEnd += ChangeCamera;
    }

    private void SetCamerasSpawnPoint(GridManager arg1, RoomData startRoom)
    {
        Vector3 spawnPos = new Vector3(startRoom.Bounds.center.x, startRoom.Bounds.center.y, -10);
        
        _exploringCam.transform.position = spawnPos;
        _battleCam.transform.position = new Vector3(spawnPos.x, spawnPos.y - _battleCamYoffset, spawnPos.z);
        
        _exploringCam.SetActive(true);
        _battleCam.SetActive(false);
    }

    private void ChangeCamera(BattleManager obj, RoomData battleRoom)
    {
        Vector3 spawnPos = new Vector3(battleRoom.Bounds.center.x, battleRoom.Bounds.center.y, -10);
        
        _exploringCam.transform.position = spawnPos;
        _battleCam.transform.position = new Vector3(spawnPos.x, spawnPos.y - _battleCamYoffset, spawnPos.z);
        
        _exploringCam.SetActive(!_exploringCam.activeSelf);
        _battleCam.SetActive(!_battleCam.activeSelf);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
