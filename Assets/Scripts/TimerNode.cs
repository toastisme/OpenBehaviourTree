using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


namespace Behaviour{
public class TimerNode : Node
{
    /**
    * \class TimerNode
    * Represents a timer in the BehaviourTreeClass
    * (that things like timeouts and cooldowns can derive from)
    * The timer runs for TimerValue +/- RandomDeviation. 
    * If valueKey or RandomDeviationKey set, TimerValue and RandomDeviation
    * are set using these keys whenever StartTimer() is called. 
    */

    public bool Active{get; protected set;}

    string valueKey = "";
    string randomDeviationKey = "";

    public float TimerValue{get; set;}
    public float RandomDeviation{get; set;}

    MonoBehaviour monoBehaviour;
    BehaviourTreeBlackboard blackboard;
    Coroutine timer;

    public TimerNode(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        Node parentNode = null,
        Node childNode = null
    ) : base(
        taskName: taskName,
        parentNode:parentNode

    ){
        if (childNode != null){
            ChildNodes.Add(childNode);
        }
        this.blackboard = blackboard;
    }

    public virtual void LoadTask(MonoBehaviour monoBehaviour){
        this.monoBehaviour = monoBehaviour;
    }

    public override void ResetTask(bool fail=false){
        ResetTimer();
        if (fail){CurrentState = NodeState.Failed;}      
        ChildNodes[0].ResetTask(fail);
    }

    void LoadValues(){
        if (valueKey != ""){
            TimerValue = blackboard.GetWeightValue(valueKey);
        }
        if (randomDeviationKey != ""){
            RandomDeviation = blackboard.GetWeightValue(randomDeviationKey);
        }
    }

    public void StartTimer(){
        ResetTimer();
        LoadValues();
        timer = this.monoBehaviour.StartCoroutine(ExecuteTimer());
    }

    public void StopTimer(){
        if (timer != null){
            this.monoBehaviour.StopCoroutine(timer);
        }
        Active = false;
    }

    public virtual void ResetTimer(){
        StopTimer();
    }

    protected virtual void OnTimerComplete(){}

    IEnumerator ExecuteTimer(){
        float duration = TimerValue + UnityEngine.Random.Range(-RandomDeviation, RandomDeviation);
        duration = duration < 0 ? 0 : duration;
        Active = true;
        yield return new WaitForSeconds(duration);
        Active = false;
        OnTimerComplete();
    }

    public override void UpdateBlackboard(ref BehaviourTreeBlackboard blackboard){
        this.blackboard = blackboard;
    }

    public override NodeType GetNodeType()
    {
        return NodeType.Timer;
    }

    

}
}
