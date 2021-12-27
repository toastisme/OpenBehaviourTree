using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BehaviourBase{
    public abstract class Node : INode, IGUINode
    {
        public NodeType nodeType {get; protected set;}

        //// Node variables
        public NodeState nodeState {get; protected set;}
        protected List<Node> childNodes;
        protected Node parentNode;

        //// GUI varibles
        protected Rect rect;
        protected Color color;
        protected bool isDragged;
        public bool isSelected{get; protected set;}
        protected GUIStyle activeStyle;
        protected GUIStyle defaultStyle;
        protected GUIStyle selectedStyle;

        // Call number
        protected GUIStyle callNumberStyle;
        protected Rect callNumberRect;
        protected int callNumber = 1;
        protected Color callNumberColor;
        public string displayName{get; set;} 
        public string displayTask;

        // Constructor
        protected Node(
            NodeType nodeType,
            string displayTask,
            string displayName,
            Rect rect,
            Node parentNode,
            GUIStyle defaultStyle,
            GUIStyle selectedStyle,
            GUIStyle callNumberStyle,
            Color color,
            Color callNumberColor
        )
        {
            this.nodeType = nodeType;
            this.displayTask = displayTask;
            this.displayName = displayName;
            this.rect = rect;
            this.callNumberRect = new Rect(rect.x, 
                                           rect.y, 
                                           rect.width/6f,
                                           rect.width/6f);
            this.parentNode = parentNode;
            this.defaultStyle = defaultStyle;
            this.selectedStyle = selectedStyle;
            this.callNumberStyle = callNumberStyle;
            this.color = color;
            this.callNumberColor = callNumberColor;
            this.nodeState = NodeState.Idle;
        }

        // Node methods
        public virtual NodeState Evaluate(){return nodeState;}
        public void ResetState(){
            nodeState = NodeState.Idle;
            foreach(Node childNode in childNodes){
                childNode.ResetState();
            }
        }
        public void ResetOtherStates(Node exceptionNode){
            foreach(Node childNode in childNodes){
                if (childNode != exceptionNode){
                    childNode.ResetState();
                }
            }
        }
        public List<Node> GetChildNodes(){return childNodes;}
        public Node GetParentNode(){return parentNode;}
        public Node GetRunningLeafNode(){
            if (nodeState != NodeState.Running){
                foreach (Node childNode in childNodes){
                    Node runningLeafNode = childNode.GetRunningLeafNode();
                    if (runningLeafNode != null){
                        return runningLeafNode;
                    } 
                }
            }
            return null;
        }

        // GUINode methods
        public virtual void SetSelected(bool selected){
            if (selected){
                isSelected = true;
                activeStyle = selectedStyle;
            }
            else{
                isSelected = false;
                activeStyle = defaultStyle;
            }
        }

        public void SetCallNumber(int callNumber){
            this.callNumber = callNumber;
        }

        public virtual bool ProcessEvents(Event e){
            bool guiChanged = false;
            return guiChanged;
        }

        public virtual void Drag(Vector2 delta)
        {
            rect.position += delta;
            callNumberRect.position += delta;
        }
        public virtual void Draw()
        {
            Color currentColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(rect, displayName + "\n" + displayTask, activeStyle);
            GUI.backgroundColor = callNumberColor;
            GUI.Box(callNumberRect, callNumber.ToString(), callNumberStyle);
            GUI.backgroundColor = currentColor;
        }
        protected virtual void ProcessContextMenu(){}

        public Rect GetRect(){
            return rect;
        }

        // Misc methods

        public static string GetDefaultStringFromNodeType(NodeType nodeType){
            switch(nodeType){
                case NodeType.Root:
                    return "Root";
                case NodeType.SequenceSelector:
                    return "Sequence Selector";
                case NodeType.PrioritySelector:
                    return "Priority Selector";
                case NodeType.ProbabilitySelector:
                    return "Probability Selector";
                case NodeType.Decorator:
                    return "Decorator";
                case NodeType.ProbabilityWeight:
                    return "Constant weight (1)";
                default:
                    return "Action";
            }
        }

        public bool IsRootNode(){
            return (nodeType == NodeType.Root);
        }

        public void SetPosition(Vector2 pos){
            rect.position = pos;
        }

    }
}
