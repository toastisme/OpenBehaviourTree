using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace OpenBehaviourTree{
public class GuiActionWaitNode : CompositeGuiNode
{
    /**
    * \class OpenBehaviourTree.GuiActionWaitNode
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
        ApplyDerivedSettings();
        ApplyNodeTypeSettings(OnClickChildPoint,OnClickParentPoint);
        NodeUpdated();
    }

    protected override void ApplyDerivedSettings()
    {
        taskRectColor = BehaviourTreeProperties.ActionColor();
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        defaultTaskStyle = BehaviourTreeProperties.TaskNodeStyle();
        selectedTaskStyle = BehaviourTreeProperties.SelectedTaskNodeStyle();
        activeTaskStyle = defaultTaskStyle;
        activeStyle = defaultStyle;
        color = BehaviourTreeProperties.DefaultColor();
        iconAndText = BehaviourTreeProperties.WaitContent();
    }

    protected override void ApplyNodeTypeSettings(
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){

        ChildPoint = null; // ActionNodes are leaf nodes and so have no children
        ParentPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.Out, 
                                            BehaviourTreeProperties.ParentPointStyle(), 
                                            OnClickParentPoint);

    }

    public override void DrawDetails()
    {
        /**
         * What details are displayed in the details panel of the BehaviourTreeEditor
         */
        base.DrawDetails();
        float waitTime = actionWaitNode.TimerValue;

        GUILayout.Label("Wait Time");
        EditorGUI.BeginChangeCheck();
        bool success = float.TryParse(GUILayout.TextField(actionWaitNode.TimerValue.ToString(), 5), out waitTime);
        if (success){
            actionWaitNode.TimerValue = waitTime;
        }
        
        float randomDeviation = actionWaitNode.RandomDeviation;
        GUILayout.Label("Random Deviation");
        success = float.TryParse(GUILayout.TextField(actionWaitNode.RandomDeviation.ToString(), 5), out randomDeviation);
        if (success){
            actionWaitNode.RandomDeviation = randomDeviation;
        }
        if (EditorGUI.EndChangeCheck()){
            TreeModified();
            NodeUpdated();
        }
    }

    string GetDisplayTaskString(){
        string s = DisplayTask;
        if (actionWaitNode != null){
            s += " (" + actionWaitNode.TimerValue.ToString() + " +/- ";
            s += actionWaitNode.RandomDeviation.ToString() + " sec)";
        }
        return s;
    }
    
    protected override void DrawSelf()
    {
        if (IsSelected){
            GUI.color = BehaviourTreeProperties.SelectedTint();
        }
        string s =  "\n" + DisplayName + "\n" + GetDisplayTaskString();
        GUI.backgroundColor = color;
        GUI.Box(scaledRect, "", activeStyle);
        GUI.backgroundColor = taskRectColor;
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

    public override float GetRequiredBoxWidth(){

        /**
         * A dirty way to approximate the width rect needs to
         * fit the GuiNode text.
         */

        float dx = BehaviourTreeProperties.ApproximateNodeTextWidth();
        return Mathf.Max(GetDisplayTaskString().Length, DisplayName.Length) * dx;
    }

}
}