using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUINode
{
    public Rect rect;
    public Rect callNumberRect;
    private int callNumber = 0;
    private string name = "";
    private string task  = "";

    public bool isDragged;
    public bool isSelected;

    public ConnectionPoint ChildPoint;
    public ConnectionPoint ParentPoint;
    private List<Connection> childNodes;
    private Connection parentNode;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<GUINode> OnRemoveNode;
    public Action<GUINode> UpdatePanelDetails;

    public GUINode(string task,
                   Vector2 position, 
                   float width, 
                   float height, 
                   GUIStyle nodeStyle, 
                   GUIStyle selectedStyle, 
                   GUIStyle ChildPointStyle, 
                   GUIStyle ParentPointStyle, 
                   Action<GUINode> UpdatePanelDetails,
                   Action<ConnectionPoint> OnClickChildPoint, 
                   Action<ConnectionPoint> OnClickParentPoint, 
                   Action<GUINode> OnClickRemoveNode)
    {
        this.task = task;
        rect = new Rect(position.x, position.y, width, height);
        callNumberRect = new Rect(position.x, position.y -10, width/6, width/6);
        style = nodeStyle;
        ChildPoint = new ConnectionPoint(this, ConnectionPointType.In, ChildPointStyle, OnClickChildPoint);
        ParentPoint = new ConnectionPoint(this, ConnectionPointType.Out, ParentPointStyle, OnClickParentPoint);
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        defaultNodeStyle.alignment = TextAnchor.MiddleCenter;
        selectedNodeStyle.alignment = TextAnchor.MiddleCenter;
        OnRemoveNode = OnClickRemoveNode;
        this.UpdatePanelDetails = UpdatePanelDetails;

        childNodes = new List<Connection>();

    }

    public ConnectionPoint GetChildPoint(){return ChildPoint;}
    public ConnectionPoint GetParentPoint(){return ParentPoint;}

    public string GetName(){return name;}
    public void SetName(string newName){
        name = newName;
    }
    public string GetTask(){return task;}
    public void SetTask(string newTask){
        task = newTask;
    }
    public void Drag(Vector2 delta)
    {
        rect.position += delta;
        callNumberRect.position += delta;
        if (childNodes != null){
            foreach(Connection childNode in childNodes){
                childNode.GetChildNode().Drag(delta);
            }
        }
    }

    public void Draw()
    {
        //GUI.color = new Color(255, 0, 0);
        ChildPoint.Draw();
        ParentPoint.Draw();
        GUI.Box(rect, task + "\n" + name, style);
        GUI.Box(callNumberRect, callNumber.ToString(), style);
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

    public List<Connection> GetChildNodes(){return childNodes;}
    public Connection GetParentNode(){return parentNode;}
    public void SetChildNodes(List<Connection> childNodes){this.childNodes = childNodes;}
    public void AddChildNode(Connection connection){
        this.childNodes.Add(connection);
    }
    public void RemoveChildNode(Connection connection){
        childNodes.Remove(connection);
    }

    public void RefreshChildOrder(){
        /**
         * Orders childNodes by x position
         */
        if (childNodes != null){
            childNodes.Sort((x,y) => x.GetChildNode().GetXPos().CompareTo(y.GetChildNode().GetXPos()));
        }

    }

    public float GetXPos(){
        return rect.x;
    }
    public void RemoveParentNode(){
        this.parentNode = null;
    }
    public void SetParentNode(Connection connection){
        this.parentNode = connection;       
    }

    public void SetCallNumber(int callNumber){
        this.callNumber = callNumber;
    }

    public int GetCallNumber(){return callNumber;}



}