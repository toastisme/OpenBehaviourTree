using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;

namespace Behaviour{

public class ActionWaitNode : TimerNode
{    
    /**
    * \class ActionWaitNode
    * Represents an action node in the BehaviourTree class specifically
    * for waiting.
    */

    public ActionWaitNode(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        float timerValue,
        float randomDeviation,
        string valueKey = "",
        string randomDeviationKey = "",
        Node parentNode = null,
        Node childNode = null
    ) :base(
        taskName: taskName,
        blackboard: ref blackboard,
        timerValue:timerValue,
        randomDeviation:randomDeviation,
        valueKey:valueKey,
        randomDeviationKey:randomDeviationKey,
        parentNode:parentNode,
        childNode: childNode
    ){
    }

    public override void ResetTask(bool fail=false){

        ResetTimer();
        if (fail){CurrentState = NodeState.Failed;}      
    }

    public override NodeState Evaluate(){

        /**
        * If the node is idle with no action running, starts the action.
        * Returns the state of the node.
        */

        if ((CurrentState == NodeState.Idle) && !Active){
            StartTimer();
            CurrentState = NodeState.Running;
        }
        else if ((CurrentState == NodeState.Running) && !Active){
            CurrentState = NodeState.Succeeded;
        }

        return CurrentState;        
    }

    public override void ResetState(){

        /**
        * Stops the action if running.
        * Sets the CurrentState to Idle.
        */
        ResetTask();
        CurrentState = NodeState.Idle;
    }

    public override NodeType GetNodeType(){return NodeType.ActionWait;}
}
}