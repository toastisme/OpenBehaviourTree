namespace Behaviour{
public class SequenceSelector : Node
{    
    /**
    * \class SequenceSelector
    * Represents a sequence node in the BehaviourTree class.
    * Child nodes are evaluated in order, and if any fail this node evaluates as failed.
    */

    public SequenceSelector(
        string taskName,
        Node parentNode=null
    ) :base(
        taskName:taskName,
        parentNode:parentNode
    ){}
    public override NodeState Evaluate() { 
        bool anyChildRunning = false; 
        
        foreach(Node node in ChildNodes) { 
            switch (node.Evaluate()) { 
                case NodeState.Idle:
                    NodeState = NodeState.Idle;
                    return NodeState;
                case NodeState.Failed: 
                    NodeState = NodeState.Failed; 
                    return NodeState;                     
                case NodeState.Succeeded: 
                    continue; 
                case NodeState.Running: 
                    ResetOtherStates(node);
                    anyChildRunning = true; 
                    break; 
                default: 
                    NodeState = NodeState.Succeeded; 
                    return NodeState; 
            } 
        } 
        NodeState = anyChildRunning ? NodeState.Running : NodeState.Succeeded; 
        return NodeState; 
    }
    public override NodeType GetNodeType(){return NodeType.SequenceSelector;}
}
}