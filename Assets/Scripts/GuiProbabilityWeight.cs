using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiProbabilityWeight : GuiNode
{
    Connection parentConnection;

    public GuiProbabilityWeight(
        ProbabilityWeight node,
        string displayTask,
        string displayName,
        Rect rect,
        Action<GuiNode> UpdatePanelDetails,
        ref BehaviourTreeBlackboard blackboard,
        Connection parentConnection
    ): base(
        node:node,
        displayTask:displayTask,
        displayName:displayName,
        rect:rect,
        UpdatePanelDetails:UpdatePanelDetails,
        blackboard:blackboard
    )
    {
        this.parentConnection = parentConnection;
        ApplyDerivedSettings();
    }

    protected override void ApplyDerivedSettings()
    {
        color = NodeProperties.ProbabilityWeightColor();
        defaultStyle = NodeProperties.ProbabilityWeightStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
    }

    

    public void Move(Vector2 pos)
    {
        rect.position = pos;
    }

    public override bool ProcessEvents(Event e)
    {
        bool guiChanged = false;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
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
                if (e.button == 1 && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;
        }

        return guiChanged;
    }

    public override void Drag(Vector2 delta){}

    protected override void ProcessContextMenu(){
        Dictionary<string, int> intKeys = blackboard.GetIntKeys();
        Dictionary<string, float> floatKeys = blackboard.GetFloatKeys();
        GenericMenu genericMenu = new GenericMenu();
        if ((intKeys == null && floatKeys == null) || (intKeys.Count == 0 && floatKeys.Count == 0)){
            genericMenu.AddDisabledItem(new GUIContent("Add blackboard float or int keys to use as weights"));
            genericMenu.AddItem(new GUIContent("Constant weight (1)"), false, () => SetTask("Constant weight (1)"));
        }
        else{
            genericMenu.AddItem(new GUIContent("Constant weight (1)"), false, () => SetTask("Constant weight (1)"));
            foreach(KeyValuePair<string, int> kvp in blackboard.GetIntKeys()){
                        genericMenu.AddItem(new GUIContent(kvp.Key), false, () => SetTask(kvp.Key + ": " + kvp.Value.ToString()));
            }
            foreach(KeyValuePair<string, float> kvp in blackboard.GetFloatKeys()){
                    genericMenu.AddItem(new GUIContent(kvp.Key), false, () => SetTask(kvp.Key + ": " + kvp.Value.ToString()));
            }
        }
        genericMenu.AddItem(new GUIContent("Remove connection"), false, () => parentConnection.OnClickRemoveConnection(parentConnection));
        genericMenu.ShowAsContext();

    }
    public override void Draw()
    {
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUI.Box(rect, "\n" + displayName + "\n" + displayTask, activeStyle);
        GUI.backgroundColor = currentColor;
    }
    public void SetTask(string newTask){
        displayTask = newTask;
    }


}
}
