using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUINode : NodeBase
{
    private List<GUIDecorator> decorators;
    private Vector2 initDecoratorPos = new Vector2(0f,0f);
    private float decoratorHeight = 35f;
    public ConnectionPoint ChildPoint;
    public ConnectionPoint ParentPoint;
    private List<Connection> childNodes;
    private Connection parentNode;
    public Action<GUINode> OnRemoveNode;

    public GUINode(string task,
                   Vector2 position, 
                   float width, 
                   float height, 
                   GUIStyle nodeStyle, 
                   GUIStyle selectedStyle, 
                   GUIStyle ChildPointStyle, 
                   GUIStyle ParentPointStyle, 
                   GUIStyle callNumberStyle, 
                   GUIStyle decoratorStyle, 
                   Action<NodeBase> UpdatePanelDetails,
                   Action<ConnectionPoint> OnClickChildPoint, 
                   Action<ConnectionPoint> OnClickParentPoint, 
                   Action<GUINode> OnClickRemoveNode)
    {
        this.task = task;
        rect = new Rect(position.x, position.y, width, height);
        initDecoratorPos = new Vector2(0, rect.height*.5f);
        callNumberRect = new Rect(position.x, position.y -10, width/6, width/6);
        style = nodeStyle;
        this.callNumberStyle = callNumberStyle;
        this.decoratorStyle = decoratorStyle; 
        ChildPoint = new ConnectionPoint(this, ConnectionPointType.In, ChildPointStyle, OnClickChildPoint);
        ParentPoint = new ConnectionPoint(this, ConnectionPointType.Out, ParentPointStyle, OnClickParentPoint);
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;
        this.UpdatePanelDetails = UpdatePanelDetails;

        childNodes = new List<Connection>();
        decorators = new List<GUIDecorator>();

    }

    public ConnectionPoint GetChildPoint(){return ChildPoint;}
    public ConnectionPoint GetParentPoint(){return ParentPoint;}

    public override void Drag(Vector2 delta)
    {
        rect.position += delta;
        callNumberRect.position += delta;
        if (childNodes != null){
            foreach(Connection childNode in childNodes){
                childNode.GetChildNode().Drag(delta);
            }
        }
        if (decorators != null){
            foreach(GUIDecorator decorator in decorators){
                decorator.SetPosition(decorator.GetRect().position + delta);
            }
        }
    }

    public override void Draw()
    {
        ChildPoint.Draw();
        ParentPoint.Draw();
        GUI.Box(rect, task + "\n" + name, style);
        GUI.Box(callNumberRect, callNumber.ToString(), callNumberStyle);
        foreach (GUIDecorator decorator in decorators){
            decorator.Draw();
        }
    }

    protected override void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.AddItem(new GUIContent("Add Decorator"), false, OnClickAddDecorator);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    private void OnClickRemoveDecorator(GUIDecorator decorator){
        decorators.Remove(decorator);
    }

    private void OnClickAddDecorator(){
        decorators.Add(new GUIDecorator("Decorator",
                              new Vector2(
                                  rect.x + initDecoratorPos[0], rect.y + initDecoratorPos[1]+(decoratorHeight*decorators.Count+1)), 
                              rect.width,
                              decoratorHeight, 
                              decoratorStyle, 
                              decoratorStyle, 
                              UpdatePanelDetails,
                              OnClickRemoveDecorator));
        rect.height += decoratorHeight;
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



}