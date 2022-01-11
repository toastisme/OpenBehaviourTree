
using System.Collections.Generic;

namespace Behaviour{
public class BehaviourTreeLoader
{
    public static Node NodeFactory(NodeType nodeType, 
                            string taskName,
                            ref BehaviourTreeBlackboard blackboard
                            ){
        switch(nodeType){
            case NodeType.Root:
                return new PrioritySelector(
                            taskName:"Root"
                            );
            case NodeType.SequenceSelector:
                return new SequenceSelector(
                        taskName:taskName
                );
            case NodeType.PrioritySelector:
                return new PrioritySelector(
                        taskName:taskName
                );
            case NodeType.ProbabilitySelector:
                return new ProbabilitySelector(
                        taskName:taskName,
                        blackboard:ref blackboard
                );
            case NodeType.ProbabilityWeight:
                return new ProbabilityWeight(
                        taskName:taskName
                );
            case NodeType.Decorator:
                return new Decorator(
                        taskName:taskName,
                        blackboard:ref blackboard
                );
            case NodeType.Action:
                return new ActionNode(
                        taskName:taskName,
                        blackboard:ref blackboard
                );
            default:
                throw new System.Exception("Unknown node type");
        }

    }
    public static Node ReadNodeFromSerializedNodes(ref int index,
                                           BehaviourTreeBlackboard blackboard,
                                           List<SerializableNode> serializedNodes
                                           ) {
        /**
         *Recursive function to write all serializedNodes into node
         */

        var serializedNode = serializedNodes [index];
        Node node = BehaviourTreeLoader.NodeFactory(
            nodeType:(NodeType)serializedNode.type,
            taskName:serializedNode.taskName,
            blackboard:ref blackboard
        );
        // Parent/child nodes are set after calling the constructor

        // The tree needs to be read in depth-first, since that's how we wrote it out.
        for (int i = 0; i != serializedNode.childCount; i++) {
            index++;
            Node childNode = BehaviourTreeLoader.ReadNodeFromSerializedNodes(
                index:ref index,
                blackboard:blackboard,
                serializedNodes:serializedNodes
                );
            childNode.SetParentNode(node);
        }
        return node;
    }
}
}