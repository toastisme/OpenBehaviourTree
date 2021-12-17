using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallableNode : NodeBase
{
    public GUIStyle callNumberStyle;
    protected Rect callNumberRect;
    protected int callNumber = 0;
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
        GUI.Box(rect, name + "\n" + task, style);
        GUI.Box(callNumberRect, callNumber.ToString(), style);
    }
}
