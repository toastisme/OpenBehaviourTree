using System.Collections.Generic;
using System;
using System.Linq;

namespace BehaviourTree{
public class ProbabilitySelector : Node
{    
    /**
    * \class ProbabilitySelector
    * Represents a probability node in the BehaviourTree class.
    * A Child node is selected at random, and if it returns succeed this node evaluates as succeeded.
    * Selection is based off of weights obtained from ProbabilityWeights attached to each
    child connection.
    */

    private Node selectedNode;
    private BehaviourTreeBlackboard blackboard;

    public ProbabilitySelector(
        string taskName,
        ref BehaviourTreeBlackboard blackboard,
        Node parentNode=null
    ) :base(
        taskName:taskName,
        parentNode:parentNode
    ){
        selectedNode = null;
        this.blackboard = blackboard;
    }
    private Node SelectNode(List<float> nodeWeights){

        /**
        * Selects a node randomly, weighted by nodeWeights
        */

        List<float> nodeWeights = GetWeights();
        float weightTotal = nodeWeights.Sum();
        float randomVal = System.Random.value;
        float currentProb = 0f;
        for(int i=0; i< nodes.Count; i++){
            currentProb += nodeWeights[i]/weightTotal;
            if (randomVal <= currentProb){
                // Get the node attached to the ProbabilityWeight node
                return nodes[i].ChildNodes[0];
            }
        }
        return null;
    }

    public override NodeState Evaluate(){

        /**
        * If selectedNode == null, UpdateWeights is called and 
        * a node is selected random, weighted by childNodeWeights (SelectNode).
        * State is then set to selectedNode.Evaluate().
        * If selectedNode.Evaluate is FAILURE or SUCCESS, 
        * selectedNode is set to null.
        */

        if (selectedNode == null){
            selectedNode = SelectNode();
        }
        NodeState = selectedNode.Evaluate();
        if (selectedNode.Evaluate() == NodeState.Failed || selectedNode.Evaluate() == NodeState.Succeeded){
            selectedNode = null;
        }
        return NodeState;
    }

    List<float> GetWeights(){
        List<float> weights = new List<float>();
        foreach(Node childNode in ChildNodes){
            weights.Add(blackboard.GetWeightValue(childNode.NodeTask));
        }
        return weights;
    }
    public override NodeType GetNodeType(){return NodeType.ProbabilitySelector;}
}
}