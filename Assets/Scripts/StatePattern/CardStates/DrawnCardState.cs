using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawnCardState : IState
{
    public void OnEnter()
    {
        Debug.Log("Enter Drawn");
    }

    public void OnExit()
    {
        Debug.Log("Exit Drawn");
    }

    public void Tick()
    {
        Debug.Log("Tick Drawn");
    }
}
