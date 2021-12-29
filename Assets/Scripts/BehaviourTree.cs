using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourBase{
    public class BehaviourTree : ScriptableObject 
    {
        public List<AggregateNode> nodes;
        public List<Connection> connections;
        public AggregateNode rootNode;
        public BehaviourTreeBlackboard blackboard;
        public float evaluationDelay = 0.0001f;


        public void LoadTree(){
            /**
             * Loads all actionNode tasks and runs their setup (getting required gameobject components etc.)
             */
            foreach(AggregateNode node in nodes){
                if (node is ActionNode actionNode){
                    actionNode.LoadTask();
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
    }
}
