using System;
using System.Collections.Generic;

namespace Behaviour{
public enum NodeState{
    Running,
    Failed,
    Succeeded,
    Idle
}
public enum NodeType{
    Root,
    SequenceSelector,
    ProbabilitySelector,
    ProbabilityWeight,
    PrioritySelector,
    Action,
    Decorator
}

[Serializable]
public struct SerializableNode{
    public int type;
    public string taskName;
    public int childCount;
    public int parentIdx;
}
public abstract class Node
{
    public NodeState NodeState {get; protected set;}
    /**
        * If a decision node (SequenceSelector, ProbabilitySelector, PrioritySelector), 
        * TaskName is the node type.
        * If a decorator, TaskName is a boolean key in the BehaviourTreeBlackboard.
        * If a ProbabilityWeight, TaskName is an int or float key 
        * in the BehaviourTreeBlackboard.
        * If an ActionNode, TaskName is the name of a BehaviourTreeTask class.
        */ 
    public string TaskName {get; set;}
    public List<Node> ChildNodes{get; protected set;}
    public Node ParentNode{get; protected set;}

    protected Node(
        string taskName,
        Node parentNode = null
    ){
        this.TaskName = taskName;
        this.ParentNode = parentNode;
        this.ChildNodes = new List<Node>();
    }
    public virtual NodeState Evaluate(){return NodeState;}
    public virtual void ResetState(){
        NodeState = NodeState.Idle;
        foreach(Node childNode in ChildNodes){
            childNode.ResetState();
        }
    }
    public virtual void ResetOtherStates(Node exceptionNode){
        foreach(Node childNode in ChildNodes){
            if (childNode != exceptionNode){
                childNode.ResetState();
            }
        }
    }

    public abstract NodeType GetNodeType();

    public void AddChildNode(Node childNode){
        if (ChildNodes == null){
            ChildNodes = new List<Node>();
        }
        if (!ChildNodes.Contains(childNode)){
            ChildNodes.Add(childNode);
            childNode.SetParentNode(this);
        }
    }

    public void RemoveChildNode(Node childNode){
        if (ChildNodes != null){
            if (ChildNodes.Contains(childNode)){
                childNode.SetParentNode(null);
                ChildNodes.Remove(childNode);
            }
        }
    }

    public void SetParentNode(Node parentNode){
        ParentNode = parentNode;
        if (ParentNode != null){
            ParentNode.AddChildNode(this);
        }
    }

    public void Unlink(bool updateList=true){
        if (updateList){
            if (ChildNodes != null){
                foreach(Node childNode in ChildNodes){
                    childNode.SetParentNode(this.ParentNode);
                    ParentNode.AddChildNode(childNode);
                }
            }
        }
        else{
            if (ChildNodes != null){
                foreach(Node childNode in ChildNodes){
                    childNode.SetParentNode(null);
                }
            }
        }
        ParentNode.RemoveChildNode(this);
        ChildNodes = null;
    }

    public void InsertBeforeSelf(Node node){
        node.SetParentNode(ParentNode);
        ParentNode.RemoveChildNode(this);
        node.AddChildNode(this);
    }

}
}
