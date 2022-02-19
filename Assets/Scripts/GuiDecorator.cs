using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class GuiDecorator : CallableGuiNode
{

    /**
    * \class GuiDecorator
    * Displays a Decorator in the BehaviourTree class, using the BehaviourTreeEditor.
    */

    GuiNode parentGuiNode;
    Action<GuiDecorator> OnRemoveDecorator;

    public GuiDecorator(
        Node decorator,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action<GuiDecorator> OnRemoveDecorator,
        ref BehaviourTreeBlackboard blackboard,
        GuiNode parentGuiNode
    ) :base(
        node:decorator,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        blackboard: ref blackboard
    )
    {
        this.parentGuiNode = parentGuiNode;
        this.OnRemoveDecorator = OnRemoveDecorator;
        ApplyDerivedSettings();
    }

    public void SetParentGuiNode(GuiNode node){
        this.parentGuiNode = node;
    }

    public void SetEditorActions(
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action<GuiDecorator> OnRemoveDecorator){

            /**
            * Allows for decoupling editor actions being set from the 
            * constructor (required e.g when loading from disk in BehaviourTreeLoader)
            */ 

            this.UpdatePanelDetails = UpdatePanelDetails;
            this.TreeModified = TreeModified;
            this.OnRemoveDecorator = OnRemoveDecorator;
    }

    protected override void ApplyDerivedSettings(){
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = BehaviourTreeProperties.DecoratorColor();
        rect.size = BehaviourTreeProperties.SubNodeSize();
    }

    protected void Remove(){
        if (OnRemoveDecorator != null){
            OnRemoveDecorator(this);
        }
    }

    public override void Drag(Vector2 delta)
    {
        parentGuiNode.Drag(delta);
    }

    public void DragWithoutParent(Vector2 delta){
        rect.position += delta;
        callNumber.Drag(delta);
    }

    protected void ProcessContextMenu(){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove decorator"), false, Remove);
        genericMenu.ShowAsContext();
    }

    public override bool ProcessEvents(Event e, Vector2 mousePos){
        return ProcessTaskRectEvents(e, mousePos);
    }
    public virtual bool ProcessTaskRectEvents(Event e, Vector2 mousePos){
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

}
}