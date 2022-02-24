using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class TimeoutNode : TimerNode
{
    /**
    * \class Behaviour.TimeoutNode
    * Represents a timeout in the BehaviourTree class
    * The timer runs for TimerValue +/- RandomDeviation. 
    * If valueKey or RandomDeviationKey set, TimerValue and RandomDeviation
    * are set using these keys whenever StartTimer() is called. 
    */

    public bool Exceeded{get; protected set;}

    public TimeoutNode(
        ref BehaviourTreeBlackboard blackboard,
        float timerValue,
        float randomDeviation,
        string valueKey = "",
        string randomDeviationKey = "",
        Node parentNode = null,
        Node childNode = null
    ) : base(
        taskName: "Timeout",
        blackboard: ref blackboard,
        timerValue:timerValue,
        randomDeviation:randomDeviation,
        valueKey:valueKey,
        randomDeviationKey:randomDeviationKey,
        parentNode:parentNode,
        childNode: childNode
    ){}

    public override NodeState Evaluate(){
        if (Exceeded){
            CurrentState = NodeState.Failed;
            ResetTask(true);
            return CurrentState;
        }
        if ((CurrentState == NodeState.Idle) && !Active){
            StartTimer();
        }
        CurrentState = ChildNodes[0].Evaluate();
        return CurrentState;
    }

    protected override void OnTimerComplete(){
        Exceeded = true;
    }

    public override void ResetTimer(){
        base.ResetTimer();
        Exceeded = false;
    }
}
}