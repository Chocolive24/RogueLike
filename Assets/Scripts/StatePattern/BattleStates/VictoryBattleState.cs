using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryBattleState : IState
{
    public VictoryBattleState(GameManager gameManger)
    {
        _gameManager = gameManger;
    }

    private GameManager _gameManager;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public void OnEnter()
    {
        Debug.Log("Enter Victory");
        // Create rewards
    }

    public void OnExit()
    {
        _gameManager.IsInBattleState = false;
    }

    public void Tick()
    {
        // idk
    }
}
