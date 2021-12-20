using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIActionNode : GUINode
{
    List<string> customTaskNames;
    public GUIActionNode(string task,
                   Vector2 position, 
                   Vector2 size,
                   NodeStyles nodeStyles,
                   NodeColors nodeColors,
                   Action<NodeBase> UpdatePanelDetails,
                   Action<ConnectionPoint> OnClickChildPoint, 
                   Action<GUINode> OnClickRemoveNode,
                   BehaviourTree behaviourTree,
                   List<string> customTaskNames
                   ) : base(NodeType.Action,
                          position,
                          size,
                          nodeStyles,
                          nodeColors,
                          UpdatePanelDetails,
                          OnClickChildPoint,
                          null,
                          OnClickRemoveNode,
                          behaviourTree)
    {
        this.customTaskNames = customTaskNames;
        this.task = task;

    }
    protected override void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        Dictionary<string, bool> boolKeys = bt.blackboard.GetBoolKeys();
        if (boolKeys == null || boolKeys.Count == 0){
            genericMenu.AddDisabledItem(new GUIContent("Add blackboard bool keys to use as decorators"));
        }
        else{            
            foreach(string boolName in bt.blackboard.GetBoolKeys().Keys){
                genericMenu.AddItem(new GUIContent("Add Decorator/" + boolName), false, () => OnClickAddDecorator(boolName));
            }
        }
        foreach(string taskName in customTaskNames){
            genericMenu.AddItem(new GUIContent("Action/" + taskName), false, () => SetTask(taskName));
        }
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }
}
