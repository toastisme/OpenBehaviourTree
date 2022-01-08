using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public class CallNumberNode : IGuiNode
{
    GUIStyle style;
    public Rect NodeRect{get; set;}
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
        GUI.Box(NodeRect, CallNumber.ToString(), style);
        GUI.backgroundColor = currentColor;
    }

    public void Drag(Vector2 delta){
        NodeRect.position += delta;
    }
    /** 
     * Finish GuiNode
     * Update BehaviourTreeEditor
     * Blackboard saving/loading 
     */

}
}