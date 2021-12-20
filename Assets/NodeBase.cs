using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum NodeType{
    Root,
    SequenceSelector,
    ProbabilitySelector,
    PrioritySelector,
    Action,
    Decorator,
    ProbabilityWeight
}
public class NodeBase 
{
    protected NodeType nodeType;
    public bool isDragged;
    private bool isSelected;
    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    public Action<NodeBase> UpdatePanelDetails;

    protected Rect rect;
    protected string name = "";
    protected string task  = "";
    public string GetName(){return name;}
    public void SetName(string newName){
        name = newName;
    }
    public string GetTask(){return task;}
    public void SetTask(string newTask){
        task = newTask;
    }

    public Rect GetRect(){return rect;} 
    public void SetPosition(Vector2 position){
        this.rect.position = position;
    }

    public virtual bool ProcessEvents(Event e)
    {
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

    protected virtual void ProcessContextMenu(){}

    public virtual void Drag(Vector2 delta)
    {
        rect.position += delta;
    }
    public virtual void Draw()
    {
        GUI.Box(rect, "\n" + name + "\n" + task, style);
    }

    public bool IsSelected(){return isSelected;}

    public void SetSelected(bool selected){
        if (selected){
            isSelected = true;
            style = selectedNodeStyle;
        }
        else{
            isSelected = false;
            style = defaultNodeStyle;
        }
    }

    public NodeType GetNodeType(){return nodeType;}

    public static NodeType GetNodeTypeFromString(string s){
        switch(s){
            case "Root":
                return NodeType.Root;
            case "Sequence":
                return NodeType.SequenceSelector;
            case "Priority Selector":
                return NodeType.PrioritySelector;
            case "Probability Selector":
                return NodeType.ProbabilitySelector;
            case "Decorator":
                return NodeType.Decorator;
            case "Constant weight (1)":
                return NodeType.ProbabilityWeight;
            default:
                return NodeType.Action;
        }

    }

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


    public void SetNodeTypeFromTask(string task){
        nodeType = NodeBase.GetNodeTypeFromString(task);
    }

}
