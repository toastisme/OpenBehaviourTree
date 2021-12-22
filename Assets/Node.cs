using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public abstract class Node : INode, IGUINode
{
    public NodeType nodeType {get; protected set;}

    //// Node variables
    public NodeState nodeState {get; protected set;}
    private List<Node> childNodes;
    private Node parentNode;

    //// GUI varibles
    protected Rect rect;
    protected Color color;
    protected bool isDragged;
    protected bool isSelected;
    protected GUIStyle activeStyle;
    protected GUIStyle defaultStyle;
    protected GUIStyle selectedStyle;
    protected Action<Node> UpdatePanelDetails;

    // Call number
    protected GUIStyle callNumberStyle;
    protected Rect callNumberRect;
    protected int callNumber = 1;
    protected Color callNumberColor;
    public string displayName{get; set;} 
    public string displayTask{get; set;}

    // Node methods
    public virtual NodeState Evaluate(){return nodeState;}
    public void ResetState(){
        nodeState = NodeState.Idle;
        foreach(Node childNode in childNodes){
            childNode.ResetState();
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
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        guiChanged = true;
                        SetSelected(true);
                        UpdatePanelDetails(this);
                    }
                    else
                    {
                        guiChanged = true;
                        SetSelected(false);
                    }
                }

                if (e.button == 1 && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
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

}
