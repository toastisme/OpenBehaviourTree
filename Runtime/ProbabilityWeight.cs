namespace Behaviour{

public class ProbabilityWeight : Node
{

    /**
     * \class ProbabilityWeight
     * Node placed between a ProbabilitySelector and its
     * child nodes. Holds either a constant weight, or a blackboard
     * key that links to an int or float variable.
     * The weight value is used to influence which nodes are selected
     * by the ProbabilitySelector.
     */

    float weight=1f;
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

    public void SetWeight(float weight){
        this.weight = weight;
    }

    public float GetWeight(){return weight;}

    public bool HasConstantWeight(){
        return (TaskName == "Constant Weight");        
    }

    public override SerializableNode Serialize()
    {
        return new SerializableNode(){
            type=(int)GetNodeType(),
            taskName=TaskName,
            childCount=ChildNodes.Count,
            weight=this.weight
        };
    }

}
}