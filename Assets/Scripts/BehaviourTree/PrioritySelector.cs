using UnityEngine;
namespace Behaviour{
public class PrioritySelector : Node
{    
    /**
    * \class PrioritySelector
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

        if (CooldownActive()){
            CurrentState = NodeState.Failed;
            return CurrentState;
        }


        foreach (Node node in ChildNodes){
            switch(node.Evaluate()){
                case NodeState.Idle:
                    CurrentState = NodeState.Idle;
                    return CurrentState;
                case NodeState.Failed:
                    continue;
                case NodeState.Succeeded:
                    CurrentState = NodeState.Succeeded;
                    StartCooldown();
                    return CurrentState;
                case NodeState.Running:
                    if (HasTimeout() && !TimeoutActive() && !TimeoutExceeded()){
                        StartTimeout();
                    }
                    if (!HasTimeout() || (TimeoutActive() && !TimeoutExceeded())){
                        CurrentState = NodeState.Running;
                    }
                    else{
                        CurrentState = NodeState.Failed;
                        if (!TimeoutActive() && TimeoutExceeded()){
                            ResetTimeout();
                        }
                    }
                    return CurrentState;
                default:
                    continue;
            }
        }

        if (!TimeoutActive() && TimeoutExceeded()){
            ResetTimeout();
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