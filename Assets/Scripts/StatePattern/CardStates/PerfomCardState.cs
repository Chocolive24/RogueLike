using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfomCardState : IState
{
    public void OnEnter()
    {
        Debug.Log("Enter Perform");
    }

    public void OnExit()
    {
        Debug.Log("Exit Perform");
    }

    public void Tick()
    {
        Debug.Log("Tick Perform");
    }
}
