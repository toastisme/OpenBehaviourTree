using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class GuiNodeTimer : GuiNode
{
    GuiNode parentGuiNode;
    Action OnRemoveTimer;
    NodeTimer nodeTimer;

    public GuiNodeTimer(
        NodeTimer nodeTimer,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action OnRemoveTimer,
        ref BehaviourTreeBlackboard blackboard,
        GuiNode parentGuiNode
    ) :base(
        node:null,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        blackboard: ref blackboard
    ){
        this.nodeTimer = nodeTimer;
        this.parentGuiNode = parentGuiNode;
        this.OnRemoveTimer = OnRemoveTimer;
        ApplyDerivedSettings();
    }

    protected override void ApplyDerivedSettings(){
        defaultStyle = NodeProperties.GUINodeStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = NodeProperties.DecoratorColor();
        rect.size = NodeProperties.SubNodeSize();
    }

    private void Remove(){
        if (OnRemoveTimer != null){
            OnRemoveTimer();
        }
    }

    public override void Drag(Vector2 delta)
    {
        if (IsSelected){
            parentGuiNode.Drag(delta);
        }
        rect.position += delta;
    }


    protected void ProcessContextMenu(){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove timer"), false, Remove);
        genericMenu.ShowAsContext();
    }

    public override bool ProcessEvents(Event e){
        bool guiChanged = false;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
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

                if (e.button == 1 && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return guiChanged;
    }

    public override void Draw()
    {
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUI.Box(rect, "\n" + DisplayName + "\n" + DisplayTask, activeStyle);
        GUI.backgroundColor = currentColor;
    }

    public override void DrawDetails()
    {
        base.DrawDetails();
        
    }

}
}