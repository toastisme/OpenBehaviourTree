using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public enum TimerType{
    Timeout,
    Cooldown
}
public class GuiNodeTimer : GuiNode
{
    GuiNode parentGuiNode;
    Action<GuiNodeTimer> OnRemoveTimer;
    NodeTimer nodeTimer;

    string displayTask;

    float defaultTimerVal;
    public override string DisplayTask{
        // What the node actually does
        get{
            return displayTask;
        }
        set{
            displayTask = value;
        }
    }

    public GuiNodeTimer(
        NodeTimer nodeTimer,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action<GuiNodeTimer> OnRemoveTimer,
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
        this.defaultTimerVal = NodeProperties.DefaultTimerVal();
    }

    protected override void ApplyDerivedSettings(){
        defaultStyle = NodeProperties.GUINodeStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = NodeProperties.DecoratorColor();
        rect.size = NodeProperties.SubNodeSize();
        if (displayTask == "Timeout"){
            iconAndText = NodeProperties.TimeoutContent();
        }
        else{
            iconAndText = NodeProperties.CooldownContent();
        }
    }

    private void Remove(){
        if (OnRemoveTimer != null){
            OnRemoveTimer(this);
        }
    }

    public override void Drag(Vector2 delta)
    {
        parentGuiNode.Drag(delta);
    }

    public void DragWithoutParent(Vector2 delta){
        rect.position += delta;
    }


    protected void ProcessContextMenu(){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove timer"), false, Remove);
        genericMenu.ShowAsContext();
    }

    public override bool ProcessEvents(Event e, Vector2 mousePos){
        bool guiChanged = false;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (apparentRect.Contains(mousePos))
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

                if (e.button == 1 && apparentRect.Contains(mousePos))
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
        iconAndText.text = "\n\n" + DisplayTask + " (" + nodeTimer.GetTimerVal() + " sec)";
        GUI.Box(apparentRect, iconAndText, activeStyle);
        GUI.backgroundColor = currentColor;
    }

    public override void DrawDetails()
    {
        GUILayout.Label("Task: " + DisplayTask);
        GUILayout.Label("Value (sec)");
        float timerVal = defaultTimerVal;
        float.TryParse(GUILayout.TextField(nodeTimer.GetTimerVal().ToString(), 50), out timerVal);
        nodeTimer.SetTimerVal(timerVal);
    }
    public void SetEditorActions(
        Action<GuiNode> UpdatePanelDetails,
        Action<GuiNodeTimer> OnRemoveTimer){
            this.UpdatePanelDetails = UpdatePanelDetails;
            this.OnRemoveTimer = OnRemoveTimer;
    }


}
}