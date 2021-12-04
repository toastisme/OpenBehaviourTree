using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    public ConnectionPoint childPoint;
    private GUINode childNode;
    public ConnectionPoint parentPoint;
    private GUINode parentNode;
    public Action<Connection> OnClickRemoveConnection;

    public Connection(ConnectionPoint childPoint, ConnectionPoint parentPoint, Action<Connection> OnClickRemoveConnection)
    {
        this.childPoint = childPoint;
        childNode = childPoint.GetNode();
        this.parentPoint = parentPoint;
        parentNode = parentPoint.GetNode();
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            childPoint.rect.center,
            parentPoint.rect.center,
            childPoint.rect.center,
            parentPoint.rect.center,
            Color.white,
            null,
            2f
        );

        if (Handles.Button((childPoint.rect.center + parentPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }

    GUINode GetParentNode(){
        return parentNode;
    }
    GUINode GetChildNode(){
        return childNode;
    }
}