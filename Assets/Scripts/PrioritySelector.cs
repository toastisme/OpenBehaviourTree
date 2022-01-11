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
                    NodeState = NodeState.Idle;
                    return NodeState;
                case NodeState.Failed:
                    continue;
                case NodeState.Succeeded:
                    NodeState = NodeState.Succeeded;
                    return NodeState;
                case NodeState.Running:
                    NodeState = NodeState.Running;
                    ResetOtherStates(node);
                    return NodeState;
                default:
                    continue;
            }
        }
        NodeState = NodeState.Failed;
        return NodeState;
    }
    public override NodeType GetNodeType(){return NodeType.PrioritySelector;}
}
}