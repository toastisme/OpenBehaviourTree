
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
        string task;
        int childCount;
    }
    public abstract class Node
    {
        public NodeState NodeState {get; protected set;}
        public string NodeTask {get; protected set;}
        public List<Node> ChildNodes{get; protected set;}
        protected Node ParentNode{get; protected set;}

        protected Node(
            string task,
            Node parentNode
        ){
            this.NodeTask = task;
            this.ParentNode = parentNode;
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

        public void AddChildNode(Node childNode){
            ChildNodes.Add(childNode);
        }
        public void SetParentNode(Node parentNode){
            ParentNode = parentNode;
        }


    }
}
