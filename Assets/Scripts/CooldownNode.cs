using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Behaviour{

public class SerializableCooldownNode:SerializableTimerNode{
    public bool activateOnSuccess;
    public bool activateOnFailure;

    public SerializableCooldownNode(
        int type,
        string taskName,
        int childCount,
        string valueKey,
        string randomDeviationKey,
        float value,
        float randomDeviation,
        bool activateOnSuccess,
        bool activateOnFailure
    ): base(
        type:type,
        taskName:taskName,
        childCount:childCount,
        valueKey:valueKey,
        randomDeviationKey:randomDeviationKey,
        value:value,
        randomDeviation:randomDeviation
    ){
        this.activateOnSuccess = activateOnSuccess;
        this.activateOnFailure = activateOnFailure;
    }
}

public class CooldownNode : TimerNode
{
    /**
    * \class CooldownNode
    * Represents a cooldown in the BehaviourTreeClass
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
        return new SerializableCooldownNode(
            type:(int)GetNodeType(),
            taskName:TaskName,
            childCount:ChildNodes.Count,
            valueKey:valueKey,
            randomDeviationKey:randomDeviationKey,
            value:TimerValue,
            randomDeviation:RandomDeviation,
            activateOnSuccess:activateOnSuccess,
            activateOnFailure:activateOnFailure
        );
    }
}
}