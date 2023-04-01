using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardedCardState : IState
{
    public void OnEnter()
    {
        Debug.Log("Enter Discarded");
    }

    public void OnExit()
    {
        Debug.Log("Exit Discarded");
    }

    public void Tick()
    {
        Debug.Log("Tick Discarded");
    }
}
