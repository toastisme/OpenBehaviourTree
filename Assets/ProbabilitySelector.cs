

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;

namespace BehaviourBase{
    public class ProbabilitySelector : AggregateNode
    {    
        /**
        * \class ProbabilitySelector
        * Represents a probability node in the BehaviourTree class.
        * A Child node is selected at random, and if it returns succeed this node evaluates as succeeded.
        * Selection is based off of weights obtained from ProbabilityWeights attached to each
        child connection.
        */

        private Node selectedNode;

        public ProbabilitySelector(
            string displayTask,
            string displayName,
            Rect rect,
            Node parentNode,
            GUIStyle defaultStyle,
            GUIStyle selectedStyle,
            GUIStyle callNumberStyle,
            Color color,
            Color callNumberColor,
            Action<Node> UpdatePanelDetails,
            NodeStyles nodeStyles,
            NodeColors nodeColors,
            Action<ConnectionPoint> OnClickChildPoint,
            Action<ConnectionPoint> OnClickParentPoint,
            Action<AggregateNode> OnRemoveNode,
            ref BehaviourTreeBlackboard blackboard
        ) :base(
            nodeType:NodeType.ProbabilitySelector,
            displayTask:displayTask,
            displayName:displayName,
            rect:rect,
            parentNode:parentNode,
            defaultStyle:defaultStyle,
            selectedStyle:selectedStyle,
            callNumberStyle:callNumberStyle,
            color:color,
            callNumberColor:callNumberColor,
            UpdatePanelDetails:UpdatePanelDetails,
            nodeStyles:nodeStyles,
            nodeColors:nodeColors,
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint,
            OnRemoveNode:OnRemoveNode,
            blackboard: ref blackboard
        ){
            selectedNode = null;
        }
        private Node SelectNode(List<Node> nodes, List<float> nodeWeights){

            /**
            * Selects a node randomly, weighted by nodeWeights
            */

            float weightTotal = nodeWeights.Sum();
            float randomVal = UnityEngine.Random.value;
            float currentProb = 0f;
            for(int i=0; i< nodes.Count; i++){
                currentProb += nodeWeights[i]/weightTotal;
                if (randomVal <= currentProb){
                    return nodes[i];
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
                List<float> weights = GetWeights();
                selectedNode = SelectNode(childNodes, weights);
            }
            nodeState = selectedNode.Evaluate();
            if (selectedNode.Evaluate() == NodeState.Failed || selectedNode.Evaluate() == NodeState.Succeeded){
                selectedNode = null;
            }
            return nodeState;
        }

        public List<float> GetWeights(){
            List<float> weights = new List<float>();
            foreach(Connection connection in childConnections){
                string weightKey = connection.GetProbabilityWeightKey();
                weights.Add(blackboard.GetWeightValue(weightKey));
            }
            return weights;
        }
    }


}