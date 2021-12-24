using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BehaviourBase{
    public class Decorator : AggregateNode
    {
        /**
        * \class Decorator
        * Represents a decorator node in the BehaviourTree class.
        * The decorator node refers to a boolean (blackboard.GetBoolKeys()[displayTask]), the current value of which
        * dictates the the node's Nodestate.
        */

        Action<Decorator> OnRemoveDecorator;
        /*
            NodeType nodeType,
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
        */
        public Decorator(
            ref BehaviourTreeBlackboard blackboard,
            string displayTask,
            string displayName,
            Rect rect,
            Node parentNode,
            GUIStyle defaultStyle,
            GUIStyle selectedStyle,
            GUIStyle callNumberStyle,
            Color color,
            Color callNumberColor,
            Action<AggregateNode> UpdatePanelDetails,
            Action<Decorator> OnRemoveDecorator,
            AggregateNode childNode
        ) : base(
            NodeType.Decorator,
            displayTask:displayTask,
            displayName:displayName,
            rect:rect,
            parentNode:parentNode,
            UpdatePanelDetails:UpdatePanelDetails,
            nodeStyles:null,
            nodeColors:null,
            OnClickChildPoint:null,
            OnClickParentPoint:null,
            OnRemoveNode:null,
            blackboard:ref blackboard
        ){
            this.OnRemoveDecorator = OnRemoveDecorator;
            this.childNodes = new List<Node>{childNode};
        }

        // Node Methods 

        public override NodeState Evaluate(){
            nodeState = blackboard.GetBoolKeys()[displayTask] ? childNodes[0].Evaluate() : NodeState.Failed;
            if (nodeState == NodeState.Failed){childNodes[0].ResetState();} 
            return nodeState;
        } 

        // GUINode Methods

        private void Remove(){
            if (OnRemoveDecorator != null){
                OnRemoveDecorator(this);
            }
        }

        public override void Drag(Vector2 delta)
        {
            if (isSelected){
                parentNode.Drag(delta);
            }
            rect.position += delta;
            callNumberRect.position += delta;
        }

        protected override void ProcessContextMenu(){
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove decorator"), false, Remove);
            genericMenu.ShowAsContext();
        }

        public override bool ProcessEvents(Event e){
            return base.ProcessSubNodeEvents(e);
        }


    }

}