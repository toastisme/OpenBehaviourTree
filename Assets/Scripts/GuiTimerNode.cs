using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class GuiTimerNode : GuiDecorator
{
    TimerNode timerNode;
    public GuiTimerNode(
        Node timerNode,
        string displayName,
        string displayTask,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action<GuiDecorator> OnRemoveDecorator,
        ref BehaviourTreeBlackboard blackboard,
        GuiNode parentGuiNode
    ) :base(
        decorator:timerNode,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        OnRemoveDecorator:OnRemoveDecorator,
        blackboard: ref blackboard,
        parentGuiNode: parentGuiNode
    )
    {
        timerNode = (TimerNode)timerNode;
    }

    public override void Draw()
    {
        if (IsSelected){
            GUI.color = BehaviourTreeProperties.SelectedTint();
        }
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        iconAndText.text = "\n\n" + DisplayTask + " (" + timerNode.AsString() + ")";
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
        
        GUILayout.Label("Task: " + DisplayTask);
        float timerVal;
        float randomDeviationVal;
        bool success;

        GUILayout.Label("Value (sec)");
        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginDisabledGroup((timerNode.valueKey != ""));
        success = float.TryParse(GUILayout.TextField(timerNode.TimerValue.ToString(), 50), out timerVal);
        if (success){
            timerNode.TimerValue = timerVal;
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Label("Blackboard Key");
        string timeoutButtonName = timerNode.valueKey == "" ? "None" : timerNode.valueKey;
        if (GUILayout.Button(timeoutButtonName, GUILayout.MinWidth(175), GUILayout.MaxWidth(175))){
            ProcessContextMenu(SetValueKey);
        }

        GUILayout.Label("Random Deviation (sec)");
        EditorGUI.BeginDisabledGroup((timerNode.randomDeviationKey != ""));
        success = float.TryParse(
            GUILayout.TextField(timerNode.RandomDeviation.ToString(), 50),
             out randomDeviationVal
             );
        if (success){
            timerNode.RandomDeviation = randomDeviationVal;
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Label("Blackboard Key");
        string deviationButtonName = timerNode.randomDeviationKey == "" ? "None" : timerNode.randomDeviationKey;
        if (GUILayout.Button(deviationButtonName, GUILayout.MinWidth(175), GUILayout.MaxWidth(175))){
            ProcessContextMenu(SetRandomDeviationKey);
        }


        if (EditorGUI.EndChangeCheck()){
            TreeModified();
        }
    }

    protected void ProcessContextMenu(Action<string, float> FieldUpdater){
        GenericMenu genericMenu = new GenericMenu();
        if (blackboard == null){
            genericMenu.AddDisabledItem(
                new GUIContent("Add blackboard asset to the behaviour tree to use variables as weights"));
            genericMenu.ShowAsContext();
            return;
        }
        Dictionary<string, int> intKeys = blackboard.GetIntKeys();
        Dictionary<string, float> floatKeys = blackboard.GetFloatKeys();
        if(NoCustomKeys(intKeys, floatKeys)){
            genericMenu.AddDisabledItem(
                new GUIContent("Add blackboard float or int keys to use as timer values")
                );
            genericMenu.AddItem(
                new GUIContent("None"), 
                false, 
                () => FieldUpdater("", -1)
                );
        }
        else{
            genericMenu.AddItem(
                new GUIContent("None"), 
                false, 
                () => FieldUpdater("", -1)
                );
            if (intKeys != null){
                foreach(KeyValuePair<string, int> kvp in blackboard.GetIntKeys()){
                            genericMenu.AddItem(
                                new GUIContent(kvp.Key), 
                                false, 
                                () => FieldUpdater(kvp.Key, (float)kvp.Value)
                                );
                }
            }
            if (floatKeys != null){
                foreach(KeyValuePair<string, float> kvp in floatKeys){
                        genericMenu.AddItem(
                            new GUIContent(kvp.Key), 
                            false, 
                            () => FieldUpdater(kvp.Key, kvp.Value)
                            );
                }
            }
        }
        genericMenu.ShowAsContext();
    }

    public void SetValueKey(string newValueKey, float value=-1f){
        timerNode.valueKey = newValueKey;
        if (value > 0){
            timerNode.TimerValue = value;
        }
    }

    public void SetRandomDeviationKey(string newKey, float value=-1f){
        timerNode.randomDeviationKey = newKey;
        if (value > 0){
            timerNode.RandomDeviation = value;
        }
    }

    protected bool NoCustomKeys(Dictionary<string, int> intKeys, Dictionary<string,float> floatKeys){

        if (intKeys == null && floatKeys == null) {
            return true;
        }
        if ((intKeys != null && intKeys.Count == 0) && (floatKeys!=null && floatKeys.Count == 0)){
            return true;
        }
        return false;

    }

}
}
