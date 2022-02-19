using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class BehaviourTree : ScriptableObject,  ISerializationCallbackReceiver 
{

    /**
    * \class BehaviourTree
    * Class to store and run behaviour trees.
    */

    public PrioritySelector rootNode; // Actual root node
    public CompositeGuiNode guiRootNode; // GUI representation in BehaviourTreeEditor

    [SerializeField]
    public BehaviourTreeBlackboard blackboard;
    [SerializeField]
    public float evaluationDelay = 0.0001f;
    [SerializeField]
    public List<SerializableNode> serializedNodes;
    [SerializeField]
    public List<GuiNodeData> nodeMetaData; 

    public Action OnSetBlackboard;
    
    public void LoadTree(MonoBehaviour monoBehaviour){

        /**
        * Loads all actionNode tasks and runs their setup
        * (getting required gameobject components etc.)
        */

        LoadRuntimeProperties(monoBehaviour, rootNode);
    }

    public void LoadTree(
        MonoBehaviour monoBehaviour,
        ref BehaviourTreeBlackboard blackboard
        ){

        /**
        * Loads all actionNode tasks and runs their setup (getting required gameobject components etc.),
        * and updates any nodes that use a blackboard to use blackboard
        */

        LoadRuntimeProperties(
            monoBehaviour:monoBehaviour,
            node:rootNode,
            blackboard:ref blackboard
        );
        
    }
    public void LoadRuntimeProperties(
        MonoBehaviour monoBehaviour,
        Node node,
        ref BehaviourTreeBlackboard blackboard
    ){

        /**
         * Recursively loads properties needed from the gameobject
         */

        if (Node.RequiresBlackboard(node)){
            node.UpdateBlackboard(ref blackboard);
         }

        if (node is ActionNode actionNode){
            actionNode.LoadTask(monoBehaviour);
        }
        else{
            if (node.ChildNodes != null){
                foreach(Node childNode in node.ChildNodes){
                    LoadRuntimeProperties(monoBehaviour, childNode, ref blackboard);
                }
            }
        }

    }

    public void LoadRuntimeProperties(
        MonoBehaviour monoBehaviour,
        Node node
    )
    {

        /**
         * Recursively loads properties needed from the gameobject
         */

        if (node is ActionNode actionNode){
            actionNode.LoadTask(monoBehaviour);
        }
        else{
            if (node.ChildNodes != null){
                foreach(Node childNode in node.ChildNodes){
                    LoadRuntimeProperties(monoBehaviour, childNode);
                }
            }
        }
    }

    public void ResetTree(){
        /**
        * Calls ResetState on all nodes.
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
        if (serializedNodes.Count > 0) {
            int idx = 0;
            rootNode = (PrioritySelector)BehaviourTreeLoader.ReadNodeFromSerializedNodes (
                index:ref idx, 
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
        if (CanSaveTree()){
            serializedNodes = new List<SerializableNode>();
            if (CanSaveGuiData()){
                nodeMetaData = new List<GuiNodeData>();
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
    }

}
}
