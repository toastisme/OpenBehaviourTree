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
        NodeState = ChildNodes[0].Evaluate();
        return NodeState;
    }

        public override NodeType GetNodeType()
        {
            return NodeType.ProbabilityWeight;
        }
    
}
}