using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BehaviourTree{
public abstract class GuiNode : IGuiNode
{
    Node BtNode {get; protected set;} // The node this GuiNode is displaying
    protected BehaviourTreeBlackboard blackboard;
    public List<GuiDecorator> Decorators{get; set;}

    // Actions
    protected Action<GuiNode> UpdatePanelDetails;
    protected Action<GuiNode> OnRemoveNode;

    // Connection points
    public ConnectionPoint ChildPoint{get; set;}
    public ConnectionPoint ParentPoint{get; set;}

    // Connections
    public Connection ParentConnection{get; set;}
    public List<Connection> ChildConnections{get; set;}

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
    Rect taskRect; // Rect containing the task and display name
    CallNumberNode callNumber; // Displays when the node will be called in the tree
    public string DisplayName{get; set;}
    protected GUIStyle activeStyle;
    protected GUIStyle defaultStyle;
    protected GUIStyle selectedStyle;
    protected Color color;
    protected Color subNodeColor;
    protected Vector2 subNodeSize = NodeProperites.SubNodeSize();

    // Bookkeeping
    protected bool isDragged;
    public bool IsSelected{get; protected set;}

    protected GuiNode(
        Node node,
        string displayTask,
        string displayName,
        Rect rect,
        Connection parentConnection,
        Action<GuiNode> UpdatePanelDetails,
        Action<GuiNode> OnRemoveNode,
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint,
        ref BehaviourTreeBlackboard blackboard
    ){
        BtNode = node;
        DisplayTask = displayTask;
        DisplayName = displayName;
        this.rect = rect;

        ParentConnection = parentConnection;
        ChildConnections = new List<Connection>();
        Decorators = new List<GuiDecorator>();

        callNumber = new CallNumberNode();
        callNumber.NodeRect.position = rect.position;

        this.UpdatePanelDetails = UpdatePanelDetails;
        this.OnRemoveNode = OnRemoveNode;

        ApplyNodeTypeSettings(
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint
            );

    }

    virtual void ApplyNodeTypeSettings(
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){

        // Default GuiNode has a child and parent point
        ChildPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.In, 
                                            nodeStyles.childPointStyle, 
                                            OnClickChildPoint);
        CarentPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.Out, 
                                            nodeStyles.parentPointStyle, 
                                            OnClickParentPoint);

        defaultStyle = NodeProperties.GUINodeStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = NodeProperties.DefaultColor();
        subNodeColor = NodeProperties.DefaultColor();
    }
    public virtual void Drag(Vector2 delta){
        DragSelf(delta);
        callNumber.Drag(delta);
        DragDecorators(delta);
    }

    void DragSelf(Vector2 delta){
        rect.position += delta;
        taskRect.position += delta;
    }

    void DragDecorators(Vector2 delta){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                decorator.Drag(delta);
            }
        }
    }

    public virtual void DragWithChildren(Vector2 delta){
        Drag(delta);
        if (ChildConnections != null){
            foreach(Connection connection in ChildConnections){
                connection.ChildNode.Drag(delta);
            }
        }
    }

    bool IsRunning(){
        return (BtNode.NodeState == NodeState.Running);
    }

    public virtual void Draw(){
        Color currentColor = GUI.backgroundColor;
        ChildPoint?.Draw();
        ParentPoint?.Draw();
        if (IsRunning()){
            GUI.color = NodeProperties.RunningTint;
        }
        DrawSelf();
        callNumber.Draw();
        DrawDecorators();
        GUI.backgroundColor = currentColor;
        GUI.color = NodeProperties.DefaultTint;
    }

    void DrawSelf(){
        GUI.backgroundColor = color;
        GUI.Box(rect, "", activeStyle);
        GUI.backgroundColor = subNodeColor; 
        GUI.Box(taskRect, "\n" + DisplayName + "\n" + DisplayTask, activeStyle);
    }

    void DrawDecorators(){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                decorator.Draw();
            }
        }
    }

    public virtual bool ProcessEvents(Event e){
        bool guiChanged = false;
        bool decoratorSelected = false;

        guiChanged = ProcessDecoratorEvents(e, decoratorSelected);
        // Only bother processing subnode events if no decorator was selected
        if (!decoratorSelected){
            bool guiChangedFromNode =  ProcessSubNodeEvents(e);
            if (guiChangedFromNode){
                guiChanged = true;
            }
        }
        else{
            SetSelected(false);
        }
        return guiChanged;
    }

    bool ProcessDecoratorEvents(Event e, out bool decoratorSelected){
        bool guiChanged = false;
        if (decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                bool guiChangedFromDecorator = decorator.ProcessEvents(e);
                if (!guiChanged && guiChangedFromDecorator){
                    guiChanged = true;
                }
                if (!decoratorSelected && decorator.IsSelected()){
                    decoratorSelected = true;
                }
            }
        }
        return guiChanged;
    }

    public virtual bool ProcessSubNodeEvents(Event e){
        bool guiChanged = false;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
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

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    DragWithChildren(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return guiChanged;
    }

    protected void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    protected void OnClickRemoveDecorator(GuiDecorator decorator){
        int idx = Decorators.FindIndex(a => a==decorator);
        if (idx != -1){

            Decorators.Remove(decorator);

            // Resize node 
            rect.height -= subNodeSize[1];
            taskRect.y -= subNodeSize[1];
            CallNumber.NodeRect.y -= subNodeSize[1];

            // Move all decorators below the removed one up
            Vector2 moveVec = new Vector2(0, -subNodeSize[1]);
            for (int i = idx; i < decorators.Count; i++){
                decorators[i].Drag(moveVec);
            }
            GUI.changed = true;
        }
        else{
            throw new Exception("Decorator not found in decorators list.");
        }

    }

    protected void OnClickAddDecorator(string displayTask){
        GuiDecorator decorator = new GuiDecorator( 
                            blackboard:ref blackboard,
                            displayTask:displayTask,
                            displayName:"",
                            rect:new Rect(
                            rect.x + initDecoratorPos[0], 
                            rect.y + initDecoratorPos[1]+(subNodeSize[1]*decorators.Count+1),
                            subNodeSize[0],
                            subNodeSize[1] 
                            ),
                            parentNode:this.parentNode,
                            UpdatePanelDetails:UpdatePanelDetails,
                            OnRemoveDecorator:OnClickRemoveDecorator,
                            childNode:this
                            );
        decorator.SetCallNumber(callNumber);
        callNumber.CallNumber++;
        this.parentNode = decorator;
        decorators.Add(decorator);
                
        rect.height += subNodeSize[1];
        taskRect.y += subNodeSize[1];
        callNumber.NodeRect.y += subNodeSize[1];
        GUI.changed = true;
    }

    public virtual void SetSelected(bool selected);

    public void SetCallNumber(int num){
        callNumber.CallNumber = num;
    }
    protected virtual void ProcessContextMenu()
    {
        if (!IsRootNode()){
            GenericMenu genericMenu = new GenericMenu();
            Dictionary<string, bool> boolKeys = blackboard.GetBoolKeys();
            if (boolKeys == null || boolKeys.Count == 0){
                genericMenu.AddDisabledItem(new GUIContent("Add blackboard bool keys to use as decorators"));
            }
            else{            
                foreach(string boolName in blackboard.GetBoolKeys().Keys){
                    if (!DecoratorKeyActive(boolName)){
                        genericMenu.AddItem(new GUIContent("Add Decorator/" + boolName), false, () => OnClickAddDecorator(boolName));
                    }
                    else{
                        genericMenu.AddDisabledItem(new GUIContent("Add Decorator/" + boolName));
                    }
                }
            }
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }
    }

    protected virtual bool IsRootNode(){return false;}
    public void AddChildConnection(Connection connection){
        this.ChildConnections.Add(connection);
    }
    public void RemoveChildConnection(Connection connection){
        this.ChildConnections.Remove(connection);
    }
    public void RemoveParentConnection(){
        this.ParentConnection.GetParentNode().RemoveChildConnection(this.ParentConnection);
        this.ParentConnection = null;
    }

    public void RefreshChildOrder(){
        /**
        * Orders childConnections by x position
        */
        if (ChildConnections != null){
            ChildConnections.Sort((x,y) => x.GetChildNode().GetXPos().CompareTo(y.GetChildNode().GetXPos()));
        }
        childNodes = new List<Node>();
        foreach(Connection connection in ChildConnections){
            ChildNodes.Add(connection.GetChildNode());
        }

    }

    public float GetXPos(){
        return rect.x;
    }
    public void SetParentConnection(Connection connection){
        this.ParentConnection = connection;       
    }
    public void RefreshDecoratorTasks(string oldKeyName, string newKeyName){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                if (decorator.displayTask == oldKeyName){
                    decorator.displayTask = newKeyName;
                }
            }
        }
    }

    public void SetPosition(Vector2 pos){
        Drag(pos - rect.position);

    }

    private bool DecoratorKeyActive(string boolName){
        if (Decorators != null){
            for (int i=0; i < Decorators.Count; i++){
                if (Decorators[i].displayTask == boolName){
                    return true;
                }
            }
        }
        return false;
    }

    public virtual void DrawDetails(){
        GUILayout.Label("Task: " + DisplayTask);
        GUILayout.Label("Name");
        DisplayName = GUILayout.TextField(DisplayName, 50);
    }
}
}