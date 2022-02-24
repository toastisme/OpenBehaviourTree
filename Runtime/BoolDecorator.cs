using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{

public class BoolDecorator : Decorator
{
    /**
    * \class Behaviour.BoolDecorator
    * Represents a boolean decorator node in the BehaviourTree class.
    * The decorator node refers to a boolean (blackboard.GetBoolKeys()[displayTask]),
    * the current value of which dictates the the node's Nodestate.
    */

    public bool invertCondition = false;

    public BoolDecorator(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        Node parentNode = null,
        Node childNode = null
    ) : base(
        taskName:taskName,
        blackboard: ref blackboard,
        parentNode:parentNode,
        childNode:childNode

    ){
        if (childNode != null){
            ChildNodes.Add(childNode);
        }
    }

    public BoolDecorator(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        bool invertCondition,
        Node parentNode = null,
        Node childNode = null
    ) : base(
        taskName:taskName,
        blackboard: ref blackboard,
        parentNode:parentNode,
        childNode:childNode
    ){
        if (childNode != null){
            ChildNodes.Add(childNode);
        }
        this.invertCondition = invertCondition;
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
    public override NodeType GetNodeType(){return NodeType.BoolDecorator;}

    public override SerializableNode Serialize()
    {
        return new SerializableNode(){
            type=(int)GetNodeType(),
            taskName=TaskName,
            childCount=ChildNodes.Count,
            invertCondition=this.invertCondition
        };
    }
}
}