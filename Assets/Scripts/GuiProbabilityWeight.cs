using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class GuiProbabilityWeight : GuiNode
{
    Connection parentConnection;
    ProbabilityWeight probabilityWeight;

    public GuiProbabilityWeight(
        Node node,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        ref BehaviourTreeBlackboard blackboard,
        Connection parentConnection
    ): base(
        node:node,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        blackboard:ref blackboard
    )
    {
        this.parentConnection = parentConnection;
        this.probabilityWeight = (ProbabilityWeight)node;
        ApplyDerivedSettings();
    }

    public void SetEditorActions(
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified
    ){
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.TreeModified = TreeModified;
    }

    protected override void ApplyDerivedSettings()
    {
        color = NodeProperties.ProbabilityWeightColor();
        defaultStyle = NodeProperties.GUINodeStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        rect.size = NodeProperties.SubNodeSize();
        iconAndText = NodeProperties.ProbabilityWeightContent();
    }

    

    public void Move(Vector2 pos)
    {
        rect.position = pos;
    }

    public override bool ProcessEvents(Event e, Vector2 mousePos)
    {
        bool guiChanged = false;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (apparentRect.Contains(mousePos))
                    {
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
        }

        return guiChanged;
    }

    public override void Drag(Vector2 delta){}

    bool NoCustomKeys(Dictionary<string, int> intKeys, Dictionary<string,float> floatKeys){
        if (intKeys == null && floatKeys == null) {
            return true;
        }
        if ((intKeys != null && intKeys.Count == 0) && (floatKeys!=null && floatKeys.Count == 0)){
            return true;
        }
        return false;

    }

    protected void ProcessContextMenu(){
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
                new GUIContent("Add blackboard float or int keys to use as weights")
                );
            genericMenu.AddItem(
                new GUIContent("Constant Weight"), 
                false, 
                () => SetTask("Constant Weight")
                );
        }
        else{
            genericMenu.AddItem(
                new GUIContent("Constant Weight"), 
                false, 
                () => SetTask("Constant Weight")
                );
            if (intKeys != null){
                foreach(KeyValuePair<string, int> kvp in blackboard.GetIntKeys()){
                            genericMenu.AddItem(
                                new GUIContent(kvp.Key), 
                                false, 
                                () => SetTask(kvp.Key, (float)kvp.Value)
                                );
                }
            }
            if (floatKeys != null){
                foreach(KeyValuePair<string, float> kvp in floatKeys){
                        genericMenu.AddItem(
                            new GUIContent(kvp.Key), 
                            false, 
                            () => SetTask(kvp.Key, kvp.Value)
                            );
                }
            }
        }
        genericMenu.AddItem(new GUIContent("Remove connection"), false, () => parentConnection.RemoveConnection());
        genericMenu.ShowAsContext();

    }
    public override void Draw()
    {
        if (IsSelected){
            GUI.color = NodeProperties.SelectedTint();
        }
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        iconAndText.text = "\n" + DisplayName + "\n" + DisplayTask + " : " + probabilityWeight.GetWeight().ToString();
        GUI.Box(apparentRect, iconAndText, activeStyle);
        GUI.backgroundColor = currentColor;
        GUI.color = NodeProperties.DefaultTint();
    }
    public void SetTask(string newTask){
        DisplayTask = newTask;
    }

    public void SetTask(string newTask, float val){
        DisplayTask = newTask;
        probabilityWeight.SetWeight(val);
    }

    public void SetParentConnection(Connection connection){
        parentConnection = connection;
    }

    bool HasConstantWeight(){
        return (probabilityWeight.HasConstantWeight());
    }


    public override void DrawDetails(){
        GUILayout.Label(NodeProperties.GetDefaultStringFromNodeType(GetNodeType()));
        GUILayout.Label("Task: " + DisplayTask);
        GUILayout.Label("Name");
        EditorGUI.BeginChangeCheck();
        DisplayName = GUILayout.TextField(DisplayName, 50);
        if (HasConstantWeight()){
            GUILayout.Label("Constant Weight");
            float weight;
            bool success = float.TryParse(
                GUILayout.TextField(probabilityWeight.GetWeight().ToString(), 5), out weight);
            if (success){
                probabilityWeight.SetWeight(weight);
            }
        }
        if (EditorGUI.EndChangeCheck()){
            TreeModified();
        }
    }

}
}
