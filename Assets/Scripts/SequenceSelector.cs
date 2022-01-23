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

        if (CooldownActive()){
            CurrentState = NodeState.Failed;
            return CurrentState;
        }

        if (!TimeoutActive() && !TimeoutExceeded()){
            StartTimeout();
        }
        
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
            if (!TimeoutExceeded()){
                CurrentState = NodeState.Running;                
            }
            else{
                ResetChildStates();
                CurrentState = NodeState.Failed;
                ResetTimeout();
            }
        }

        if (!TimeoutActive() && TimeoutExceeded()){
            ResetTimeout();
        }

        if (CurrentState == NodeState.Succeeded){
            StartCooldown();
        }

        return CurrentState; 
    }
    public override NodeType GetNodeType(){return NodeType.SequenceSelector;}
}
}