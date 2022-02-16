
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

        // Get the class
        Type type = TypeUtils.GetType(TaskName); 
        ConstructorInfo constructor = TypeUtils.ResolveEmptyConstructor(type);
        object[] EMPTY_PARAMETERS = new object[0]; 
        
        // Setup the class
        btTask =  (BehaviourTreeTask)constructor.Invoke(EMPTY_PARAMETERS);
        btTask.SetBlackboard(blackboard:ref blackboard);
        btTask.Setup(monoBehaviour);
        ResetTask();
        
    }

    protected virtual void ResetTask(){

        /** 
         * Stops the underlying coroutine and gets it ready to run again
         */

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

        // If cooldown is active return Failed
        if (CooldownActive()){
            if (executeTask.Running){
                ResetTask();
            }
            CurrentState = NodeState.Failed;
            return CurrentState;
        }
        // If cooldown exceeded set to Idle
        else if (CooldownExceeded()){
            CurrentState = NodeState.Idle;
            ResetTask();
            ResetCooldown();
        }

        // If Idle, start the task
        if (CurrentState == NodeState.Idle && !executeTask.Running){
            executeTask.Start();
            ResetTimeout();
            StartTimeout();
            CurrentState = NodeState.Running;
        }

        // If timeout exceeded return Failed
        if (TimeoutExceeded()){
            // Record one evaluate call with Failed before returning to Idle
            if (CurrentState == NodeState.Running){
                ResetTask();
                CurrentState = NodeState.Failed;            
            }
            else{
                // Next evaluate call will then execute task again
                CurrentState = NodeState.Idle;
                ResetTimeout();
            }
        }

        if (CurrentState == NodeState.Succeeded){
            StartCooldown();
        }
        return CurrentState;        
    }

    public override void ResetState(){

        /**
        * Stops the action if running.
        * Sets CurrentState to Idle.
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