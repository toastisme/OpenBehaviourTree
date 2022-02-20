using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class GuiProbabilityWeight : GuiNode
{
    /**
    * \class GuiProbabilityWeight
    * Displays an ProbabilityWeight in the BehaviourTree class using the BehaviourTreeEditor.
    * These are children of Connections.
    */
    Connection parentConnection; // The Connection self is child of 
    ProbabilityWeight probabilityWeight; // The ProbabilityWeight being displayed

    public Action NodeUpdated;

    public GuiProbabilityWeight(
        Node node,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action NodeUpdated,
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
        this.NodeUpdated = NodeUpdated;
        this.probabilityWeight = (ProbabilityWeight)node;
        ApplyDerivedSettings();
        if (NodeUpdated != null){
            NodeUpdated();
        }
    }

    public void SetEditorActions(
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified
    ){
        /**
         * Allows for decoupling editor actions being set from the 
         * constructor (required e.g when loading from disk in BehaviourTreeLoader)
         */ 
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.TreeModified = TreeModified;
    }

    protected override void ApplyDerivedSettings()
    {
        color = BehaviourTreeProperties.ProbabilityWeightColor();
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        rect.size = BehaviourTreeProperties.SubNodeSize();
        iconAndText = BehaviourTreeProperties.ProbabilityWeightContent();
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
                    if (scaledRect.Contains(mousePos))
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
                if (e.button == 1 && scaledRect.Contains(mousePos))
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
            GUI.color = BehaviourTreeProperties.SelectedTint();
        }
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        iconAndText.text = "\n" + DisplayName + "\n" + DisplayTask + " : " + probabilityWeight.GetWeight().ToString();
        GUI.Box(scaledRect, iconAndText, activeStyle);
        GUI.backgroundColor = currentColor;
        GUI.color = BehaviourTreeProperties.DefaultTint();
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

    public bool HasConstantWeight(){
        return (probabilityWeight.HasConstantWeight());
    }


    public override void DrawDetails(){
        /**
         * What details are displayed in the details panel of the BehaviourTreeEditor
         */
        GUILayout.Label(BehaviourTreeProperties.GetDefaultStringFromNodeType(GetNodeType()));
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
            NodeUpdated();
        }
    }
    public override float GetRequiredBoxWidth(){

        /**
         * A dirty way to approximate the width rect needs to
         * fit the GuiNode text.
         */

        float dx = BehaviourTreeProperties.ApproximateDecoratorTextWidth();
        float minWidth = BehaviourTreeProperties.GuiNodeSize().x;
        string taskString = DisplayTask + " : " + probabilityWeight.GetWeight().ToString();
        float length = Mathf.Max(DisplayName.Length, taskString.Length)*dx;
        return Mathf.Max(minWidth, length);

    }

}
}
