namespace Behaviour{
public class ProbabilityWeight : Node
{
    public ProbabilityWeight(
        string taskName,
        Node parentNode=null,
        Node childNode=null
    ) : base(
        taskName:taskName,
        parentNode:parentNode
    ){
        if (childNode != null){
            ChildNodes.Add(childNode);
        }
    }
    public override NodeState Evaluate(){
        CurrentState = ChildNodes[0].Evaluate();
        return CurrentState;
    }

        public override NodeType GetNodeType()
        {
            return NodeType.ProbabilityWeight;
        }
    
}
}