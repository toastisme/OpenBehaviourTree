using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIProbabilityWeight : NodeBase
{
    BehaviourTree bt;
    Color nodeColor;
    public GUIProbabilityWeight(string task,
                   Vector2 position, 
                   Vector2 size,
                   GUIStyle nodeStyle,
                   GUIStyle selectedStyle, 
                   Color nodeColor,
                   Action<NodeBase> UpdatePanelDetails,
                   BehaviourTree behaviourTree
                   )
    {
        SetNodeTypeFromTask(task);
        this.style = nodeStyle;
        this.nodeColor = nodeColor;
        this.selectedNodeStyle = selectedStyle;
        this.task = task;
        this.name = "";
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.rect = new Rect(position.x, position.y, size.x, size.y);
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
            genericMenu.AddItem(new GUIContent("Constant weight (1)"), false, () => SetTask("Constant weight (1)"));
        }
        else{
            genericMenu.AddItem(new GUIContent("Constant weight (1)"), false, () => SetTask("Constant weight (1)"));
            foreach(KeyValuePair<string, int> kvp in bt.blackboard.GetIntKeys()){
                        genericMenu.AddItem(new GUIContent(kvp.Key), false, () => SetTask(kvp.Key + ": " + kvp.Value.ToString()));
            }
            foreach(KeyValuePair<string, float> kvp in bt.blackboard.GetFloatKeys()){
                    genericMenu.AddItem(new GUIContent(kvp.Key), false, () => SetTask(kvp.Key + ": " + kvp.Value.ToString()));
            }
        }
        genericMenu.ShowAsContext();

    }
    public override void Draw()
    {
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;
        GUI.Box(rect, "\n" + name + "\n" + task, style);
        GUI.backgroundColor = currentColor;
    }


}
