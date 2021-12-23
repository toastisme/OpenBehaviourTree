using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace BehaviourBase{
    public class SequenceSelector : AggregateNode
    {    
        /**
        * \class SequenceSelector
        * Represents a sequence node in the BehaviourTree class.
        * Child nodes are evaluated in order, and if any fail this node evaluates as failed.
        */

        public SequenceSelector(
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
            nodeType:NodeType.SequenceSelector,
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
            blackboard:ref blackboard
        ){}
        public override NodeState Evaluate() { 
            bool anyChildRunning = false; 
            
            foreach(Node node in childNodes) { 
                switch (node.Evaluate()) { 
                    case NodeState.Idle:
                        nodeState = NodeState.Idle;
                        return nodeState;
                    case NodeState.Failed: 
                        nodeState = NodeState.Failed; 
                        return nodeState;                     
                    case NodeState.Succeeded: 
                        continue; 
                    case NodeState.Running: 
                        ResetOtherStates(node);
                        anyChildRunning = true; 
                        break; 
                    default: 
                        nodeState = NodeState.Succeeded; 
                        return nodeState; 
                } 
            } 
            nodeState = anyChildRunning ? NodeState.Running : NodeState.Succeeded; 
            return nodeState; 
        }
    }

}