
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace BehaviourBase{
    public class PrioritySelector : AggregateNode
    {    
        /**
        * \class PrioritySelector
        * Represents a priority node in the BehaviourTree class.
        * Child nodes are evaluated in order, and if any succeed this node evaluates as succeeded.
        * Returns failed if all child nodes return failed.
        */

        public PrioritySelector(
            string displayTask,
            string displayName,
            Rect rect,
            Node parentNode,
            Action<AggregateNode> UpdatePanelDetails,
            NodeStyles nodeStyles,
            NodeColors nodeColors,
            Action<ConnectionPoint> OnClickChildPoint,
            Action<ConnectionPoint> OnClickParentPoint,
            Action<AggregateNode> OnRemoveNode,
            ref BehaviourTreeBlackboard blackboard
        ) :base(
            nodeType:NodeType.PrioritySelector,
            displayTask:displayTask,
            displayName:displayName,
            rect:rect,
            parentNode:parentNode,
            UpdatePanelDetails:UpdatePanelDetails,
            nodeStyles:nodeStyles,
            nodeColors:nodeColors,
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint,
            OnRemoveNode:OnRemoveNode,
            blackboard: ref blackboard
        ){}
        public override NodeState Evaluate() { 
            foreach (Node node in childNodes){
                switch(node.Evaluate()){
                    case NodeState.Idle:
                        nodeState = NodeState.Idle;
                        return nodeState;
                    case NodeState.Failed:
                        continue;
                    case NodeState.Succeeded:
                        nodeState = NodeState.Succeeded;
                        return nodeState;
                    case NodeState.Running:
                        nodeState = NodeState.Running;
                        ResetOtherStates(node);
                        return nodeState;
                    default:
                        continue;
                }
            }
            nodeState = NodeState.Failed;
            return nodeState;
        }
    }

}