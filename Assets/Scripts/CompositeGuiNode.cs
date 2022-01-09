using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public class CompositeGuiNode : CallableGuiNode
{
    /**
     * CompositeGuiNodes are nodes made up of GuiNodes (i.e they hold a list of GuiDecorators),
     * and have connections to other CompositeGuiNodes.
     */
     
    public Node BtNode {get; protected set;} // The node this GuiNode is displaying
    public List<GuiDecorator> Decorators{get; set;}
    Action<CompositeGuiNode> OnRemoveNode;

    // Connection points
    public ConnectionPoint ChildPoint{get; set;}
    public ConnectionPoint ParentPoint{get; set;}

    // Connections
    public Connection ParentConnection{get; set;}
    public List<Connection> ChildConnections{get; set;}

    // Appearance
    Rect taskRect; // Rect containing the task and display name
    protected Color taskRectColor;
    protected Vector2 taskRectSize = NodeProperites.SubNodeSize();

    public CompositeGuiNode(
        Node node,
        string displayTask,
        string displayName,
        Rect rect,
        Connection parentConnection,
        Action<GuiNode> UpdatePanelDetails,
        Action<CompositeGuiNode> OnRemoveNode,
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint,
        ref BehaviourTreeBlackboard blackboard
    ) :base(
        node:node,
        displayTask:displayTask,
        displayName:displayName,
        rect:rect,
        UpdatePanelDetails:UpdatePanelDetails,
        blackboard: ref blackboard
    )
    {
        ParentConnection = parentConnection;
        ChildConnections = new List<Connection>();
        Decorators = new List<GuiDecorator>();
        this.OnRemoveNode = OnRemoveNode;


        ApplyDerivedSettings();
        ApplyNodeTypeSettings(
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint
            );

    }

    protected override void ApplyDerivedSettings(){
        defaultStyle = NodeProperties.GUINodeStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = NodeProperties.DefaultColor();
        taskRectColor = NodeProperties.DefaultColor();
    }

    protected virtual void ApplyNodeTypeSettings(
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

    }

    public override void Drag(Vector2 delta){
        base.Drag(delta);
        taskRect.position += delta;
        DragDecorators(delta);
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

    public override void Draw(){
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
        GUI.backgroundColor = taskRectColor; 
        GUI.Box(taskRect, "\n" + DisplayName + "\n" + DisplayTask, activeStyle);
    }

    void DrawDecorators(){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                decorator.Draw();
            }
        }
    }

    public override bool ProcessEvents(Event e){
        bool guiChanged = false;
        bool decoratorSelected = false;

        guiChanged = ProcessDecoratorEvents(e, decoratorSelected);
        // Only bother processing taskRect events if no decorator was selected
        if (!decoratorSelected){
            bool guiChangedFromNode =  ProcessTaskRectEvents(e);
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

    public virtual bool ProcessTaskRectEvents(Event e){
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

    protected void OnClickRemoveDecorator(GuiDecorator decorator){
        int idx = Decorators.FindIndex(a => a==decorator);
        if (idx != -1){

            Decorators.Remove(decorator);

            // Resize node 
            rect.height -= taskRectSize[1];
            taskRect.y -= taskRectSize[1];
            CallNumber.NodeRect.y -= taskRectSize[1];

            // Move all decorators below the removed one up
            Vector2 moveVec = new Vector2(0, -taskRectSize[1]);
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
        //TODO update this constructor
        GuiDecorator decorator = new GuiDecorator( 
                            blackboard:ref blackboard,
                            displayTask:displayTask,
                            displayName:"",
                            rect:new Rect(
                            rect.x + initDecoratorPos[0], 
                            rect.y + initDecoratorPos[1]+(taskRectSize[1]*decorators.Count+1),
                            taskRectSize[0],
                            taskRectSize[1] 
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
                
        rect.height += taskRectSize[1];
        taskRect.y += taskRectSize[1];
        callNumber.NodeRect.y += taskRectSize[1];
        GUI.changed = true;
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

    public void RefreshDecoratorTasks(string oldKeyName, string newKeyName){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                if (decorator.displayTask == oldKeyName){
                    decorator.displayTask = newKeyName;
                }
            }
        }
    }
    protected void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}
}