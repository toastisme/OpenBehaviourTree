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
    */

    protected BehaviourTreeBlackboard blackboard;

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

    public override void UpdateBlackboard(ref BehaviourTreeBlackboard blackboard){
        this.blackboard = blackboard;
    }

    public override NodeType GetNodeType(){return NodeType.Decorator;}

}
}