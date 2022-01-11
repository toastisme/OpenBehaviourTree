
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
    */

    BehaviourTreeTask btTask; // The task class
    Task executeTask; // The actual execute coroutine, within a task wrapper
    private BehaviourTreeBlackboard blackboard; // Used find the task class at runtime

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

    public void LoadTask(MonoBehaviour monoBehaviour){
        /**
            * Uses task to obtain the corresponding BehaviourTreeTask class,
            * calls its constructor and runs the BehaviourTreeTask Setup, using
            * the GameObject's monoBehaviour
            */

        Type type = TypeUtils.GetType(TaskName); // Full class name (include the namespaces)
        ConstructorInfo constructor = TypeUtils.ResolveEmptyConstructor(type);
        object[] EMPTY_PARAMETERS = new object[0]; 
        
        // Invoke the constructor
        btTask =  (BehaviourTreeTask)constructor.Invoke(EMPTY_PARAMETERS);
        btTask.SetBlackboard(blackboard:ref blackboard);
        btTask.Setup(monoBehaviour);
        ResetTask();
        
    }

    private void ResetTask(){
        // Stops the underlying coroutine and gets it ready to run again
        if(executeTask != null){
            if (executeTask.Running){
                executeTask.Stop();
            }
        }
        executeTask = new Task(btTask.ExecuteTask((x)=>NodeState=x), false);
    }

    public override NodeState Evaluate(){

        /**
        * If the node is idle with no action running, starts the action.
        * Returns the state of the node.
        */

        if (NodeState == NodeState.Idle && !executeTask.Running){
            executeTask.Start();
        }
        return NodeState;        
    }

    public override void ResetState(){

        /**
        * Stops the action if running.
        * Sets the NodeState to Idle.
        */

        if (executeTask.Running){
            executeTask.Stop();
        }
        ResetTask();
        NodeState = NodeState.Idle;
    }

    public override NodeType GetNodeType(){return NodeType.Action;}
}
}