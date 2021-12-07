using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeBase 
{
    public bool isDragged;
    public bool isSelected;
    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    public GUIStyle callNumberStyle;
    public GUIStyle decoratorStyle;
    public Action<NodeBase> UpdatePanelDetails;

    protected Rect rect;
    protected Rect callNumberRect;
    protected int callNumber = 0;
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
    public Rect GetCallNumberRect(){return callNumberRect;}
    public int GetCallNumber(){return callNumber;}
    public void SetCallNumber(int callNumber){
        this.callNumber = callNumber;
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                        UpdatePanelDetails(this);
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
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

        return false;
    }

    protected virtual void ProcessContextMenu(){}

    public virtual void Drag(Vector2 delta)
    {
        rect.position += delta;
    }
    public virtual void Draw()
    {
        GUI.Box(rect, task + "\n" + name, style);
        GUI.Box(callNumberRect, callNumber.ToString(), style);
    }

}
