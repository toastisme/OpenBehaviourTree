using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Behaviour{
public abstract class GuiNode : IGuiNode
{
    protected BehaviourTreeBlackboard blackboard;

    // Actions
    protected Action<GuiNode> UpdatePanelDetails;

    // Appearance 
    public string DisplayTask{
        // What the node actually does
        get{
            return Node.TaskName;
        }
        set{
            Node.TaskName = value;
        }
    }
    
    Rect rect; // The base rect decorators and tasks are drawn on
    public string DisplayName{get; set;}
    protected GUIStyle activeStyle;
    protected GUIStyle defaultStyle;
    protected GUIStyle selectedStyle;
    protected Color color;

    // Bookkeeping
    protected bool isDragged;
    public bool IsSelected{get; protected set;}

    protected GuiNode(
        Node node,
        string displayTask,
        string displayName,
        Rect rect,
        Action<GuiNode> UpdatePanelDetails,
        ref BehaviourTreeBlackboard blackboard
    ){
        BtNode = node;
        DisplayTask = displayTask;
        DisplayName = displayName;
        this.rect = rect;
        this.UpdatePanelDetails = UpdatePanelDetails;

        ApplyDerivedSettings();
    }

    protected virtual void ApplyDerivedSettings(){}

    public virtual void Drag(Vector2 delta){
        rect.position += delta;
    }

    bool IsRunning(){
        return (BtNode.NodeState == NodeState.Running);
    }

    public virtual void Draw(){}

    public virtual bool ProcessEvents(Event e){}

    public virtual void SetSelected(bool selected);

    protected virtual bool IsRootNode(){return false;}

    public float GetXPos(){
        return rect.x;
    }
    public void SetParentConnection(Connection connection){
        this.ParentConnection = connection;       
    }

    public void SetPosition(Vector2 pos){
        Drag(pos - rect.position);

    }

    public virtual void DrawDetails(){
        GUILayout.Label("Task: " + DisplayTask);
        GUILayout.Label("Name");
        DisplayName = GUILayout.TextField(DisplayName, 50);
    }
}
}