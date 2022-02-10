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
    public GuiNodeTimer GuiTimeout{get; set;}
    public GuiNodeTimer GuiCooldown{get; set;}

    Action<CompositeGuiNode> OnRemoveNode;

    // Connection points
    public ConnectionPoint ChildPoint{get; set;}
    public ConnectionPoint ParentPoint{get; set;}

    // Connections
    public Connection ParentConnection{get; set;}
    public List<Connection> ChildConnections{get; set;}

    // Appearance
    protected GUIStyle defaultTaskStyle;
    protected GUIStyle selectedTaskStyle;
    protected GUIStyle activeTaskStyle;

    protected Rect taskRect; // Rect containing the task and display name
    protected Rect apparentTaskRect; // Rect containing the task and display name
    protected Color taskRectColor;
    protected Vector2 taskRectSize = NodeProperties.TaskNodeSize();
    protected Vector2 subRectSize = NodeProperties.SubNodeSize();
    private Vector2 initDecoratorPos = NodeProperties.InitDecoratorPos();

    public CompositeGuiNode(
        Node node,
        string displayTask,
        string displayName,
        Vector2 pos,
        Connection parentConnection,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
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
        TreeModified:TreeModified,
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
        apparentTaskRect = new Rect(
            rect.x,
            rect.y+taskRectSize.y*.5f,
            taskRectSize.x,
            taskRectSize.y
        );
        SetDefaultCallNumberPos();
        ApplyDerivedSettings();
        ApplyNodeTypeSettings(
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint
            );
    }

    public override void SetDefaultCallNumberPos()
    {
        Vector2 callNumberPos = taskRect.position;
        callNumberPos += new Vector2(taskRect.size.x, 0);
        callNumberPos -= new Vector2(callNumber.GetRect().width, 0);
        callNumber.SetPosition(callNumberPos);
    }
    
    public void SetEditorActions(
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action<CompositeGuiNode> OnRemoveNode,
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.TreeModified = TreeModified;
        this.OnRemoveNode = OnRemoveNode;
        ApplyNodeTypeSettings(
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint
        );
        foreach(GuiDecorator decorator in Decorators){
            decorator.SetEditorActions(
                UpdatePanelDetails:UpdatePanelDetails,
                TreeModified:TreeModified,
                OnRemoveDecorator:OnClickRemoveDecorator
            );
        }

        GuiCooldown?.SetEditorActions(
            UpdatePanelDetails:UpdatePanelDetails,
            TreeModified:TreeModified,
            OnRemoveTimer:OnRemoveTimer
        );

        GuiTimeout?.SetEditorActions(
            UpdatePanelDetails:UpdatePanelDetails,
            TreeModified:TreeModified,
            OnRemoveTimer:OnRemoveTimer
        );

    }

    protected override void ApplyDerivedSettings(){
        defaultStyle = NodeProperties.GUINodeStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        defaultTaskStyle = NodeProperties.TaskNodeStyle();
        selectedTaskStyle = NodeProperties.SelectedTaskNodeStyle();
        activeTaskStyle = defaultTaskStyle;
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

    public override void UpdateOrigin(Vector2 origin)
    {
        base.UpdateOrigin(origin);
        apparentTaskRect = new Rect(taskRect.x, taskRect.y, taskRect.width, taskRect.height);
        apparentTaskRect.position -= origin;
        ChildPoint?.UpdateOrigin(origin);
        ParentPoint?.UpdateOrigin(origin);
        GuiTimeout?.UpdateOrigin(origin);
        GuiCooldown?.UpdateOrigin(origin);
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                decorator.UpdateOrigin(origin);
            }
        }
    }

    public override void Drag(Vector2 delta){
        base.Drag(delta);
        taskRect.position += delta;
        DragDecorators(delta);
    }

    void DragDecorators(Vector2 delta){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                decorator.DragWithoutParent(delta);
            }
        }
        GuiTimeout?.DragWithoutParent(delta);
        GuiCooldown?.DragWithoutParent(delta);
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
        GUI.color = NodeProperties.DefaultTint();
        if (IsRunning()){
            GUI.color = NodeProperties.RunningTint();
        }
        else if(HasFailed()){
            GUI.color = NodeProperties.FailedTint();
        }
        DrawSelf();
        callNumber.Draw();
        DrawDecorators();
        GuiTimeout?.Draw();
        GuiCooldown?.Draw();
        GUI.backgroundColor = currentColor;
        GUI.color = NodeProperties.DefaultTint();
    }

    protected virtual void DrawSelf(){
        if (IsSelected){
            GUI.color = NodeProperties.SelectedTint();
        }
        GUI.backgroundColor = color;
        GUI.Box(apparentRect, "", activeStyle);
        GUI.backgroundColor = taskRectColor; 
        iconAndText.text ="\n" + DisplayName + "\n" + DisplayTask;
        GUI.Box(apparentTaskRect, iconAndText, activeTaskStyle);
        if (IsSelected){
            GUI.color = NodeProperties.DefaultTint();
        }
    }

    void DrawDecorators(){
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                decorator.Draw();
            }
        }
    }

    public override bool ProcessEvents(Event e, Vector2 mousePos){
        bool guiChanged = false;
        bool decoratorSelected = false;
        bool timerSelected = false;

        guiChanged = ProcessDecoratorEvents(e, mousePos, out decoratorSelected, out timerSelected);
        // Only bother processing taskRect events if no decorator was selected
        if (!decoratorSelected && !timerSelected){
            bool guiChangedFromNode =  ProcessTaskRectEvents(e, mousePos);
            if (guiChangedFromNode){
                guiChanged = true;
            }
        }
        else{
            SetSelected(false);
        }
        return guiChanged;
    }

    public override void SetSelected(bool selected)
    {
        base.SetSelected(selected);
        if (selected){
            IsSelected = true;
            activeStyle = selectedStyle;
            activeTaskStyle = selectedTaskStyle;
        }
        else{
            IsSelected = false;
            activeStyle = defaultStyle;
            activeTaskStyle = defaultTaskStyle;
        }
    }

    bool ProcessDecoratorEvents(Event e, Vector2 mousePos, out bool decoratorSelected, out bool timerSelected){
        bool guiChanged = false;
        decoratorSelected = false;
        timerSelected = false;
        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                bool guiChangedFromDecorator = decorator.ProcessEvents(e, mousePos);
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
            if (GuiTimeout != null){
                changedFromTimers = GuiTimeout.ProcessEvents(e, mousePos);
                if (!timerSelected && GuiTimeout.IsSelected){
                    timerSelected = true;
                }
            }
            if (!timerSelected && GuiCooldown != null){
                changedFromTimers = GuiCooldown.ProcessEvents(e, mousePos);
                if (!timerSelected && GuiCooldown.IsSelected){
                    timerSelected = true;
                }
            }
            if (!guiChanged && changedFromTimers){
                guiChanged = true;
            }
        }
        return guiChanged;
    }

    public virtual bool ProcessTaskRectEvents(Event e, Vector2 mousePos){
        bool guiChanged = false;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (apparentRect.Contains(mousePos))
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

                if (e.button == 1 && apparentRect.Contains(mousePos))
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
            rect.height -= subRectSize[1];
            taskRect.y -= subRectSize[1];
            callNumber.Drag(new Vector2(0, -subRectSize[1]));
            SetCallNumber(callNumber.CallNumber-1);

            // Move all decorators below the removed one up
            Vector2 moveVec = new Vector2(0, -subRectSize[1]);
            for (int i = idx; i < Decorators.Count; i++){
                Decorators[i].DragWithoutParent(moveVec);
                Decorators[i].SetCallNumber(Decorators[i].callNumber.CallNumber-1);
            }
            GUI.changed = true;
            TreeModified();
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
        rect.height += subRectSize[1];
        taskRect.y += subRectSize[1];
        callNumber.Drag(new Vector2(0, subRectSize[1]));
        GUI.changed = true;
        TreeModified();

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
        if (GuiTimeout != null){
            pos += new Vector2(0, subRectSize[1]); 
        }
        if (GuiCooldown != null){
            pos += new Vector2(0, subRectSize[1]); 
        }
        // Add gui decorator
        GuiDecorator guiDecorator = new GuiDecorator( 
            decorator:decorator,
            displayTask:displayTask,
            displayName:"",
            pos:pos,
            UpdatePanelDetails:UpdatePanelDetails,
            TreeModified:TreeModified,
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
        rect.height += subRectSize[1];
        taskRect.y += subRectSize[1];
        callNumber.Drag(new Vector2(0, subRectSize[1]));
        GUI.changed = true;
        TreeModified();
    }

    void ShiftDecoratorsDown(bool iterateCallNumbers=true){
        if (Decorators != null){
            for (int i=0; i<Decorators.Count; i++){
                if (iterateCallNumbers){
                    Decorators[i].SetCallNumber(Decorators[i].callNumber.CallNumber + 1);
                }
                Decorators[i].DragWithoutParent(new Vector2(0,subRectSize[1]));       
            }
        }
    }

    void ShiftDecoratorsUp(bool iterateCallNumbers=true){
        if (Decorators != null){
            for (int i=0; i<Decorators.Count; i++){
                if (iterateCallNumbers){
                    Decorators[i].SetCallNumber(Decorators[i].callNumber.CallNumber - 1);
                }
                Decorators[i].DragWithoutParent(new Vector2(0,-subRectSize[1]));       
            }
        }
    }

    protected virtual void ProcessContextMenu()
    {
        if (IsRootNode()){
            return;
        }
        GenericMenu genericMenu = new GenericMenu();
        if (blackboard == null){
            genericMenu.AddDisabledItem(new GUIContent("Add a blackboard asset to the behaviour tree to add conditional decorators"));
        }
        else{
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
            if (GuiTimeout == null){
                genericMenu.AddItem(new GUIContent("Add timeout"), false, () => AddTimer(TimerType.Timeout));
            }
            else{
                genericMenu.AddDisabledItem(new GUIContent("Add timeout"));
            }
            if (GuiCooldown == null){
                genericMenu.AddItem(new GUIContent("Add cooldown"), false, () => AddTimer(TimerType.Cooldown));
            }
            else{
                genericMenu.AddDisabledItem(new GUIContent("Add cooldown"));
            }
        }
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
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

    protected bool DecoratorKeyActive(string boolName){
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
                    TreeModified();
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

    public bool HasParentConnection(){
        return this.ParentConnection != null;
    }

    public void AddExistingTimer(TimerType timerType){
        Vector2 pos;

        switch(timerType){
            case TimerType.Timeout:
                if (!BtNode.HasTimeout()){
                    throw new Exception("Trying to add timeout that does not exist.");
                }
                if (GuiCooldown != null){
                    pos = GuiCooldown.GetPos();
                    GuiCooldown.DragWithoutParent(new Vector2(0, subRectSize[1]));
                }                   
                else{
                    pos = new Vector2(
                        rect.x + initDecoratorPos[0],
                        rect.y + initDecoratorPos[1]
                    );
                }
                this.GuiTimeout = new GuiNodeTimer(
                    nodeTimer:BtNode.GetTimeout(),
                    displayTask:"Timeout",
                    displayName:"",
                    pos:pos,
                    UpdatePanelDetails:UpdatePanelDetails,
                    TreeModified:TreeModified,
                    OnRemoveTimer:OnRemoveTimer,
                    blackboard:ref blackboard,
                    parentGuiNode:this
                );
                break;
            case TimerType.Cooldown:
                if (!BtNode.HasCooldown()){
                    throw new Exception("Trying to add cooldown that does not exist.");
                }
                if (GuiTimeout != null){
                    pos = GuiTimeout.GetPos();
                    GuiTimeout.DragWithoutParent(new Vector2(0, subRectSize[1]));
                }                   
                else{
                    pos = new Vector2(
                        rect.x + initDecoratorPos[0],
                        rect.y + initDecoratorPos[1]
                    );
                }
                this.GuiCooldown = new GuiNodeTimer(
                    nodeTimer:BtNode.GetCooldown(),
                    displayTask:"Cooldown",
                    displayName:"",
                    pos:pos,
                    UpdatePanelDetails:UpdatePanelDetails,
                    TreeModified:TreeModified,
                    OnRemoveTimer:OnRemoveTimer,
                    blackboard:ref blackboard,
                    parentGuiNode:this
                );
                break;
        }
        // Update params to make space for timer
        ShiftDecoratorsDown(iterateCallNumbers:false);
        rect.height += subRectSize[1];
        taskRect.y += subRectSize[1];
        callNumber.Drag(new Vector2(0, subRectSize[1]));
        GUI.changed = true;

    }

    public void AddTimer(TimerType timerType, float timerVal=-1){

        Vector2 pos;
        if (timerVal < 0){timerVal = NodeProperties.DefaultTimerVal();}

        switch(timerType){
            case TimerType.Timeout:
                if (GuiCooldown != null){
                    pos = GuiCooldown.GetPos();
                    GuiCooldown.DragWithoutParent(new Vector2(0, subRectSize[1]));
                }                   
                else{
                    pos = new Vector2(
                        rect.x + initDecoratorPos[0],
                        rect.y + initDecoratorPos[1]
                    );
                }
                NodeTimer timeout = new NodeTimer(
                    timerVal:timerVal);
                this.GuiTimeout = new GuiNodeTimer(
                    nodeTimer:timeout,
                    displayTask:"Timeout",
                    displayName:"",
                    pos:pos,
                    UpdatePanelDetails:UpdatePanelDetails,
                    TreeModified:TreeModified,
                    OnRemoveTimer:OnRemoveTimer,
                    blackboard:ref blackboard,
                    parentGuiNode:this
                );
                BtNode.AddTimeout(nodeTimeout:timeout);
                break;
            case TimerType.Cooldown:
                if (GuiTimeout != null){
                    pos = GuiTimeout.GetPos();
                    GuiTimeout.DragWithoutParent(new Vector2(0, subRectSize[1]));
                }                   
                else{
                    pos = new Vector2(
                        rect.x + initDecoratorPos[0],
                        rect.y + initDecoratorPos[1]
                    );
                }
                NodeTimer cooldown = new NodeTimer(
                    timerVal:timerVal);
                this.GuiCooldown = new GuiNodeTimer(
                    nodeTimer:cooldown,
                    displayTask:"Cooldown",
                    displayName:"",
                    pos:pos,
                    UpdatePanelDetails:UpdatePanelDetails,
                    TreeModified:TreeModified,
                    OnRemoveTimer:OnRemoveTimer,
                    blackboard:ref blackboard,
                    parentGuiNode:this
                );
                BtNode.AddCooldown(nodeCooldown:cooldown);
                break;
        }
                
        // Update params to make space for timer
        ShiftDecoratorsDown(iterateCallNumbers:false);
        rect.height += subRectSize[1];
        taskRect.y += subRectSize[1];
        callNumber.Drag(new Vector2(0, subRectSize[1]));
        GUI.changed = true;
        TreeModified();
    }

    public void OnRemoveTimer(GuiNodeTimer guiNodeTimer){
        if (guiNodeTimer == this.GuiTimeout){
            if (this.GuiCooldown != null){
                if (this.GuiCooldown.GetPos().y > guiNodeTimer.GetPos().y){
                    this.GuiCooldown.DragWithoutParent(new Vector2(0, -subRectSize[1]));                   
                }
            }
            ShiftDecoratorsUp(iterateCallNumbers:false);
            rect.height -= subRectSize[1];
            taskRect.y -= subRectSize[1];
            callNumber.Drag(new Vector2(0, -subRectSize[1]));
            this.GuiTimeout.SetSelected(false);
            this.GuiTimeout = null;
            BtNode.RemoveTimeout();
            
        }
        else if (guiNodeTimer == this.GuiCooldown){
            if (this.GuiTimeout != null){
                if (this.GuiTimeout.GetPos().y > guiNodeTimer.GetPos().y){
                    this.GuiTimeout.DragWithoutParent(new Vector2(0, -subRectSize[1]));                   
                }
            }
            ShiftDecoratorsUp(iterateCallNumbers:false);
            rect.height -= subRectSize[1];
            taskRect.y -= subRectSize[1];
            callNumber.Drag(new Vector2(0, -subRectSize[1]));
            this.GuiCooldown.SetSelected(false);
            this.GuiCooldown = null;
            BtNode.RemoveCooldown();
        }
        GUI.changed = true;
        TreeModified();
    }

    public Rect GetApparentRect(){return apparentRect;}

}
}