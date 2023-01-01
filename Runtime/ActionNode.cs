
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;

namespace OpenBehaviourTree{
public class ActionNode : Node
{    
    /**
    * \class OpenBehaviourTree.ActionNode
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

    public override void ResetTask(bool failed=false){

        /** 
         * Stops the underlying coroutine and gets it ready to run again
         */

        if(executeTask != null){
            if (executeTask.Running){
                executeTask.Stop();
            }
        }
        executeTask = new Task(btTask.ExecuteTask((x)=>CurrentState=x), false);
        if (failed){CurrentState = NodeState.Failed;}
    }

    public override NodeState Evaluate(){

        /**
        * If the node is idle with no action running, starts the action.
        * Returns the state of the node.
        */

        // If Idle, start the task and set to Running
        if (CurrentState == NodeState.Idle && !executeTask.Running){
            executeTask.Start();
            CurrentState = NodeState.Running;
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