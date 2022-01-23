using System.Collections.Generic;
using System;
using System.Linq;

namespace Behaviour{
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
    private Node SelectNode(){

        /**
        * Selects a node randomly, weighted by nodeWeights
        */

        List<float> nodeWeights = GetWeights();
        float weightTotal = nodeWeights.Sum();
        float randomVal = UnityEngine.Random.value;
        float currentProb = 0f;
        for(int i=0; i< ChildNodes.Count; i++){
            currentProb += nodeWeights[i]/weightTotal;
            if (randomVal <= currentProb){
                // Get the node attached to the ProbabilityWeight node
                return ChildNodes[i].ChildNodes[0];
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
        if (CooldownActive()){
            CurrentState = NodeState.Failed;
            return CurrentState;
        }

        if (!TimeoutActive() && !TimeoutExceeded()){
            StartTimeout();
        }

        if (selectedNode == null){
            selectedNode = SelectNode();
        }
        CurrentState = selectedNode.Evaluate();
        if (CurrentState == NodeState.Failed || CurrentState == NodeState.Succeeded){
            selectedNode = null;
        }
        if (TimeoutExceeded()){
            ResetChildStates();
            CurrentState = NodeState.Failed;
            ResetTimeout();
        }

        if (CurrentState == NodeState.Succeeded){
            StartCooldown();
        }

        return CurrentState;
    }

    List<float> GetWeights(){
        List<float> weights = new List<float>();
        foreach(Node childNode in ChildNodes){
            weights.Add(blackboard.GetWeightValue(childNode.TaskName));
        }
        return weights;
    }
    public override NodeType GetNodeType(){return NodeType.ProbabilitySelector;}
    public override void UpdateBlackboard(ref BehaviourTreeBlackboard blackboard){
        this.blackboard = blackboard;
    }
}
}