namespace Behaviour{
public class PrioritySelector : Node
{    
    /**
    * \class PrioritySelector
    * Represents a priority node in the BehaviourTree class.
    * Child nodes are evaluated in order, and if any succeed this node evaluates as succeeded.
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
                    ResetOtherStates(node);
                    return CurrentState;
                default:
                    continue;
            }
        }
        CurrentState = NodeState.Failed;
        return CurrentState;
    }
    public override NodeType GetNodeType(){return NodeType.PrioritySelector;}
}
}