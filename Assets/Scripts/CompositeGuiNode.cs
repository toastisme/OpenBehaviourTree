using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class CompositeGuiNode : CallableGuiNode
{
    /**
     * CompositeGuiNodes are nodes made up of GuiNodes (i.e they hold a list of GuiDecorators),
     * and have connections to other CompositeGuiNodes.
     */
     
    public List<GuiDecorator> Decorators{get; set;}
    GuiNodeTimer guiTimeout;
    GuiNodeTimer guiCooldown;

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
    protected Vector2 taskRectSize = NodeProperties.SubNodeSize();
    private Vector2 initDecoratorPos = NodeProperties.InitDecoratorPos();

    public CompositeGuiNode(
        Node node,
        string displayTask,
        string displayName,
        Vector2 pos,
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
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        blackboard: ref blackboard
    )
    {
        ParentConnection = parentConnection;
        ChildConnections = new List<Connection>();
        Decorators = new List<GuiDecorator>();
        this.OnRemoveNode = OnRemoveNode;
        taskRect = new Rect(
            rect.x,
            rect.y+taskRectSize.y*.5f,
            taskRectSize.x,
            taskRectSize.y
        );
        callNumber.SetPosition(taskRect.position);
        ApplyDerivedSettings();
        ApplyNodeTypeSettings(
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint
            );
    }

    public void SetEditorActions(
        Action<GuiNode> UpdatePanelDetails,
        Action<CompositeGuiNode> OnRemoveNode,
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.OnRemoveNode = OnRemoveNode;
        ApplyNodeTypeSettings(
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint
        );
        foreach(GuiDecorator decorator in Decorators){
            decorator.SetEditorActions(
                UpdatePanelDetails:UpdatePanelDetails,
                OnRemoveDecorator:OnClickRemoveDecorator
            );
        }
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
                                            NodeProperties.ChildPointStyle(), 
                                            OnClickChildPoint);
        ParentPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.Out, 
                                            NodeProperties.ParentPointStyle(), 
                                            OnClickParentPoint);

    }

    public override void Drag(Vector2 delta){
        base.Drag(delta);
        taskRect.position += delta;
        DragUnselectedDecorators(delta);
    }

    void DragUnselectedDecorators(Vector2 delta){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                if (!decorator.IsSelected){
                    decorator.Drag(delta);
                }
            }
        }
        if (guiTimeout != null && !guiTimeout.IsSelected){
            guiTimeout.Drag(delta);
        }
        if (guiCooldown != null && !guiCooldown.IsSelected){
            guiCooldown.Drag(delta);
        }
    }

    public virtual void DragWithChildren(Vector2 delta){
        Drag(delta);
        if (ChildConnections != null){
            foreach(Connection connection in ChildConnections){
                connection.GetChildNode().DragWithChildren(delta);
            }
        }
    }

    public override void Draw(){
        Color currentColor = GUI.backgroundColor;
        ChildPoint?.Draw();
        ParentPoint?.Draw();
        if (IsRunning()){
            GUI.color = NodeProperties.RunningTint();
        }
        DrawSelf();
        callNumber.Draw();
        DrawDecorators();
        guiTimeout?.Draw();
        guiCooldown?.Draw();
        GUI.backgroundColor = currentColor;
        GUI.color = NodeProperties.DefaultTint();
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
        bool timerSelected = false;

        guiChanged = ProcessDecoratorEvents(e, out decoratorSelected, out timerSelected);
        // Only bother processing taskRect events if no decorator was selected
        if (!decoratorSelected && !timerSelected){
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

    bool ProcessDecoratorEvents(Event e, out bool decoratorSelected, out bool timerSelected){
        bool guiChanged = false;
        decoratorSelected = false;
        timerSelected = false;
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                bool guiChangedFromDecorator = decorator.ProcessEvents(e);
                if (!guiChanged && guiChangedFromDecorator){
                    guiChanged = true;
                }
                if (!decoratorSelected && decorator.IsSelected){
                    decoratorSelected = true;
                }
            }
        }
        if (!decoratorSelected){
            bool changedFromTimers = false;
            if (guiTimeout != null){
                changedFromTimers = guiTimeout.ProcessEvents(e);
                if (!timerSelected && guiTimeout.IsSelected){
                    timerSelected = true;
                }
            }
            if (!timerSelected && guiCooldown != null){
                changedFromTimers = guiCooldown.ProcessEvents(e);
                if (!timerSelected && guiCooldown.IsSelected){
                    timerSelected = true;
                }
            }
            if (!guiChanged && changedFromTimers){
                guiChanged = true;
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

        decorator.BtNode.Unlink();
        decorator.SetSelected(false);

        int idx = Decorators.FindIndex(a => a==decorator);
        if (idx != -1){

            Decorators.Remove(decorator);

            // Resize node 
            rect.height -= taskRectSize[1];
            taskRect.y -= taskRectSize[1];
            callNumber.Drag(new Vector2(0, -taskRectSize[1]));
            SetCallNumber(callNumber.CallNumber-1);

            // Move all decorators below the removed one up
            Vector2 moveVec = new Vector2(0, -taskRectSize[1]);
            for (int i = idx; i < Decorators.Count; i++){
                Decorators[i].Drag(moveVec);
                Decorators[i].SetCallNumber(Decorators[i].callNumber.CallNumber-1);
            }
            GUI.changed = true;
        }
        else{
            throw new Exception("Decorator not found in decorators list.");
        }

    }

    public void AddDecorator(GuiDecorator guiDecorator){
        guiDecorator.SetParentGuiNode(this);
         if (Decorators != null && Decorators.Count >0){
            guiDecorator.SetCallNumber(Decorators[0].callNumber.CallNumber);
        }
        else{
            guiDecorator.SetCallNumber(this.callNumber.CallNumber);
        }
        callNumber.CallNumber++;
        // Maker room for new decorator
        Decorators.Insert(0, guiDecorator);
                
        // Update params to make space for gui decorator
        rect.height += taskRectSize[1];
        taskRect.y += taskRectSize[1];
        callNumber.Drag(new Vector2(0, taskRectSize[1]));
        GUI.changed = true;

    }

    protected void OnClickAddDecorator(string displayTask){

        // Add actual decorator
        Decorator decorator = new Decorator(
            taskName:displayTask,
            blackboard:ref blackboard
        );

         if (Decorators != null && Decorators.Count >0){
            Decorators[0].BtNode.InsertBeforeSelf(decorator);
        }
        else{
            this.BtNode.InsertBeforeSelf(decorator);
        }
        // Insert before this node in list


        Vector2 pos = new Vector2(
            rect.x + initDecoratorPos[0],
            rect.y + initDecoratorPos[1]
        );
        if (guiTimeout != null){
            pos += new Vector2(0, taskRectSize[1]); 
        }
        if (guiCooldown != null){
            pos += new Vector2(0, taskRectSize[1]); 
        }
        // Add gui decorator
        GuiDecorator guiDecorator = new GuiDecorator( 
            decorator:decorator,
            displayTask:displayTask,
            displayName:"",
            pos:pos,
            UpdatePanelDetails:UpdatePanelDetails,
            OnRemoveDecorator:OnClickRemoveDecorator,
            blackboard:ref blackboard,
            parentGuiNode:this
            );
         if (Decorators != null && Decorators.Count >0){
            guiDecorator.SetCallNumber(Decorators[0].callNumber.CallNumber);
        }
        else{
            guiDecorator.SetCallNumber(this.callNumber.CallNumber);
        }
        callNumber.CallNumber++;
        // Maker room for new decorator
        ShiftDecoratorsDown();
        Decorators.Insert(0, guiDecorator);
                
        // Update params to make space for gui decorator
        rect.height += taskRectSize[1];
        taskRect.y += taskRectSize[1];
        callNumber.Drag(new Vector2(0, taskRectSize[1]));
        GUI.changed = true;
    }

    void ShiftDecoratorsDown(bool iterateCallNumbers=true){
        if (Decorators != null){
            for (int i=0; i<Decorators.Count; i++){
                if (iterateCallNumbers){
                    Decorators[i].SetCallNumber(Decorators[i].callNumber.CallNumber + 1);
                }
                Decorators[i].DragWithoutParent(new Vector2(0,taskRectSize[1]*(i+1)));       
            }
        }
    }

    void ShiftDecoratorsUp(bool iterateCallNumbers=true){
        if (Decorators != null){
            for (int i=0; i<Decorators.Count; i++){
                if (iterateCallNumbers){
                    Decorators[i].SetCallNumber(Decorators[i].callNumber.CallNumber - 1);
                }
                Decorators[i].DragWithoutParent(new Vector2(0,-taskRectSize[1]*(i+1)));       
            }
        }
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
            if (guiTimeout == null){
                genericMenu.AddItem(new GUIContent("Add timeout"), false, () => AddTimer(TimerType.Timeout));
            }
            else{
                genericMenu.AddDisabledItem(new GUIContent("Add timeout"));
            }
            if (guiCooldown == null){
                genericMenu.AddItem(new GUIContent("Add cooldown"), false, () => AddTimer(TimerType.Cooldown));
            }
            else{
                genericMenu.AddDisabledItem(new GUIContent("Add cooldown"));
            }
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }
    }

    public void AddChildConnection(Connection connection){
        if (this.ChildConnections == null){
            this.ChildConnections = new List<Connection>();
        }
        this.ChildConnections.Add(connection);
    }
    public void RemoveChildConnection(Connection connection){
        if (this.ChildConnections != null){
            this.ChildConnections.Remove(connection);
        }
    }
    public void RemoveParentConnection(){
        if (this.ParentConnection != null){
            this.ParentConnection.GetParentNode().RemoveChildConnection(this.ParentConnection);
            this.ParentConnection = null;
        }
    }

    public void RefreshChildOrder(){
        /**
        * Orders childConnections by x position
        */
        if (ChildConnections != null){
            ChildConnections.Sort((x,y) => x.GetChildNode().GetXPos().CompareTo(y.GetChildNode().GetXPos()));
        }
        List<Node> reorderedChildNodes = new List<Node>();
        foreach(Connection connection in ChildConnections){
            CompositeGuiNode childNode = connection.GetChildNode();
            if (connection.HasProbabilityWeight()){
                reorderedChildNodes.Add(connection.probabilityWeight.BtNode);
            }
            else if (childNode.Decorators != null && childNode.Decorators.Count > 0){
                // Add the top decorator of that node
                reorderedChildNodes.Add(childNode.Decorators[0].BtNode);
            }
            else{
                reorderedChildNodes.Add(childNode.BtNode);
            }
        }
        BtNode.ReplaceChildNodes(reorderedChildNodes);
    }

    private bool DecoratorKeyActive(string boolName){
        if (Decorators != null){
            for (int i=0; i < Decorators.Count; i++){
                if (Decorators[i].DisplayTask == boolName){
                    return true;
                }
            }
        }
        return false;
    }

    public void RefreshDecoratorTasks(string oldKeyName, string newKeyName){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                if (decorator.DisplayTask == oldKeyName){
                    decorator.DisplayTask = newKeyName;
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

    public void SetParentConnection(Connection connection){
        this.ParentConnection = connection;       
    }

    public void AddTimer(TimerType timerType){
        Vector2 pos;
        switch(timerType){
            case TimerType.Timeout:
                if (guiCooldown != null){
                    pos = guiCooldown.GetPos();
                    guiCooldown.DragWithoutParent(new Vector2(0, taskRectSize[1]));
                }                   
                else{
                    pos = new Vector2(
                        rect.x + initDecoratorPos[0],
                        rect.y + initDecoratorPos[1]
                    );
                }
                NodeTimer timeout = new NodeTimer(
                    timerVal:NodeProperties.DefaultTimerVal());
                this.guiTimeout = new GuiNodeTimer(
                    nodeTimer:timeout,
                    displayTask:"Timeout",
                    displayName:"",
                    pos:pos,
                    UpdatePanelDetails:UpdatePanelDetails,
                    OnRemoveTimer:OnRemoveTimer,
                    blackboard:ref blackboard,
                    parentGuiNode:this
                );
                BtNode.AddTimeout(nodeTimeout:timeout);
                break;
            case TimerType.Cooldown:
                if (guiTimeout != null){
                    pos = guiTimeout.GetPos();
                    guiTimeout.DragWithoutParent(new Vector2(0, taskRectSize[1]));
                }                   
                else{
                    pos = NodeProperties.InitDecoratorPos();
                }
                NodeTimer cooldown = new NodeTimer(
                    timerVal:NodeProperties.DefaultTimerVal());
                this.guiCooldown = new GuiNodeTimer(
                    nodeTimer:cooldown,
                    displayTask:"Cooldown",
                    displayName:"",
                    pos:pos,
                    UpdatePanelDetails:UpdatePanelDetails,
                    OnRemoveTimer:OnRemoveTimer,
                    blackboard:ref blackboard,
                    parentGuiNode:this
                );
                BtNode.AddCooldown(nodeCooldown:cooldown);
                break;
        }
                
        // Update params to make space for timer
        ShiftDecoratorsDown(iterateCallNumbers:false);
        rect.height += taskRectSize[1];
        taskRect.y += taskRectSize[1];
        callNumber.Drag(new Vector2(0, taskRectSize[1]));
        GUI.changed = true;
    }

    public void OnRemoveTimer(GuiNodeTimer guiNodeTimer){
        if (guiNodeTimer == this.guiTimeout){
            if (this.guiCooldown != null){
                if (this.guiCooldown.GetPos().y > guiNodeTimer.GetPos().y){
                    this.guiCooldown.DragWithoutParent(new Vector2(0, -taskRectSize[1]));                   
                }
            }
            ShiftDecoratorsUp(iterateCallNumbers:false);
            rect.height -= taskRectSize[1];
            taskRect.y -= taskRectSize[1];
            callNumber.Drag(new Vector2(0, -taskRectSize[1]));
            this.guiTimeout = null;
            BtNode.RemoveTimeout();
            
        }
        else if (guiNodeTimer == this.guiCooldown){
            if (this.guiTimeout != null){
                if (this.guiTimeout.GetPos().y > guiNodeTimer.GetPos().y){
                    this.guiTimeout.DragWithoutParent(new Vector2(0, -taskRectSize[1]));                   
                }
            }
            ShiftDecoratorsUp(iterateCallNumbers:false);
            rect.height -= taskRectSize[1];
            taskRect.y -= taskRectSize[1];
            callNumber.Drag(new Vector2(0, -taskRectSize[1]));
            this.guiCooldown = null;
            BtNode.RemoveCooldown();
        }
    }
}
}