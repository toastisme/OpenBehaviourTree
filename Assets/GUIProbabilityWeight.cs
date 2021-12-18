using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIProbabilityWeight : NodeBase
{
    BehaviourTree bt;
    public GUIProbabilityWeight(string task,
                   Vector2 position, 
                   float width, 
                   float height, 
                   GUIStyle nodeStyle,
                   GUIStyle selectedStyle, 
                   Action<NodeBase> UpdatePanelDetails,
                   BehaviourTree behaviourTree
                   )
    {
        this.style = nodeStyle;
        this.selectedNodeStyle = selectedStyle;
        this.task = task;
        this.name = "";
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.rect = new Rect(position[0], position[1], width, height);
        defaultNodeStyle = nodeStyle;
        this.selectedNodeStyle = selectedStyle;
        bt = behaviourTree;
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
        Dictionary<string, int> intKeys = bt.blackboard.GetIntKeys();
        Dictionary<string, float> floatKeys = bt.blackboard.GetFloatKeys();
        GenericMenu genericMenu = new GenericMenu();
        if ((intKeys == null && floatKeys == null) || (intKeys.Count == 0 && floatKeys.Count == 0)){
            genericMenu.AddDisabledItem(new GUIContent("Add blackboard float or int keys to use as weights"));
        }
        else{
            foreach(KeyValuePair<string, int> kvp in bt.blackboard.GetIntKeys()){
                        genericMenu.AddItem(new GUIContent(kvp.Key), false, () => SetTask(kvp.Key + ": " + kvp.Value.ToString()));
            }
            foreach(KeyValuePair<string, float> kvp in bt.blackboard.GetFloatKeys()){
                    genericMenu.AddItem(new GUIContent(kvp.Key), false, () => SetTask(kvp.Key + ": " + kvp.Value.ToString()));
            }
        }
        genericMenu.ShowAsContext();

    }


}
