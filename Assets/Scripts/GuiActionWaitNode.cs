using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class GuiActionWaitNode : GuiActionNode
{
    /**
    * \class GuiActionWaitNode
    * Displays an ActionNode in the BehaviourTree class using the BehaviourTreeEditor,
    * specifically for the Wait BehaviourTreeTask.
    * Users can specify the wait time and random deviation that are then passed to the Wait task at runtime.
    */

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
        iconAndText = BehaviourTreeProperties.WaitContent();
    }

    public override void DrawDetails()
    {
        /**
         * What details are displayed in the details panel of the BehaviourTreeEditor
         */
        base.DrawDetails();
        float waitTime = actionWaitNode.WaitTime;

        GUILayout.Label("Wait Time");
        EditorGUI.BeginChangeCheck();
        bool success = float.TryParse(GUILayout.TextField(actionWaitNode.WaitTime.ToString(), 5), out waitTime);
        if (success){
            actionWaitNode.WaitTime = waitTime;
        }
        
        float randomDeviation = actionWaitNode.RandomDeviation;
        GUILayout.Label("Random Deviation");
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
            GUI.color = BehaviourTreeProperties.SelectedTint();
        }
        GUI.backgroundColor = color;
        GUI.Box(scaledRect, "", activeStyle);
        GUI.backgroundColor = taskRectColor;
        string s =  "\n" + DisplayName + "\n" + DisplayTask;
        s += " (" + actionWaitNode.WaitTime.ToString() + " +/- ";
        s += actionWaitNode.RandomDeviation.ToString() + " sec)";
        iconAndText.text = s;
        GUI.Box(scaledTaskRect, iconAndText, activeTaskStyle);
        GUI.color = BehaviourTreeProperties.DefaultTint();
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