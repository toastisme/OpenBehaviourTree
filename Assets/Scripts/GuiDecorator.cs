using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class GuiDecorator : CallableGuiNode
{
    GuiNode parentNode;
    Action<GuiDecorator> OnRemoveDecorator;
    Decorator decorator;

    public GuiDecorator(
        Decorator decorator,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action<GuiDecorator> OnRemoveDecorator,
        ref BehaviourTreeBlackboard blackboard,
        GuiNode parentNode
    ) :base(
        node:decorator,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        blackboard: ref blackboard
    )
    {
        this.decorator = decorator;
        this.parentNode = parentNode;
        this.OnRemoveDecorator = OnRemoveDecorator;
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
        if (OnRemoveDecorator != null){
            OnRemoveDecorator(this);
        }
    }

    public override void Drag(Vector2 delta)
    {
        if (IsSelected){
            parentNode.Drag(delta);
        }
        rect.position += delta;
        callNumber.Drag(delta);
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

    public override bool ProcessEvents(Event e){
        return ProcessTaskRectEvents(e);
    }
    public virtual bool ProcessTaskRectEvents(Event e){
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
        string displayTaskAndCondition = DisplayTask;
        if (decorator.invertCondition){displayTaskAndCondition = "!" + displayTaskAndCondition;}
        GUI.Box(rect, "\n" + DisplayName + "\n" + displayTaskAndCondition, activeStyle);
        callNumber.Draw();
        GUI.backgroundColor = currentColor;
    }

    public override void DrawDetails()
    {
        base.DrawDetails();
        decorator.invertCondition = EditorGUILayout.Toggle("Invert condition", 
                                                            decorator.invertCondition);
        
    }
}
}