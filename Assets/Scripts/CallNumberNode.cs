using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class CallNumberNode : IGuiNode
{
    GUIStyle style;
    Rect rect;
    Rect apparentRect;
    Color color;
    public int CallNumber{get; set;}

    public CallNumberNode(){
        style = NodeProperties.CallNumberStyle();
        color = NodeProperties.CallNumberColor(); 
        Vector2 size = NodeProperties.CallNumberSize();
        rect = new Rect(0, 0, size.x, size.y);
        apparentRect = new Rect(0, 0, size.x, size.y);
    }

    public void Draw(){
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUI.Box(apparentRect, CallNumber.ToString(), style);
        GUI.backgroundColor = currentColor;
    }

    public void Drag(Vector2 delta){
        rect.position += delta;
    }

    public void SetPosition(Vector2 pos){
        rect.position = pos;
    }

    public void UpdateOrigin(Vector2 origin){
        apparentRect = new Rect(rect.x, rect.y, rect.width, rect.height);
        apparentRect.position -= origin;
    }

    public Rect GetRect(){return rect;}

}
}