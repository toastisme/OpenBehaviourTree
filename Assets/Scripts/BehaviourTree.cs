using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class BehaviourTree : ScriptableObject,  ISerializationCallbackReceiver 
{
    public PrioritySelector rootNode;
    public CompositeGuiNode guiRootNode;

    [SerializeField]
    public BehaviourTreeBlackboard blackboard;
    [SerializeField]
    public float evaluationDelay = 0.0001f;
    [SerializeField]
    public List<SerializableNode> serializedNodes;
    [SerializeField]
    public List<GuiNodeData> nodeMetaData;
    
    public void LoadTree(MonoBehaviour monoBehaviour){
        /**
        * Loads all actionNode tasks and runs their setup (getting required gameobject components etc.)
        */
        LoadActionNodes(monoBehaviour, rootNode);
    }

    public void LoadActionNodes(
        MonoBehaviour monoBehaviour,
        Node node
    )
    {
        if (node is ActionNode actionNode){
            actionNode.LoadTask(monoBehaviour);
        }
        else{
            if (node.ChildNodes != null){
                foreach(Node childNode in node.ChildNodes){
                    LoadActionNodes(monoBehaviour, childNode);
                }
            }
        }
    }

    public void ResetTree(){
        /**
        * Sets the state of all nodes to NodeState.Idle by calling ResetState on all nodes.
        */
        rootNode.ResetState();
    }

    public IEnumerator RunTree(){

        /**
        * The main loop for running the tree
        */

        NodeState rootState;
        ResetTree();
        while(true){
            rootState = rootNode.Evaluate();
            while(rootState != NodeState.Succeeded){
                rootState = rootNode.Evaluate();
                yield return new WaitForSeconds(evaluationDelay);
            }
            ResetTree();
            yield return null;
        }
    }
    public void OnAfterDeserialize() {
        //Unity has just written new data into the serializedNodes field.
        //let's populate our actual runtime data with those new values.
        if (serializedNodes.Count > 0) {
            int idx = 0;
            rootNode = (PrioritySelector)BehaviourTreeLoader.ReadNodeFromSerializedNodes (index:ref idx, 
                                                            blackboard:blackboard,
                                                            serializedNodes:serializedNodes
                                                            );
        }
    }

    bool CanSaveGuiData(){
        return (guiRootNode != null);
    }

    bool CanSaveTree(){
        return (rootNode != null);
    }

    public void OnBeforeSerialize() {
        // Unity is about to read the serializedNodes field's contents.
        // The correct data must now be written into that field "just in time".

        if (CanSaveTree()){
            serializedNodes = new List<SerializableNode>();
            nodeMetaData = new List<GuiNodeData>();

            serializedNodes.Clear();
            nodeMetaData.Clear();
            if (CanSaveGuiData()){
                BehaviourTreeSaver.SaveTree(
                    guiRootNode:guiRootNode,
                    serializedNodes:ref serializedNodes,
                    nodeMetaData:ref nodeMetaData
                );
            }
            else{
                BehaviourTreeSaver.SaveTree(
                    rootNode:rootNode,
                    serializedNodes:ref serializedNodes
                );
            }
        }
        // Now Unity is free to serialize this field, and we should get back the expected 
        // data when it is deserialized later.
    }
}
}
