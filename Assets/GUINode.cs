using System;
using UnityEditor;
using UnityEngine;

public class GUINode
{
    public Rect rect;
    public string name = "NODE";
    public bool isDragged;
    public bool isSelected;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<GUINode> OnRemoveNode;

    public GUINode(string _name,
                   Vector2 position, 
                   float width, 
                   float height, 
                   GUIStyle nodeStyle, 
                   GUIStyle selectedStyle, 
                   GUIStyle inPointStyle, 
                   GUIStyle outPointStyle, 
                   Action<ConnectionPoint> OnClickInPoint, 
                   Action<ConnectionPoint> OnClickOutPoint, 
                   Action<GUINode> OnClickRemoveNode)
    {
        name = _name;
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        defaultNodeStyle.alignment = TextAnchor.MiddleCenter;
        selectedNodeStyle.alignment = TextAnchor.MiddleCenter;
        OnRemoveNode = OnClickRemoveNode;

    }

    public ConnectionPoint GetInPoint(){return inPoint;}
    public ConnectionPoint GetOutPoint(){return outPoint;}

    public string GetName(){return name;}

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        //GUI.color = new Color(255, 0, 0);
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, name, style);
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

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}