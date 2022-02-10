using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class GuiActionWaitNode : GuiActionNode
{
    ActionWaitNode actionWaitNode;
    string displayTask;
    public GuiActionWaitNode(
        Node node,
        string displayTask,
        string displayName,
        Vector2 pos,
        Connection parentConnection,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action<CompositeGuiNode> OnRemoveNode,
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint,
        ref BehaviourTreeBlackboard blackboard
    ) : base(
        node:node,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        parentConnection:parentConnection,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        OnRemoveNode:OnRemoveNode,
        OnClickChildPoint:OnClickChildPoint,
        OnClickParentPoint:OnClickParentPoint,
        blackboard:ref blackboard
    ){
        actionWaitNode = (ActionWaitNode)node;
        iconAndText = NodeProperties.WaitContent();
    }

    public override void DrawDetails()
    {
        base.DrawDetails();
        float waitTime = actionWaitNode.WaitTime;

        GUILayout.Label("Wait time");
        EditorGUI.BeginChangeCheck();
        bool success = float.TryParse(GUILayout.TextField(actionWaitNode.WaitTime.ToString(), 5), out waitTime);
        if (success){
            actionWaitNode.WaitTime = waitTime;
        }
        
        float randomDeviation = actionWaitNode.RandomDeviation;
        GUILayout.Label("Random deviation");
        success = float.TryParse(GUILayout.TextField(actionWaitNode.RandomDeviation.ToString(), 5), out randomDeviation);
        if (success){
            actionWaitNode.RandomDeviation = randomDeviation;
        }
        if (EditorGUI.EndChangeCheck()){
            TreeModified();
        }
    }
    protected override void DrawSelf()
    {
        if (IsSelected){
            GUI.color = NodeProperties.SelectedTint();
        }
        GUI.backgroundColor = color;
        GUI.Box(apparentRect, "", activeStyle);
        GUI.backgroundColor = taskRectColor;
        string s =  "\n" + DisplayName + "\n" + DisplayTask;
        s += " (" + actionWaitNode.WaitTime.ToString() + " +/- ";
        s += actionWaitNode.RandomDeviation.ToString() + " sec)";
        iconAndText.text = s;
        GUI.Box(apparentTaskRect, iconAndText, activeTaskStyle);
        GUI.color = NodeProperties.DefaultTint();
    }

    protected override void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        Dictionary<string, bool> boolKeys = blackboard.GetBoolKeys();
        if (boolKeys == null || boolKeys.Count == 0){
            genericMenu.AddDisabledItem(new GUIContent("Add blackboard bool keys to use as decorators"));
        }
        else{            
            foreach(string boolName in blackboard.GetBoolKeys().Keys){
                if (!DecoratorKeyActive(boolName)){
                    genericMenu.AddItem(
                        new GUIContent("Add Decorator/" + boolName), 
                        false, 
                        () => OnClickAddDecorator(boolName));
                }
                else{
                    genericMenu.AddDisabledItem(new GUIContent("Add Decorator/" + boolName));
                }
            }
        }
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

}
}