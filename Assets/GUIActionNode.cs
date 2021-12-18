using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIActionNode : GUINode
{
    List<string> customTaskNames;
    public GUIActionNode(string task,
                   Vector2 position, 
                   float width, 
                   float height, 
                   GUIStyle nodeStyle, 
                   GUIStyle selectedStyle, 
                   GUIStyle ChildPointStyle, 
                   GUIStyle ParentPointStyle, 
                   GUIStyle callNumberStyle, 
                   GUIStyle decoratorStyle, 
                   GUIStyle selectedDecoratorStyle, 
                   Action<NodeBase> UpdatePanelDetails,
                   Action<ConnectionPoint> OnClickChildPoint, 
                   Action<ConnectionPoint> OnClickParentPoint, 
                   Action<GUINode> OnClickRemoveNode,
                   BehaviourTree behaviourTree,
                   List<string> customTaskNames
                   ) : base(task,
                          position,
                          width,
                          height,
                          nodeStyle,
                          selectedStyle,
                          ChildPointStyle,
                          ParentPointStyle,
                          callNumberStyle,
                          decoratorStyle,
                          selectedDecoratorStyle,
                          UpdatePanelDetails,
                          OnClickChildPoint,
                          OnClickParentPoint,
                          OnClickRemoveNode,
                          behaviourTree)
    {
        this.customTaskNames = customTaskNames;

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
