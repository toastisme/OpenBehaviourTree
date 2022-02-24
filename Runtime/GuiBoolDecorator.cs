using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class GuiBoolDecorator : GuiDecorator
{
    /**
    * \class Behaviour.GuiBoolDecorator
    * Displays a BoolDecorator in the BehaviourTree class, using the BehaviourTreeEditor.
    */

    BoolDecorator decorator;
    
    public  GuiBoolDecorator(
        Node decorator,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action NodeUpdated,
        Action<GuiDecorator> OnRemoveDecorator,
        ref BehaviourTreeBlackboard blackboard,
        GuiNode parentGuiNode
    ) : base(
        decorator:decorator,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        NodeUpdated:NodeUpdated,
        OnRemoveDecorator:OnRemoveDecorator,
        blackboard: ref blackboard,
        parentGuiNode:parentGuiNode
    ){
        this.decorator = (BoolDecorator)decorator;
        ApplyDerivedSettings();
    }

    protected override void ApplyDerivedSettings()
    {
        base.ApplyDerivedSettings();
        iconAndText = BehaviourTreeProperties.BoolDecoratorContent();
    }

    public override void Draw()
    {
        if (IsSelected){
            GUI.color = BehaviourTreeProperties.SelectedTint();
        }
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        string displayTaskAndCondition = DisplayTask;
        if (decorator.invertCondition){displayTaskAndCondition = "!" + displayTaskAndCondition;}
        iconAndText.text ="\n" + DisplayName + "\n" + displayTaskAndCondition;
        GUI.Box(scaledRect, iconAndText, activeStyle);
        callNumber.Draw();
        GUI.backgroundColor = currentColor;
        GUI.color = BehaviourTreeProperties.DefaultTint();
    }

    public override void DrawDetails()
    {
        /**
         * What details are displayed in the details panel of the BehaviourTreeEditor
         */
        GUILayout.Label(BehaviourTreeProperties.GetDefaultStringFromNodeType(GetNodeType()));
        GUILayout.Label("Task: " + DisplayTask);
        GUILayout.Label("Name");
        EditorGUI.BeginChangeCheck();
        DisplayName = GUILayout.TextField(DisplayName, 50);
        if (EditorGUI.EndChangeCheck()){
            TreeModified();
            NodeUpdated();
        }
        bool currentCondition = decorator.invertCondition;
        EditorGUI.BeginChangeCheck();
        decorator.invertCondition = EditorGUILayout.Toggle("Invert condition", 
                                                            decorator.invertCondition);
        if (EditorGUI.EndChangeCheck()){
            TreeModified();
        }
        
    }

    

}
}