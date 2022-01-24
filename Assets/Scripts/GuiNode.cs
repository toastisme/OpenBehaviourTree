using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Behaviour{

[Serializable]
public struct GuiNodeData 
{
    public string displayName;
    public float xPos;
    public float yPos;

}
public abstract class GuiNode : IGuiNode
{
    protected BehaviourTreeBlackboard blackboard;
    public Node BtNode{get; protected set;}

    // Actions
    protected Action<GuiNode> UpdatePanelDetails;

    // Appearance 
    public virtual string DisplayTask{
        // What the node actually does
        get{
            return BtNode.TaskName;
        }
        set{
            BtNode.TaskName = value;
        }
    }
    
    protected Rect rect; // The base rect decorators and tasks are drawn on
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
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        ref BehaviourTreeBlackboard blackboard
    ){
        BtNode = node;
        DisplayTask = displayTask;
        DisplayName = displayName;
        Vector2 size = NodeProperties.GuiNodeSize();
        this.rect = new Rect(pos.x, 
                             pos.y, 
                             size.x, 
                             size.y);
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.blackboard = blackboard;

    }

    protected virtual void ApplyDerivedSettings(){}

    public virtual void Drag(Vector2 delta){
        rect.position += delta;
    }

    public bool IsRunning(){
        return (BtNode.CurrentState == NodeState.Running);
    }

    public virtual void Draw(){}

    public virtual bool ProcessEvents(Event e){return false;}

    public virtual void SetSelected(bool selected){
            if (selected){
                IsSelected = true;
                activeStyle = selectedStyle;
            }
            else{
                IsSelected = false;
                activeStyle = defaultStyle;
            }
        }

    protected virtual bool IsRootNode(){return false;}

    public float GetXPos(){
        return GetPos().x;
    }

    public Vector2 GetPos(){
        return rect.position;
    }

    public void SetPosition(Vector2 pos){
        Drag(pos - rect.position);

    }

    public virtual void DrawDetails(){
        GUILayout.Label(NodeProperties.GetDefaultStringFromNodeType(GetNodeType()));
        GUILayout.Label("Task: " + DisplayTask);
        GUILayout.Label("Name");
        DisplayName = GUILayout.TextField(DisplayName, 50);
    }

    public NodeType GetNodeType(){return BtNode.GetNodeType();}
    public Rect GetRect(){return rect;}
}
}