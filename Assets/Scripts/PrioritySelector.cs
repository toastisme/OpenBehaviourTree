namespace BehaviourTree{
    public class PrioritySelector : Node
    {    
        /**
        * \class PrioritySelector
        * Represents a priority node in the BehaviourTree class.
        * Child nodes are evaluated in order, and if any succeed this node evaluates as succeeded.
        * Returns failed if all child nodes return failed.
        */

        public PrioritySelector(
            string task,
            Node parentNode
        ) :base(
            task:task,
            parentNode:parentNode
        ){}

        public override NodeState Evaluate() { 
            foreach (Node node in ChildNodes){
                switch(node.Evaluate()){
                    case NodeState.Idle:
                        nodeState = NodeState.Idle;
                        return nodeState;
                    case NodeState.Failed:
                        continue;
                    case NodeState.Succeeded:
                        nodeState = NodeState.Succeeded;
                        return nodeState;
                    case NodeState.Running:
                        nodeState = NodeState.Running;
                        ResetOtherStates(node);
                        return nodeState;
                    default:
                        continue;
                }
            }
            nodeState = NodeState.Failed;
            return nodeState;
        }
    }

}