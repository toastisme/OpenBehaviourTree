using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIDecorator : NodeBase
{
    public Action<GUIDecorator> OnRemoveDecorator;
    public GUIDecorator(string task,
                   Vector2 position, 
                   float width, 
                   float height, 
                   GUIStyle nodeStyle, 
                   GUIStyle selectedStyle, 
                   GUIStyle callNumberStyle,
                   Action<NodeBase> UpdatePanelDetails,
                   Action<GUIDecorator> OnClickRemoveDecorator)
    {
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.OnRemoveDecorator = OnClickRemoveDecorator;
        this.style = nodeStyle;
        this.task = task;
        this.rect = new Rect(position[0], position[1], width, height);
        this.callNumberRect = new Rect(position.x, position.y -10, width/6, width/6);
        this.callNumberStyle = callNumberStyle;
    }


    private void RemoveDecorator(){
        if (OnRemoveDecorator != null){
            OnRemoveDecorator(this);
        }
    }

    protected override void ProcessContextMenu(){}

}
