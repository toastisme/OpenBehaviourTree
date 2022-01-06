using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree{
    public enum NodeState{
        Running,
        Failed,
        Succeeded,
        Idle
    }
    public enum NodeType{
        SequenceSelector,
        ProbabilitySelector,
        PrioritySelector,
        Action,
        Decorator,
    }

    [Serializable]
    public struct SerializableNode{
        int type;
        string task;
        int childCount;
    }
    public abstract class Node2
    {
        public NodeState NodeState {get; protected set;}
        public string NodeTask {get; protected set;}
        public List<Node2> ChildNodes{get; protected set;}
        protected Node2 ParentNode{get; protected set;}

        protected Node(
            string task,
            Node2 parentNode
        ){
            this.NodeTask = task;
            this.ParentNode = parentNode;
        }
        public virtual NodeState Evaluate(){return NodeState;}
        public virtual void ResetState(){
            NodeState = NodeState.Idle;
            foreach(Node2 childNode in ChildNodes){
                childNode.ResetState();
            }
        }
        public virtual void ResetOtherStates(Node2 exceptionNode){
            foreach(Node2 childNode in childNodes){
                if (childNode != exceptionNode){
                    childNode.ResetState();
                }
            }
        }

        public void AddChildNode(Node2 childNode){
            ChildNodes.Add(childNode);
        }
        public void SetParentNode(Node2 parentNode){
            ParentNode = parentNode;
        }


    }
}
