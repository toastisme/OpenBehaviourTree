
using System.Collections.Generic;

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
        Decorator
    }

    [Serializable]
    public struct SerializableNode{
        int type;
        string taskName;
        int childCount;
    }
    public abstract class Node
    {
        public NodeState NodeState {get; protected set;}
        /**
         * If a decision node (SequenceSelector, ProbabilitySelector, PrioritySelector), 
         * TaskName is the node type.
         * If a decorator, TaskName is a boolean key in the BehaviourTreeBlackboard.
         * If an ActionNode, TaskName is the name of a BehaviourTreeTask class.
         */ 
        public string TaskName {get; protected set;}
        public List<Node> ChildNodes{get; protected set;}
        protected Node ParentNode{get; protected set;}

        protected Node(
            string taskName,
            Node parentNode
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
            foreach(Node childNode in childNodes){
                if (childNode != exceptionNode){
                    childNode.ResetState();
                }
            }
        }

        public abstract NodeType GetNodeType();

        public void AddChildNode(Node childNode){
            ChildNodes.Add(childNode);
        }
        public void SetParentNode(Node parentNode){
            ParentNode = parentNode;
        }


    }
}
