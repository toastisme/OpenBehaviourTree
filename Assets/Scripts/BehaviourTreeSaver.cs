using System.Collections.Generic;
using System;
using UnityEngine;

namespace Behaviour{
public class BehaviourTreeSaver
{
    public static void AddNodeToSerializedNodes(Node node, 
                                         ref List<SerializableNode> serializedNodes) {
        /**
         *Recursive function to write all nodes depth first into serializedNodes
         */

        if (node.ParentNode == node){
            throw new Exception("Node is its own parent.");
        }

        bool invertCondition = false;
        if (node is Decorator decorator){
            invertCondition = decorator.invertCondition;
        }

        float misc1 = -1f;
        float misc2 = -1f;
        if (node.GetNodeType() == NodeType.ActionWait){
            ActionWaitNode awn = (ActionWaitNode)node;
            misc1 = awn.WaitTime; 
            misc2 = awn.RandomDeviation;
        }
        else if (node.GetNodeType() == NodeType.ProbabilityWeight){
            ProbabilityWeight pw = (ProbabilityWeight)node;
            misc1 = pw.GetWeight();
        }
        else{
            misc1 = node.HasTimeout() ? node.GetTimeout().GetTimerVal() : -1;
            misc2 = node.HasCooldown() ? node.GetCooldown().GetTimerVal() : -1;
        }

        var serializedNode = new SerializableNode () {
            type = (int)node.GetNodeType(),
            taskName = node.TaskName,
            childCount = node.ChildNodes.Count,
            invertCondition = invertCondition,
            misc1 = misc1,
            misc2 = misc2
        }
        ;
        serializedNodes.Add (serializedNode);
        foreach (var child in node.ChildNodes) {
            BehaviourTreeSaver.AddNodeToSerializedNodes (child, ref serializedNodes);
        }
    }
    public static GuiNodeData GetMetaData(GuiNode node){
        return new GuiNodeData(){
            displayName = node.DisplayName,
            xPos = node.GetPos().x,
            yPos = node.GetPos().y
        };
    }

    public static void AddNodeMetaData(CompositeGuiNode node, ref List<GuiNodeData> guiNodeData){
        /**
         *Recursive function to write all GuiNode meta data first into guiNodeData
         */

        
        // Add decorators
        if (node.Decorators != null){
            foreach(var decorator in node.Decorators){
                guiNodeData.Add(BehaviourTreeSaver.GetMetaData(decorator));
            }
        }

        // Add self
        guiNodeData.Add(BehaviourTreeSaver.GetMetaData(node));

        // Add children
        foreach (var child in node.ChildConnections){
            if (child.HasProbabilityWeight()){
                guiNodeData.Add(BehaviourTreeSaver.GetMetaData(child.probabilityWeight));
            }
            AddNodeMetaData(child.childNode, ref guiNodeData);
        }
    }

    public static void SaveTree(
        CompositeGuiNode guiRootNode,
        ref List<SerializableNode> serializedNodes,
        ref List<GuiNodeData> nodeMetaData
        ) {

            serializedNodes.Clear();
            nodeMetaData.Clear();
            Node rootNode = guiRootNode.BtNode;
            BehaviourTreeSaver.AddNodeToSerializedNodes(rootNode, ref serializedNodes);
            BehaviourTreeSaver.AddNodeMetaData(guiRootNode, ref nodeMetaData);
    }

    public static void SaveTree(
        PrioritySelector rootNode,
        ref List<SerializableNode> serializedNodes
        ) {
            serializedNodes.Clear();
            BehaviourTreeSaver.AddNodeToSerializedNodes(rootNode, ref serializedNodes);
    }


}
}