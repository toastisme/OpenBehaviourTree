using System.Collections.Generic;
using System;

namespace Behaviour{
public class BehaviourTreeSaver
{
    public static void AddNodeToSerializedNodes(Node node, 
                                         ref List<SerializableNode> serializedNodes) {
        /**
         *Recursive function to write all nodes depth first into serializedNodes
         */

        var serializedNode = new SerializableNode () {
            type = (int)node.GetNodeType(),
            taskName = node.TaskName,
            childCount = node.ChildNodes.Count,
        }
        ;
        serializedNodes.Add (serializedNode);
        foreach (var child in node.ChildNodes) {
            BehaviourTreeSaver.AddNodeToSerializedNodes (child, ref serializedNodes);
        }
    }

}
}