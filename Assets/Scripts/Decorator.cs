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
        bool invertCondition = false;

        public Decorator(
            ref BehaviourTreeBlackboard blackboard,
            string displayTask,
            string displayName,
            Rect rect,
            Node parentNode,
            NodeStyles nodeStyles,
            NodeColors nodeColors,
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
            nodeStyles:nodeStyles,
            nodeColors:nodeColors,
            OnClickChildPoint:null,
            OnClickParentPoint:null,
            OnRemoveNode:null,
            blackboard:ref blackboard
        ){
            this.OnRemoveDecorator = OnRemoveDecorator;
            this.childNodes = new List<Node>{childNode};
            this.callNumberRect.position = rect.position;
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

        public override void Draw()
        {
            Color currentColor = GUI.backgroundColor;
            GUI.backgroundColor = nodeColors.GetColor(nodeType);
            string displayTaskAndCondition = displayTask;
            if (invertCondition){displayTaskAndCondition = "!" + displayTaskAndCondition;}
            if (isSelected){
                GUI.Box(rect, "\n" + displayName + "\n" + displayTaskAndCondition, nodeStyles.selectedGuiNodeStyle);
            }
            else{
                GUI.Box(rect, "\n" + displayName + "\n" + displayTaskAndCondition, nodeStyles.guiNodeStyle);
            }
            GUI.backgroundColor = nodeColors.callNumberColor;
            GUI.Box(callNumberRect, callNumber.ToString(), nodeStyles.callNumberStyle);
            GUI.backgroundColor = currentColor;
        }

        public override void DrawDetails()
        {
            base.DrawDetails();
            invertCondition = EditorGUILayout.Toggle("Invert condition", invertCondition);
            
        }


    }

}