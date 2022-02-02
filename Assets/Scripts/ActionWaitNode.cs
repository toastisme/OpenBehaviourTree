using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;

namespace Behaviour{
public class ActionWaitNode : ActionNode
{    
    /**
    * \class ActionWaitNode
    * Represents an action node in the BehaviourTree class specifically for the Wait BehaviourTreeTask.
    */

    public Wait btWaitTask{get;private set;} // The task class
    public float WaitTime{get; set;}
    public float RandomDeviation{get; set;}
    Task executeTask; // The actual execute coroutine, within a task wrapper

    public ActionWaitNode(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        Node parentNode=null
    ) :base(
        taskName:taskName,
        blackboard: ref blackboard,
        parentNode:parentNode
    ){
    }

    public override void LoadTask(MonoBehaviour monoBehaviour){

        btWaitTask = new Wait(WaitTime, RandomDeviation);
        btWaitTask.SetBlackboard(blackboard:ref blackboard);
        btWaitTask.Setup(monoBehaviour);
        ResetTask();
        
    }

    protected override void ResetTask(){
        // Stops the underlying coroutine and gets it ready to run again
        if(executeTask != null){
            if (executeTask.Running){
                executeTask.Stop();
            }
        }
        executeTask = new Task(btWaitTask.ExecuteTask((x)=>CurrentState=x), false);
    }

    public override NodeState Evaluate(){

        /**
        * If the node is idle with no action running, starts the action.
        * Returns the state of the node.
        */

        if (CooldownActive()){
            if (executeTask.Running){
                ResetTask();
            }
            CurrentState = NodeState.Failed;
            return CurrentState;
        }

        if (CurrentState == NodeState.Idle && !executeTask.Running){
            executeTask.Start();
        }
        if (CurrentState == NodeState.Succeeded){
            StartCooldown();
        }
        return CurrentState;        
    }

    public override void ResetState(bool resetTimers=false){

        /**
        * Stops the action if running.
        * Sets the CurrentState to Idle.
        */

        if (executeTask.Running){
            executeTask.Stop();
        }
        ResetTask();
        CurrentState = NodeState.Idle;
    }

    public override NodeType GetNodeType(){return NodeType.ActionWait;}
    public override void UpdateBlackboard(ref BehaviourTreeBlackboard blackboard){
        this.blackboard = blackboard;
    }
    public override void AddCooldown(float timerVal){
        this.RandomDeviation = timerVal; 
    }
    public override void AddTimeout(float timerVal){
        this.WaitTime = timerVal; 
    }
}
}