using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Sequence : BT_Node
{
    public BT_Sequence(string name) : base("SEQUENCE" + name)
    {
        
    }

    // Attributes ------------------------------------------------------------------------------------------------------
    protected int _idxCurenntChild = 0;
    
    // Methods ---------------------------------------------------------------------------------------------------------
    public override BT_Status Process()
    {
        if (_idxCurenntChild < _children.Count)
        {
            BT_Status status = _children[_idxCurenntChild].Process();

            Debug.Log("I'm the node : " + _children[_idxCurenntChild].Name + 
                      " / Depth : " + _children[_idxCurenntChild].Depth + " / Status : " + status);
            
            if (status == BT_Status.FAILURE)
            {
                return BT_Status.FAILURE;
            }
            else if (status == BT_Status.RUNNING)
            {
                return BT_Status.RUNNING;
            }
            else
            {
                _idxCurenntChild++;
                return BT_Status.RUNNING;
            }
        }
        else
        {
            return BT_Status.SUCCESS;
        }
    }
}
