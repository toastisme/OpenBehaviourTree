using UnityEngine;
namespace OpenBehaviourTree{
public class PrioritySelector : Node
{    
    /**
    * \class OpenBehaviourTree.PrioritySelector
    * Represents a priority node in the BehaviourTree class.
    * Child nodes are evaluated in order, and if any succeed
    * this node evaluates as succeeded.
    * Returns failed if all child nodes return failed.
    */

    public PrioritySelector(
        string taskName,
        Node parentNode=null
    ) :base(
        taskName:taskName,
        parentNode:parentNode
    ){}

    public override NodeState Evaluate() { 

        /**
        * Child nodes are evaluated in order, and if any succeed 
        * this node evaluates as succeeded.
        * Returns failed if all child nodes return failed.
        */

        foreach (Node node in ChildNodes){
            switch(node.Evaluate()){
                case NodeState.Idle:
                    CurrentState = NodeState.Idle;
                    return CurrentState;
                case NodeState.Failed:
                    continue;
                case NodeState.Succeeded:
                    CurrentState = NodeState.Succeeded;
                    return CurrentState;
                case NodeState.Running:
                    CurrentState = NodeState.Running;
                    return CurrentState;
                default:
                    continue;
            }
        }

        CurrentState = NodeState.Failed;
        return CurrentState;
    }
    public override NodeType GetNodeType(){
        if (TaskName == "Root"){
            return NodeType.Root;
        }
        return NodeType.PrioritySelector;
        }
}
}