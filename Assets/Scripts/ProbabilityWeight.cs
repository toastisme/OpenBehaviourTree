using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BehaviourBase{
public class ProbabilityWeight : AggregateNode
    {

        public ProbabilityWeight(string displayWeight,
                    Rect rect,
                    GUIStyle defaultStyle,
                    GUIStyle selectedStyle, 
                    Color color,
                    Action<AggregateNode> UpdatePanelDetails,
                    ref BehaviourTreeBlackboard blackboard,
                    Connection parentConnection
                    ) : base(
                        nodeType:NodeType.ProbabilityWeight,
                        displayTask:displayWeight,
                        displayName:"",
                        rect:rect,
                        parentNode:null,
                        UpdatePanelDetails:UpdatePanelDetails,
                        nodeStyles:null,
                        nodeColors:null,
                        OnClickChildPoint:null,
                        OnClickParentPoint:null,
                        OnRemoveNode:null,
                        blackboard:ref blackboard
                    )
        {
            
            this.blackboard = blackboard;
            this.parentConnection = parentConnection;
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