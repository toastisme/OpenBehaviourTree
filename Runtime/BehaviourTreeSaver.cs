using System.Collections.Generic;
using System;
using UnityEngine;

namespace Behaviour{
public class BehaviourTreeSaver
{

    /**
    * \class Behaviour.BehaviourTreeSaver
    * Static methods to save serialized versions of Nodes and GuiNodes.
    */

    public static void AddNodeToSerializedNodes(Node node, 
                                         ref List<SerializableNode> serializedNodes) {
        /**
         *Recursive function to write all nodes depth first into serializedNodes
         */

        if (node.ParentNode == node){
            throw new Exception("Node is its own parent.");
        }

        serializedNodes.Add(node.Serialize());

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