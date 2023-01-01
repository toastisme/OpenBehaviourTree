using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace OpenBehaviourTree{
public class CooldownNode : TimerNode
{
    /**
    * \class OpenBehaviourTree.CooldownNode
    * Represents a cooldown in the BehaviourTree class
    * The timer runs for TimerValue +/- RandomDeviation. 
    * If valueKey or RandomDeviationKey set, TimerValue and RandomDeviation
    * are set using these keys whenever StartTimer() is called. 
    */

    public bool activateOnSuccess;
    public bool activateOnFailure;

    public CooldownNode(
        ref BehaviourTreeBlackboard blackboard,
        float timerValue,
        float randomDeviation,
        string valueKey = "",
        string randomDeviationKey = "",
        Node parentNode = null,
        Node childNode = null,
        bool activateOnSuccess = true,
        bool activateOnFailure = false
    ) : base(
        taskName: "Cooldown",
        blackboard: ref blackboard,
        timerValue:timerValue,
        randomDeviation:randomDeviation,
        valueKey:valueKey,
        randomDeviationKey:randomDeviationKey,
        parentNode:parentNode,
        childNode: childNode
    ){
        this.activateOnSuccess = activateOnSuccess;
        this.activateOnFailure = activateOnFailure;
    }

    public override NodeState Evaluate(){
        if (Active){
            CurrentState = NodeState.Failed;
            return CurrentState;
        }
        CurrentState = ChildNodes[0].Evaluate();
        if (activateOnSuccess && CurrentState == NodeState.Succeeded){
            StartTimer();
        }
        else if (activateOnFailure && CurrentState == NodeState.Failed){
            StartTimer();
        }
        return CurrentState;
    }

    public override SerializableNode Serialize()
    {
        return new SerializableNode(){
            type=(int)GetNodeType(),
            taskName=TaskName,
            childCount=ChildNodes.Count,
            valueKey=valueKey,
            randomDeviationKey=randomDeviationKey,
            value=TimerValue,
            randomDeviation=RandomDeviation,
            activateOnSuccess=activateOnSuccess,
            activateOnFailure=activateOnFailure
        };
    }
}
}