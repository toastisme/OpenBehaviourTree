namespace OpenBehaviourTree{
public class SequenceSelector : Node
{    
    /**
    * \class OpenBehaviourTree.SequenceSelector
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
                    CurrentState = NodeState.Idle;
                    return CurrentState;
                case NodeState.Failed: 
                    CurrentState = NodeState.Failed; 
                    return CurrentState;                     
                case NodeState.Succeeded: 
                    continue; 
                case NodeState.Running: 
                    anyChildRunning = true; 
                    break; 
                default: 
                    CurrentState = NodeState.Succeeded; 
                    return CurrentState; 
            } 
            if (anyChildRunning){
                break;
            }
        } 
        if (anyChildRunning){
            CurrentState = NodeState.Running;                
            return CurrentState;
        }
        else{
            CurrentState = NodeState.Succeeded;
        }

        return CurrentState; 
    }
    public override NodeType GetNodeType(){return NodeType.SequenceSelector;}
}
}