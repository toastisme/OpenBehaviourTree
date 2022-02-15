using System;
using System.Collections.Generic;
using UnityEngine;

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
    Decorator,
    ActionWait
}

[Serializable]
public struct SerializableNode{

    /**
     * \struct SerializableNode
     * Used to store Node data.
     */ 

    public int type;
    public string taskName;
    public int childCount;
    public int parentIdx;
    public bool invertCondition;
    public float misc1;
    public float misc2;
}
public abstract class Node
{
    /**
    * \class Node
    * Base class for a node in the BehaviourTree class.
    */

    public delegate void OnStateChangeDelegate();
    public event OnStateChangeDelegate OnStateChange;

    NodeState _currentState;
    public NodeState CurrentState {
        get{return _currentState;}
        protected set{
            if (_currentState != value && OnStateChange != null){
                OnStateChange();
            }
            _currentState = value;
        }
    }
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

    public NodeTimer nodeTimeout;
    public NodeTimer nodeCooldown;

    protected Node(
        string taskName,
        Node parentNode = null
    ){
        this.TaskName = taskName;
        this.ParentNode = parentNode;
        this.ChildNodes = new List<Node>();
        CurrentState = NodeState.Idle;
    }
    public virtual NodeState Evaluate(){return CurrentState;}

    public virtual void ResetState(){
        CurrentState = NodeState.Idle;
        foreach(Node childNode in ChildNodes){
            childNode.ResetState();
        }
    }

    public virtual void ResetChildStates (){
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

    public void ReplaceChildNodes(List<Node> newChildNodes){
        ChildNodes = newChildNodes;
    }

    public void SetParentNode(Node parentNode){
        ParentNode = parentNode;
        if (ParentNode != null){
            ParentNode.AddChildNode(this);
        }
    }

    public void Unlink(bool updateList=true){

        /**
         * Removes references of self from ParentNode
         * and ChildNodes.
         * If updateList, ParentNode is linked to 
         * ChildNodes (i.e removing self from the chain and 
         * relinking the chain).
         */

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
        ParentNode?.RemoveChildNode(this);
        ChildNodes = null;
    }

    public void InsertBeforeSelf(Node node){
        node.SetParentNode(ParentNode);
        ParentNode.RemoveChildNode(this);
        node.AddChildNode(this);
    }

    static public bool RequiresBlackboard(Node node){
        if (node is ActionNode){
            return true;
        }
        if (node is ProbabilitySelector){
            return true;
        }
        if (node is Decorator){
            return true;
        }
        return false;
    }

    public virtual void UpdateBlackboard(ref BehaviourTreeBlackboard blackboard){}

    public void AddTimeout(NodeTimer nodeTimeout){
        this.nodeTimeout = nodeTimeout;
    }

    public virtual void AddMisc1(float val){

        /**
         * SerializedNodes have two misc float fields that are
         * use for different purposes for different nodes
         */ 

        this.nodeTimeout = new NodeTimer(timerVal:val);
    }

    public virtual void AddMisc2(float val){

        /**
         * SerializedNodes have two misc float fields that are
         * use for different purposes for different nodes
         */ 

        this.nodeCooldown = new NodeTimer(timerVal:val);
    }

    public void AddCooldown(NodeTimer nodeCooldown){
        this.nodeCooldown = nodeCooldown;
    }

    public void RemoveTimeout(){
        this.nodeTimeout = null;
    }

    public void ResetTimeout(){
        nodeTimeout?.ResetTimer();
    }

    public void ResetCooldown(){
        nodeCooldown?.ResetTimer();
    }

    public void StartTimeout(){
        nodeTimeout?.StartTimer();
    }

    public void StopTimeout(){
        nodeTimeout?.StopTimer();
    }

    public void StartCooldown(){
        nodeCooldown?.StartTimer();
    }

    public void StopCooldown(){
        nodeCooldown?.StopTimer();
    }

    public void RemoveCooldown(){
        this.nodeCooldown = null;
    }

    public bool TimeoutExceeded(){
        if (nodeTimeout != null){
            return nodeTimeout.TimerExceeded();
        }   
        return false;
    }

    public bool CooldownExceeded(){
        if (nodeCooldown != null){
            return nodeCooldown.TimerExceeded();
        }   
        return false;
    }

    public bool CooldownActive(){
        if (nodeCooldown != null){
            return nodeCooldown.IsActive();
        }
        return false;
    }

    public bool TimeoutActive(){
        if (nodeTimeout != null){
            return nodeTimeout.IsActive();
        }
        return false;
    }

    public bool HasTimeout(){
        return (nodeTimeout != null);
    }

    public bool HasCooldown(){
        return (nodeCooldown != null);
    }

    public NodeTimer GetTimeout(){
        return nodeTimeout;
    }

    public NodeTimer GetCooldown(){
        return nodeCooldown;
    }

    public void SetStateDebug(NodeState nodeState){
        CurrentState = nodeState;
    }

}
}
