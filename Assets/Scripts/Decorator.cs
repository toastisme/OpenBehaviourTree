using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class Decorator : Node
{
    /**
    * \class Decorator
    * Represents a decorator node in the BehaviourTree class.
    * The decorator node refers to a boolean (blackboard.GetBoolKeys()[displayTask]), the current value of which
    * dictates the the node's Nodestate.
    */

    public bool invertCondition = false;
    BehaviourTreeBlackboard blackboard;

    public Decorator(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        Node parentNode = null,
        Node childNode = null
    ) : base(
        taskName:taskName,
        parentNode:parentNode
    ){
        if (childNode != null){
            ChildNodes.Add(childNode);
        }

        this.blackboard = blackboard;
    }

    public Decorator(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        bool invertCondition,
        Node parentNode = null,
        Node childNode = null
    ) : base(
        taskName:taskName,
        parentNode:parentNode
    ){
        if (childNode != null){
            ChildNodes.Add(childNode);
        }
        this.invertCondition = invertCondition;
        this.blackboard = blackboard;
    }

    public override NodeState Evaluate(){
        if (!invertCondition){
            CurrentState = blackboard.GetBoolKeys()[TaskName] ? ChildNodes[0].Evaluate() : NodeState.Failed;
        }
        else{
            CurrentState = !blackboard.GetBoolKeys()[TaskName] ? ChildNodes[0].Evaluate() : NodeState.Failed;
        }
        if (CurrentState == NodeState.Failed){ChildNodes[0].ResetState();} 
        return CurrentState;
    } 
    public override NodeType GetNodeType(){return NodeType.Decorator;}
}
}