using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class CallNumberNode : IGuiNode
{

    /**
     * \class Behaviour.CallNumberNode
     * Class to hold and display the call number of a given GUINode
     * in a BehaviourTree.
     */
    GUIStyle style;
    Rect rect;
    Rect scaledRect;
    Color color;
    public int CallNumber{get; set;}

    public CallNumberNode(){
        style = BehaviourTreeProperties.CallNumberStyle();
        color = BehaviourTreeProperties.CallNumberColor(); 
        Vector2 size = BehaviourTreeProperties.CallNumberSize();
        rect = new Rect(0, 0, size.x, size.y);
        scaledRect = new Rect(0, 0, size.x, size.y);
        CallNumber = -1;
    }

    public void Draw(){
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUI.Box(scaledRect, CallNumber.ToString(), style);
        GUI.backgroundColor = currentColor;
    }

    public void Drag(Vector2 delta){
        rect.position += delta;
    }

    public void SetPosition(Vector2 pos){
        rect.position = pos;
    }

    public void UpdateOrigin(Vector2 origin){
        scaledRect = new Rect(rect.x, rect.y, rect.width, rect.height);
        scaledRect.position -= origin;
    }

    public Rect GetRect(){return rect;}

}
}