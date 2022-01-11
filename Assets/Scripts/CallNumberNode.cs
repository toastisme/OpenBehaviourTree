using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class CallNumberNode : IGuiNode
{
    GUIStyle style;
    Rect rect;
    Color color;
    public int CallNumber{get; set;}

    public CallNumberNode(){
        style = NodeProperties.CallNumberStyle();
        color = NodeProperties.CallNumberColor(); 
        Vector2 size = NodeProperties.CallNumberSize();
        rect = new Rect(0, 0, size.x, size.y);
    }

    public void Draw(){
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUI.Box(rect, CallNumber.ToString(), style);
        GUI.backgroundColor = currentColor;
    }

    public void Drag(Vector2 delta){
        rect.position += delta;
    }

    public void SetPosition(Vector2 pos){
        rect.position = pos;
    }

}
}