using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class CooldownNode : TimerNode
{
    /**
    * \class CooldownNode
    * Represents a cooldown in the BehaviourTreeClass
    * The timer runs for TimerValue +/- RandomDeviation. 
    * If valueKey or RandomDeviationKey set, TimerValue and RandomDeviation
    * are set using these keys whenever StartTimer() is called. 
    */

    public CooldownNode(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        Node parentNode = null,
        Node childNode = null
    ) : base(
        taskName: taskName,
        blackboard: ref blackboard,
        parentNode:parentNode,
        childNode: childNode
    ){}

    public override NodeState Evaluate(){
        if (Active){
            CurrentState = NodeState.Failed;
            return CurrentState;
        }
        CurrentState = ChildNodes[0].Evaluate();
        if (CurrentState == NodeState.Succeeded){
            StartTimer();
        }
        return CurrentState;
    }
}
}