using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIDecorator : CallableNode
{
    public Action<GUIDecorator> OnRemoveDecorator;
    GUINode parentNode;
    public GUIDecorator(string task,
                   Vector2 position, 
                   float width, 
                   float height, 
                   GUINode parentNode,
                   GUIStyle nodeStyle,
                   GUIStyle selectedStyle, 
                   GUIStyle callNumberStyle,
                   Action<NodeBase> UpdatePanelDetails,
                   Action<GUIDecorator> OnClickRemoveDecorator)
    {
        SetNodeTypeFromTask(task);
        this.parentNode = parentNode;
        this.style = nodeStyle;
        this.selectedNodeStyle = selectedStyle;
        this.task = task;
        this.name = "";
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.OnRemoveDecorator = OnClickRemoveDecorator;
        this.rect = new Rect(position[0], position[1], width, height);
        this.callNumberRect = new Rect(position.x, position.y, width/6, width/6);
        this.callNumberStyle = callNumberStyle;
        defaultNodeStyle = nodeStyle;
        this.selectedNodeStyle = selectedStyle;
    }


    private void RemoveDecorator(){
        if (OnRemoveDecorator != null){
            OnRemoveDecorator(this);
        }
    }

    public override void Drag(Vector2 delta)
    {
        if (IsSelected()){
            parentNode.Drag(delta);
        }
        rect.position += delta;
        callNumberRect.position += delta;
    }

    protected override void ProcessContextMenu(){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove decorator"), false, RemoveDecorator);
        genericMenu.ShowAsContext();
    }

}
