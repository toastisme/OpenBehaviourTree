using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Behaviour{

[Serializable]
public struct GuiNodeData 
{
    /**
     * \struct GuiNodeData
     * Used to store data specific to GuiNode, decoupled
     * from Node.
     */ 
    public string displayName;
    public float xPos;
    public float yPos;

}
public abstract class GuiNode : IGuiNode
{

    /**
    * \class GuiNode
    * Base class for displaying a node in the BehaviourTree class, using the BehaviourTreeEditor.
    */

    protected BehaviourTreeBlackboard blackboard;

    // The underlying Node being displayed
    public Node BtNode{get; protected set;} 

    // Actions

    // How node details are passed to the BehaviourTreeEditor
    protected Action<GuiNode> UpdatePanelDetails; 

    // How the BehaviourTreeEditor knows when a change has been made to the node
    protected Action TreeModified; 

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
    
    protected GUIContent iconAndText;
    protected Rect rect; // The base rect decorators and tasks are drawn on
    protected Rect scaledRect; // rect with current zoom taken into account
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
        Action TreeModified,
        ref BehaviourTreeBlackboard blackboard
    ){
        BtNode = node;
        DisplayTask = displayTask;
        DisplayName = displayName;
        Vector2 size = BehaviourTreeProperties.GuiNodeSize();
        this.rect = new Rect(pos.x, 
                             pos.y, 
                             size.x, 
                             size.y);
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.TreeModified = TreeModified;
        this.blackboard = blackboard;
        iconAndText = new GUIContent();
    }

    protected virtual void ApplyDerivedSettings(){}

    public virtual void Drag(Vector2 delta){
        rect.position += delta;
        TreeModified();
    }

    public bool IsRunning(){
        return (BtNode.CurrentState == NodeState.Running);
    }

    public bool HasFailed(){
        return (BtNode.CurrentState == NodeState.Failed);
    }

    public virtual void Draw(){}
    public virtual bool ProcessEvents(Event e, Vector2 mousePos){return false;}

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

    public Vector2 GetScaledPos(){
        return scaledRect.position;
    }

    public void SetPosition(Vector2 pos){
        Drag(pos - rect.position);

    }

    public virtual void UpdateOrigin(Vector2 origin){
        /**
         * The origin from which the scaled rect is drawn from to 
         * factory in zooming
         */
        scaledRect = new Rect(rect.x, rect.y, rect.width, rect.height);
        scaledRect.position -= origin;

    }

    public virtual void DrawDetails(){

        /**
         * What details are displayed in the details panel of the BehaviourTreeEditor
         */
         
        GUILayout.Label(BehaviourTreeProperties.GetDefaultStringFromNodeType(GetNodeType()));
        GUILayout.Label("Task: " + DisplayTask);
        GUILayout.Label("Name");
        EditorGUI.BeginChangeCheck();
        DisplayName = GUILayout.TextField(DisplayName, 50);
        if (EditorGUI.EndChangeCheck()){
            TreeModified();
            UpdateBoxWidth(GetRequiredBoxWidth());
        }
    }

    public virtual NodeType GetNodeType(){return BtNode.GetNodeType();}

    public Rect GetRect(){return rect;}

    public virtual void UpdateBlackboard(ref BehaviourTreeBlackboard newBlackboard){
        blackboard = newBlackboard;
        BtNode.UpdateBlackboard(ref newBlackboard);

    }

    public virtual float GetRequiredBoxWidth(){

        /**
         * A dirty way to approximate the width rect needs to
         * fit the GuiNode text.
         */

        float dx = BehaviourTreeProperties.ApproximateNodeTextWidth();
        float minWidth = BehaviourTreeProperties.GuiNodeSize().x;
        float length = Mathf.Max(DisplayName.Length, DisplayTask.Length)*dx;
        return Mathf.Max(minWidth, length);

    }

    public virtual void UpdateBoxWidth(float newWidth){
        rect.width = newWidth;
    }
}
}