
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;

namespace Behaviour{
public class ActionNode : Node
{    
    /**
    * \class ActionNode
    * Represents an action node in the BehaviourTree class.
    * Action nodes run BehaviourTreeTasks when evaluated.
    */

    public BehaviourTreeTask btTask{get; private set;} // The task class
    Task executeTask; // The actual execute coroutine, within a task wrapper
    protected BehaviourTreeBlackboard blackboard; // Cached to give to the task class at runtime

    public ActionNode(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        Node parentNode=null
    ) :base(
        taskName:taskName,
        parentNode:parentNode
    ){
        this.blackboard = blackboard;
    }

    public virtual void LoadTask(MonoBehaviour monoBehaviour){

        /**
        * Uses TaskName to obtain the corresponding BehaviourTreeTask class,
        * calls its constructor and runs the BehaviourTreeTask Setup, using
        * the GameObject's monoBehaviour
        */

        Type type = TypeUtils.GetType(TaskName); 
        ConstructorInfo constructor = TypeUtils.ResolveEmptyConstructor(type);
        object[] EMPTY_PARAMETERS = new object[0]; 
        
        // Invoke the constructor
        btTask =  (BehaviourTreeTask)constructor.Invoke(EMPTY_PARAMETERS);
        btTask.SetBlackboard(blackboard:ref blackboard);
        btTask.Setup(monoBehaviour);
        ResetTask();
        
    }

    protected virtual void ResetTask(){
        // Stops the underlying coroutine and gets it ready to run again
        if(executeTask != null){
            if (executeTask.Running){
                executeTask.Stop();
            }
        }
        executeTask = new Task(btTask.ExecuteTask((x)=>CurrentState=x), false);
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
            ResetTimeout();
            StartTimeout();
        }
        if (TimeoutExceeded()){
            ResetTask();
            CurrentState = NodeState.Failed;            
            ResetTimeout();
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

    public override NodeType GetNodeType(){return NodeType.Action;}
    public override void UpdateBlackboard(ref BehaviourTreeBlackboard blackboard){
        this.blackboard = blackboard;
    }
}
}