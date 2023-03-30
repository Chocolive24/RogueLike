using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesTurnBattleState : IState
{
    public void OnEnter()
    {
        Debug.Log("Enter Enemy");
        BattleManager.Instance.HandleEnemiesTurn();
    }

    public void OnExit()
    {
        Debug.Log("Exit Enemy");
    }

    public void Tick()
    {
        Debug.Log("Tick Enemy");
    }
}
