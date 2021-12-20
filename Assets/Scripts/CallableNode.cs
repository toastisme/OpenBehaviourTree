using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallableNode : NodeBase
{
    public GUIStyle callNumberStyle;
    protected Rect callNumberRect;
    protected int callNumber = 1;
    protected Color nodeColor;
    protected Color callNumberColor;
    public Rect GetCallNumberRect(){return callNumberRect;}
    public int GetCallNumber(){return callNumber;}
    public void SetCallNumber(int callNumber){
        this.callNumber = callNumber;
    }
    public override void Drag(Vector2 delta)
    {
        rect.position += delta;
        callNumberRect.position += delta;
    }
    public override void Draw()
    {
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;
        GUI.Box(rect, name + "\n" + task, style);
        GUI.backgroundColor = callNumberColor;
        GUI.Box(callNumberRect, callNumber.ToString(), style);
        GUI.backgroundColor = currentColor;
    }
}
