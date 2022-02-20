using System;
using UnityEngine;

namespace Behaviour{
public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    /**
     * \class ConnectionPoint
     * Stores and displays a connection point attached to a CompositeGuiNode.
     */
    private Rect rect;
    private Rect scaledRect;
    private Rect nodeRect; // CompositeGuiNode Rect that self is attached to 

    public ConnectionPointType type;

    public CompositeGuiNode node;

    public GUIStyle style;
    GUIContent guiContent;

    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(CompositeGuiNode node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        nodeRect = node.GetRect();
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, nodeRect.width -12f, 20f);
        scaledRect = new Rect(0, 0, nodeRect.width -12f, 20f);
        if (this.type == ConnectionPointType.In){
            guiContent = BehaviourTreeProperties.ParentConnectionPointContent();
        }
        else{
            guiContent = BehaviourTreeProperties.ChildConnectionPointContent();
        }

    }

    public CompositeGuiNode GetNode(){return node;}

    public Rect GetRect(){
        return rect;
    }

    public Rect GetScaledRect(){
        return scaledRect;
    }

    public void UpdateOrigin(Vector2 origin){
        scaledRect = new Rect(rect.x, rect.y, rect.width, rect.height);
        scaledRect.position -= origin;
    }


    public void Draw()
    {
        nodeRect = node.GetRect();
        rect.x = nodeRect.x + (nodeRect.width * 0.5f) - rect.width * 0.5f;

        switch (type)
        {
            case ConnectionPointType.In:
            rect.y = nodeRect.y + nodeRect.height - rect.height * .5f;
            break;

            case ConnectionPointType.Out:
            rect.y = nodeRect.y - rect.height*.5f;
            break;
        }

        if (GUI.Button(scaledRect, guiContent, style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }

    public void UpdateBoxWidth(float newWidth){
        rect.width = newWidth -12f;
    }
}
}