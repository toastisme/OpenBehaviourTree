using System.Collections.Generic;

namespace BehaviourTree{
public class BehaviourTreeSaver
{
    static void AddNodeToSerializedNodes(Node node, 
                                         out List<SerializedNode> serializedNodes) {
        /**
         *Recursive function to write all nodes depth first into serializedNodes
         */

        var serializedNode = new SerializableNode () {
            type = (int)node.GetNodeType(),
            taskName = node.TaskName,
            childCount = node.GetChildNodes().Count,
        }
        ;
        serializedNodes.Add (serializedNode);
        foreach (var child in n.children) {
            BehaviourTreeSaver.AddNodeToSerializedNodes (child, out serializedNodes);
        }
    }

}
}