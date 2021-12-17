using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIProbabilityWeight : NodeBase
{
    public GUIProbabilityWeight(string task,
                   Vector2 position, 
                   float width, 
                   float height, 
                   GUIStyle nodeStyle,
                   GUIStyle selectedStyle, 
                   Action<NodeBase> UpdatePanelDetails
                   )
    {
        this.style = nodeStyle;
        this.selectedNodeStyle = selectedStyle;
        this.task = task;
        this.name = "";
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.rect = new Rect(position[0], position[1], width, height);
        defaultNodeStyle = nodeStyle;
        this.selectedNodeStyle = selectedStyle;
    }

    public void Move(Vector2 pos)
    {
        rect.position = pos;
    }
    public override bool ProcessEvents(Event e)
    {
        bool guiChanged = false;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        guiChanged = true;
                        SetSelected(true);
                        UpdatePanelDetails(this);
                    }
                    else
                    {
                        guiChanged = true;
                        SetSelected(false);
                    }
                }
                break;
        }

        return guiChanged;
    }

    public override void Drag(Vector2 delta){}

    protected override void ProcessContextMenu(){}

}
