using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    public ConnectionPoint ChildPoint;
    public ConnectionPoint ParentPoint;
    public Action<Connection> OnClickRemoveConnection;

    public Connection(ConnectionPoint ChildPoint, ConnectionPoint ParentPoint, Action<Connection> OnClickRemoveConnection)
    {
        this.ChildPoint = ChildPoint;
        this.ParentPoint = ParentPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            ChildPoint.rect.center,
            ParentPoint.rect.center,
            ChildPoint.rect.center,
            ParentPoint.rect.center,
            Color.white,
            null,
            2f
        );

        if (Handles.Button((ChildPoint.rect.center + ParentPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}