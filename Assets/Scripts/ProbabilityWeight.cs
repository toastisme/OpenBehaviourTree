namespace BehaviourTree{
public class ProbabilityWeight : Node
{
    public ProbabilityWeight(
        string taskName,
        Node parentNode,
        Node childNode
    ) : base(
        taskName:taskName,
        parentNode:parentNode
    ){
        ChildNodes.Add(childNode);
    }
    public override NodeState Evaluate(){
        NodeState = ChildNodes[0].Evaluate();
        return NodeState;
    }
}
}