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
    /**
    * \class GuiNodeTimer
    * Class for displaying a NodeTimer in the BehaviourTree class, using the BehaviourTreeEditor.
    */

    GuiNode parentGuiNode;
    Action<GuiNodeTimer> OnRemoveTimer;
    NodeTimer nodeTimer; // The NodeTimer being displayed

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
        Action TreeModified,
        Action<GuiNodeTimer> OnRemoveTimer,
        ref BehaviourTreeBlackboard blackboard,
        GuiNode parentGuiNode
    ) :base(
        node:null,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        blackboard: ref blackboard
    ){
        this.nodeTimer = nodeTimer;
        this.parentGuiNode = parentGuiNode;
        this.OnRemoveTimer = OnRemoveTimer;
        ApplyDerivedSettings();
        this.defaultTimerVal = BehaviourTreeProperties.DefaultTimerVal();
    }

    protected override void ApplyDerivedSettings(){
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = BehaviourTreeProperties.DecoratorColor();
        rect.size = BehaviourTreeProperties.SubNodeSize();
        if (displayTask == "Timeout"){
            iconAndText = BehaviourTreeProperties.TimeoutContent();
        }
        else{
            iconAndText = BehaviourTreeProperties.CooldownContent();
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
                    if (scaledRect.Contains(mousePos))
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

                if (e.button == 1 && scaledRect.Contains(mousePos))
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
        if (IsSelected){
            GUI.color = BehaviourTreeProperties.SelectedTint();
        }
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        iconAndText.text = "\n\n" + DisplayTask + " (" + nodeTimer.GetTimerVal() + " sec)";
        GUI.Box(scaledRect, iconAndText, activeStyle);
        GUI.backgroundColor = currentColor;
        GUI.color = BehaviourTreeProperties.DefaultTint();
    }

    public override void DrawDetails()
    {
        /**
         * What details are displayed in the details panel of the BehaviourTreeEditor
         */

        GUILayout.Label("Task: " + DisplayTask);
        GUILayout.Label("Value (sec)");
        float timerVal = defaultTimerVal;
        EditorGUI.BeginChangeCheck();
        bool success = float.TryParse(GUILayout.TextField(nodeTimer.GetTimerVal().ToString(), 50), out timerVal);
        if (success){
            nodeTimer.SetTimerVal(timerVal);
        }
        if (EditorGUI.EndChangeCheck()){
            TreeModified();
        }
    }
    public void SetEditorActions(

        /**
         * Allows for decoupling editor actions being set from the 
         * constructor (required e.g when loading from disk in BehaviourTreeLoader)
         */ 

        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action<GuiNodeTimer> OnRemoveTimer){
            this.UpdatePanelDetails = UpdatePanelDetails;
            this.TreeModified = TreeModified;
            this.OnRemoveTimer = OnRemoveTimer;
    }


}
}