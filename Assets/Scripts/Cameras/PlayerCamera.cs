using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    private CinemachineVirtualCamera _vCam;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        UnitsManager.OnHeroSpawn += SetFollowPlayer;
    }

    private void SetFollowPlayer(UnitsManager unitsManager)
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _vCam.Follow = unitsManager.HeroPlayer.transform;
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
