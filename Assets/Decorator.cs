using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BehaviourBase{
    public class Decorator : Node
    {
        /**
        * \class Decorator
        * Represents a decorator node in the BehaviourTree class.
        * The decorator node stores a boolean (condition), the current value of which
        * dictates the the node's Nodestate.
        */

        Action<Decorator> OnRemoveDecorator;
        bool condition;
        public Decorator(
            ref bool condition,
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
            Action<Decorator> OnRemoveDecorator,
            Node childNode
        ) : base(
            displayTask:displayTask,
            displayName:displayName,
            rect:rect,
            parentNode:parentNode,
            defaultStyle:defaultStyle,
            selectedStyle:selectedStyle,
            callNumberStyle:callNumberStyle,
            color:color,
            callNumberColor:callNumberColor,
            UpdatePanelDetails:UpdatePanelDetails
        ){
            this.OnRemoveDecorator = OnRemoveDecorator;
            this.nodeType = NodeType.Decorator;
            this.childNodes = new List<Node>{childNode};
            this.condition = condition;
        }

        // Node Methods 
        
        public override NodeState Evaluate(){
            nodeState = condition ? childNodes[0].Evaluate() : NodeState.Failed;
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


    }

}