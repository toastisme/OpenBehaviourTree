
using System.Collections.Generic;

namespace BehaviourTree{
public class BehaviourTreeLoader
{
    BehaviourTreeBlackboard blackboard;

    static int ReadNodeFromSerializedNodes(int index,
                                           List<SerializableNode> serializedNodes,
                                           out Node node) {
        /**
         *Recursive function to write all serializedNodes into node
         */

        var serializedNode = serializedNodes [index];
        NodeType nodeType = (NodeType)serializedNode.nodeType;

        // Parent/child nodes are set after calling the constructor
        switch(nodeType){
            case NodeType.Root:
                node = new PrioritySelector(
                            taskName:"Root"
                            );
            case NodeType.SequenceSelector:
                node = new SequenceSelector(
                        taskName:serializedNode.taskName
                );
                break;
            case NodeType.PrioritySelector:
                node = new PrioritySelector(
                        takeName:serializedNode.taskName
                );
                break;
            case NodeType.ProbabilitySelector:
                node = new ProbabilitySelector(
                        serializedNode.taskName,
                        ref bt.blackboard
                );
                break;
            case NodeType.ProbabilityWeight:
                node = new ProbabilityWeight(
                        serializedNode.taskName
                );
                break;
            case NodeType.Decorator:
                node = new Decorator(
                        serializedNode.taskName,
                        ref bt.blackboard
                );
                break;
            case NodeType.Action:
                node = new ActionNode(
                        serializedNode.taskName,
                        ref bt.blackboard
                );
                break;
        }

        ;
        // The tree needs to be read in depth-first, since that's how we wrote it out.
        for (int i = 0; i != serializedNode.childCount; i++) {
            Node childNode;
            index = ReadNodeFromSerializedNodes (++index, out childNode);
            childNode.SetParentNode(node);
            node.AddChildNode(childNode);
        }
        return index;
    }
}
}