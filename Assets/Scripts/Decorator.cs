using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BehaviourTree{
public class Decorator : Node
{
    /**
    * \class Decorator
    * Represents a decorator node in the BehaviourTree class.
    * The decorator node refers to a boolean (blackboard.GetBoolKeys()[displayTask]), the current value of which
    * dictates the the node's Nodestate.
    */

    bool invertCondition = false;
    BehaviourTreeBlackboard blackboard;

    public Decorator(
        string taskName,
        Node parentNode,
        ref BehaviourTreeBlackboard blackboard,
        Node childNode
    ) : base(
        taskName:taskName,
        parentNode:parentNode
    ){
        ChildNodes.Add(childNode);
        this.blackboard = blackboard;
    }

    public override NodeState Evaluate(){
        if (!invertCondition){
            NodeState = blackboard.GetBoolKeys()[displayTask] ? childNodes[0].Evaluate() : NodeState.Failed;
        }
        else{
            NodeState = !blackboard.GetBoolKeys()[displayTask] ? childNodes[0].Evaluate() : NodeState.Failed;
        }
        if (NodeState == NodeState.Failed){childNodes[0].ResetState();} 
        return NodeState;
    } 
    public override NodeType GetNodeType(){return NodeType.Decorator;}
}
}