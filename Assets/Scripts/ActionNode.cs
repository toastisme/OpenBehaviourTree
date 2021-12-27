
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace BehaviourBase{
    public class ActionNode : AggregateNode
    {    
        /**
        * \class ActionNode
        * Represents an action node in the BehaviourTree class.
        */

        public ActionNode(
            string displayTask,
            string displayName,
            Rect rect,
            AggregateNode parentNode,
            Action<AggregateNode> UpdatePanelDetails,
            NodeStyles nodeStyles,
            NodeColors nodeColors,
            Action<ConnectionPoint> OnClickChildPoint,
            Action<ConnectionPoint> OnClickParentPoint,
            Action<AggregateNode> OnRemoveNode,
            ref BehaviourTreeBlackboard blackboard
        ) :base(
            nodeType:NodeType.Action,
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
            blackboard:ref blackboard
        ){}
        public override NodeState Evaluate() { 
            return nodeState; 
        }
    }

}