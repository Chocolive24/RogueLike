using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyBT : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    protected BT_Node _root = new BT_Root("Root Node");

    //protected BT_Selector _actionSelector = new BT_Selector("Action Selector Node");

    protected BT_Sequence _attackSequence = new BT_Sequence("Attack");
    
    // Methods ---------------------------------------------------------------------------------------------------------
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _root.AddNode(_attackSequence);
        //_root.AddNode(_actionSelector);
        
        //_attackSequence.AddNode(new BT_Leaf("Movement Leaf Node", ));
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _root.Process();
    }

    // private BT_Status GoToTarget(BaseHero heroTarget)
    // {
    //     
    //     // if (Vector3.Distance(transform.position, destinationTrans.position) > _doorDistance)
    //     // {
    //     //     return BT_Status.RUNNING;
    //     // }
    //     //
    //     // return BT_Status.SUCCESS;
    // }
}
