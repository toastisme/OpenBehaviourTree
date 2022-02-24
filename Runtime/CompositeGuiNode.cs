using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Behaviour{
public class CompositeGuiNode : CallableGuiNode
{
    /**
     * \class Behaviour.CompositeGuiNode
     * CallableGuiNodes made up of several GuiNodes 
     * (i.e they hold a list of GuiDecorators),
     * and have connections to other CompositeGuiNodes.
     */
     
    // GuiNodes
    public List<GuiDecorator> Decorators{get; set;}
    bool hasTimeout = false;
    bool hasCooldown = false;

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
    protected Rect scaledTaskRect; // taskRect scaled to the current zoom level
    protected Color taskRectColor;
    protected Vector2 taskRectSize = BehaviourTreeProperties.TaskNodeSize(); // Size of rect showing the node task
    protected Vector2 subRectSize = BehaviourTreeProperties.SubNodeSize(); // Size of GuiDecorators
    private Vector2 initDecoratorPos = BehaviourTreeProperties.InitDecoratorPos();

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
        scaledTaskRect = new Rect(
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
        NodeUpdated();
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
        /**
         * Allows for decoupling editor actions being set from the 
         * constructor (required e.g when loading from disk in BehaviourTreeLoader)
         */ 

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

    }

    protected override void ApplyDerivedSettings(){
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        defaultTaskStyle = BehaviourTreeProperties.TaskNodeStyle();
        selectedTaskStyle = BehaviourTreeProperties.SelectedTaskNodeStyle();
        activeTaskStyle = defaultTaskStyle;
        color = BehaviourTreeProperties.DefaultColor();
        taskRectColor = BehaviourTreeProperties.DefaultColor();

    }

    protected virtual void ApplyNodeTypeSettings(
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){

        // Default CompositeGuiNode has a child and parent point
        ChildPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.In, 
                                            BehaviourTreeProperties.ChildPointStyle(), 
                                            OnClickChildPoint);
        ParentPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.Out, 
                                            BehaviourTreeProperties.ParentPointStyle(), 
                                            OnClickParentPoint);

    }

    public override void UpdateOrigin(Vector2 origin)
    {
        base.UpdateOrigin(origin);
        scaledTaskRect = new Rect(taskRect.x, taskRect.y, taskRect.width, taskRect.height);
        scaledTaskRect.position -= origin;
        ChildPoint?.UpdateOrigin(origin);
        ParentPoint?.UpdateOrigin(origin);
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
        GUI.color = BehaviourTreeProperties.DefaultTint();
        if (IsRunning()){
            GUI.color = BehaviourTreeProperties.RunningTint();
        }
        else if(HasFailed()){
            GUI.color = BehaviourTreeProperties.FailedTint();
        }
        DrawSelf();
        callNumber.Draw();
        DrawDecorators();
        GUI.backgroundColor = currentColor;
        GUI.color = BehaviourTreeProperties.DefaultTint();
    }

    protected virtual void DrawSelf(){
        if (IsSelected){
            GUI.color = BehaviourTreeProperties.SelectedTint();
        }
        GUI.backgroundColor = color;
        GUI.Box(scaledRect, "", activeStyle);
        GUI.backgroundColor = taskRectColor; 
        iconAndText.text ="\n" + DisplayName + "\n" + DisplayTask;
        GUI.Box(scaledTaskRect, iconAndText, activeTaskStyle);
        if (IsSelected){
            GUI.color = BehaviourTreeProperties.DefaultTint();
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
        return guiChanged;
    }

    public virtual bool ProcessTaskRectEvents(Event e, Vector2 mousePos){
        bool guiChanged = false;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (scaledRect.Contains(mousePos))
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

                if (e.button == 1 && scaledRect.Contains(mousePos))
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

        decorator.BtNode.Unlink(true);
        decorator.SetSelected(false);

        int idx = Decorators.FindIndex(a => a==decorator);
        if (idx != -1){

            if (decorator is GuiCooldownNode){
                hasCooldown = false;
            }
            else if (decorator is GuiTimeoutNode){
                hasTimeout = false;
            }

            Decorators.Remove(decorator);

            // Resize node 
            rect.height -= subRectSize[1];
            taskRect.y -= subRectSize[1];
            callNumber.Drag(new Vector2(0, -subRectSize[1]));

            Vector2 moveVec = new Vector2(0, -subRectSize[1]);
            if (callNumber.CallNumber != -1){
                SetCallNumber(callNumber.CallNumber-1);

                // Move all decorators below the removed one up
                for (int i = idx; i < Decorators.Count; i++){
                    Decorators[i].DragWithoutParent(moveVec);
                    Decorators[i].SetCallNumber(Decorators[i].callNumber.CallNumber-1);
                }
            }
            else{
                // Move all decorators below the removed one up
                for (int i = idx; i < Decorators.Count; i++){
                    Decorators[i].DragWithoutParent(moveVec);
                }
            }
            GUI.changed = true;
            TreeModified();
            NodeUpdated();
        }
        else{
            throw new Exception("Decorator not found in decorators list.");
        }

    }

    public void AddDecorator(GuiDecorator guiDecorator){
        guiDecorator.SetParentGuiNode(this);
        guiDecorator.NodeUpdated = this.NodeUpdated;
        if (callNumber.CallNumber != -1){
            if (Decorators != null && Decorators.Count >0){
                guiDecorator.SetCallNumber(Decorators[0].callNumber.CallNumber);
            }
            else{
                guiDecorator.SetCallNumber(this.callNumber.CallNumber);
            }
            callNumber.CallNumber++;
        }
        // Make room for new decorator
        Decorators.Insert(0, guiDecorator);
                
        // Update params to make space for guiDecorator
        rect.height += subRectSize[1];
        taskRect.y += subRectSize[1];
        callNumber.Drag(new Vector2(0, subRectSize[1]));
        GUI.changed = true;

        // Null check needed here as when loading from disk decorators are loaded
        // before SetEditorActions is called
        if (TreeModified != null){
            TreeModified();
        }
        NodeUpdated();

    }

    protected void OnClickAddDecorator(string displayTask){

        // The underlying decorator
        BoolDecorator decorator = new BoolDecorator(
            taskName:displayTask,
            blackboard:ref blackboard
        );

        if (Decorators != null && Decorators.Count >0){
            // Insert as parent of top decorator
            Decorators[0].BtNode.InsertBeforeSelf(decorator);
        }
        else{
            // Insert as parent of self
            this.BtNode.InsertBeforeSelf(decorator);
        }


        Vector2 pos = new Vector2(
            rect.x + initDecoratorPos[0],
            rect.y + initDecoratorPos[1]
        );

        // Add gui decorator
        GuiBoolDecorator guiDecorator = new GuiBoolDecorator( 
            decorator:decorator,
            displayTask:displayTask,
            displayName:"",
            pos:pos,
            UpdatePanelDetails:UpdatePanelDetails,
            TreeModified:TreeModified,
            NodeUpdated:NodeUpdated,
            OnRemoveDecorator:OnClickRemoveDecorator,
            blackboard:ref blackboard,
            parentGuiNode:this
            );

        
        if (callNumber.CallNumber != -1){
            if (Decorators != null && Decorators.Count >0){
                guiDecorator.SetCallNumber(Decorators[0].callNumber.CallNumber);
            }
            else{
                guiDecorator.SetCallNumber(this.callNumber.CallNumber);
            }
            callNumber.CallNumber++;
            ShiftDecoratorsDown(true);
        }
        else{
            ShiftDecoratorsDown(false);
        }

        // Make room for new decorator
        Decorators.Insert(0, guiDecorator);
                
        // Update params to make space for gui decorator
        rect.height += subRectSize[1];
        taskRect.y += subRectSize[1];
        callNumber.Drag(new Vector2(0, subRectSize[1]));
        GUI.changed = true;
        TreeModified();
        NodeUpdated();
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
            genericMenu.AddDisabledItem(
                new GUIContent(
                    "Add a blackboard asset to the behaviour tree to add conditional decorators"
                )
            );
        }
        else{
            Dictionary<string, bool> boolKeys = blackboard.GetBoolKeys();
            if (boolKeys == null || boolKeys.Count == 0){
                genericMenu.AddDisabledItem(
                    new GUIContent(
                        "Add blackboard bool keys to use as decorators"
                    )
                );
            }
            else{            
                foreach(string boolName in blackboard.GetBoolKeys().Keys){
                    if (!DecoratorKeyActive(boolName)){
                        genericMenu.AddItem(
                            new GUIContent(
                                "Add Decorator/" + boolName
                            ), 
                            false, 
                            () => OnClickAddDecorator(boolName)
                        );
                    }
                    else{
                        genericMenu.AddDisabledItem(new GUIContent("Add Decorator/" + boolName));
                    }
                }
            }
        }
        if (!hasTimeout){
            genericMenu.AddItem(new GUIContent("Add timeout"), false, () => OnClickAddTimeout());
        }
        else{
            genericMenu.AddDisabledItem(new GUIContent("Add timeout"));
        }
        if (!hasCooldown){
            genericMenu.AddItem(new GUIContent("Add cooldown"), false, () => OnClickAddCooldown());
        }
        else{
            genericMenu.AddDisabledItem(new GUIContent("Add cooldown"));
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
        callNumber.CallNumber = -1;
        
    }

    public void RefreshChildOrder(){

        /**
        * Orders ChildConnections by x position
        * (Used to ensure the nodes are evaluated from left to right as seen in the editor)
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

        /**
         * Checks to see if a decorator is using boolName
         */

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

        /**
         * Updates any decorators using oldKeyName with newKeyName
         */

        if (Decorators != null){
            foreach(GuiDecorator decorator in Decorators){
                if (decorator.DisplayTask == oldKeyName){
                    decorator.DisplayTask = newKeyName;
                    TreeModified();
                    NodeUpdated();
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

    protected void OnClickAddTimeout(){

        // The underlying node
        TimeoutNode decorator = new TimeoutNode(
            blackboard:ref blackboard,
            timerValue:BehaviourTreeProperties.DefaultTimerVal(),
            randomDeviation:BehaviourTreeProperties.DefaultRandomDeviationVal()
        );

        if (Decorators != null && Decorators.Count >0){
            // Insert as parent of top decorator
            Decorators[0].BtNode.InsertBeforeSelf(decorator);
        }
        else{
            // Insert as parent of self
            this.BtNode.InsertBeforeSelf(decorator);
        }

        Vector2 pos = new Vector2(
            rect.x + initDecoratorPos[0],
            rect.y + initDecoratorPos[1]
        );

        // Add gui decorator
        GuiTimeoutNode guiDecorator = new GuiTimeoutNode( 
            timerNode:decorator,
            displayName:"",
            pos:pos,
            UpdatePanelDetails:UpdatePanelDetails,
            TreeModified:TreeModified,
            NodeUpdated:NodeUpdated,
            OnRemoveDecorator:OnClickRemoveDecorator,
            blackboard:ref blackboard,
            parentGuiNode:this
            );

        if (callNumber.CallNumber != -1){
            if (Decorators != null && Decorators.Count >0){
                guiDecorator.SetCallNumber(Decorators[0].callNumber.CallNumber);
            }
            else{
                guiDecorator.SetCallNumber(this.callNumber.CallNumber);
            }
            callNumber.CallNumber++;
            ShiftDecoratorsDown(true);
        }
        else{
            ShiftDecoratorsDown(false);
        }

        // Make room for new decorator
        Decorators.Insert(0, guiDecorator);
                
        // Update params to make space for gui decorator
        rect.height += subRectSize[1];
        taskRect.y += subRectSize[1];
        callNumber.Drag(new Vector2(0, subRectSize[1]));

        hasTimeout = true;
        GUI.changed = true;
        TreeModified();
        NodeUpdated();
    }

    protected void OnClickAddCooldown(){

        // The underlying node
        CooldownNode decorator = new CooldownNode(
            blackboard:ref blackboard,
            timerValue:BehaviourTreeProperties.DefaultTimerVal(),
            randomDeviation:BehaviourTreeProperties.DefaultRandomDeviationVal()
        );

        if (Decorators != null && Decorators.Count >0){
            // Insert as parent of top decorator
            Decorators[0].BtNode.InsertBeforeSelf(decorator);
        }
        else{
            // Insert as parent of self
            this.BtNode.InsertBeforeSelf(decorator);
        }

        Vector2 pos = new Vector2(
            rect.x + initDecoratorPos[0],
            rect.y + initDecoratorPos[1]
        );

        // Add gui decorator
        GuiCooldownNode guiDecorator = new GuiCooldownNode( 
            timerNode:decorator,
            displayName:"",
            pos:pos,
            UpdatePanelDetails:UpdatePanelDetails,
            TreeModified:TreeModified,
            NodeUpdated:NodeUpdated,
            OnRemoveDecorator:OnClickRemoveDecorator,
            blackboard:ref blackboard,
            parentGuiNode:this
            );

        if (callNumber.CallNumber != -1){
            if (Decorators != null && Decorators.Count >0){
                guiDecorator.SetCallNumber(Decorators[0].callNumber.CallNumber);
            }
            else{
                guiDecorator.SetCallNumber(this.callNumber.CallNumber);
            }
            callNumber.CallNumber++;
            // Make room for new decorator
            ShiftDecoratorsDown(true);
        }
        else{
            ShiftDecoratorsDown(false);
        }

        Decorators.Insert(0, guiDecorator);
                
        // Update params to make space for gui decorator
        rect.height += subRectSize[1];
        taskRect.y += subRectSize[1];
        callNumber.Drag(new Vector2(0, subRectSize[1]));

        hasCooldown = true;
        GUI.changed = true;
        TreeModified();
        NodeUpdated();
    }

    public Rect GetScaledRect(){return scaledRect;}

    public override void UpdateBlackboard(ref BehaviourTreeBlackboard newBlackboard){

        base.UpdateBlackboard(ref newBlackboard);
        if (Decorators != null){
            for (int i=0; i<Decorators.Count; i++){
                Decorators[i].UpdateBlackboard(ref newBlackboard);
            }
        }
    }

    public void NodeUpdated(){
        UpdateBoxWidth(GetRequiredBoxWidth());        
    }

    public override float GetRequiredBoxWidth(){

        /**
         * A dirty way to approximate the width rect needs to
         * fit the GuiNode text.
         */

        float dx = BehaviourTreeProperties.ApproximateNodeTextWidth();
        float minWidth = BehaviourTreeProperties.GuiNodeSize().x;
        float length = Mathf.Max(DisplayName.Length, DisplayTask.Length)*dx;
        float maxWidth =  Mathf.Max(minWidth, length);

        if (Decorators != null){
            for(int i=0; i<Decorators.Count; i++){
                float boxWidth = Decorators[i].GetRequiredBoxWidth();
                if (maxWidth < boxWidth){
                    maxWidth = boxWidth;
                }
            }
        }

        if (ParentConnection!= null && ParentConnection.HasProbabilityWeight()){
            float pwWidth = ParentConnection.GetRequiredProbabilityWeightBoxWidth();
            if (maxWidth < pwWidth){
                maxWidth = pwWidth;
            }
        }

        return maxWidth;
    }

    public override void UpdateBoxWidth(float newWidth){
        rect.width = newWidth;
        taskRect.width = newWidth;
        SetCallNumberXPos(taskRect.x + newWidth);
        if (Decorators != null){
            for(int i=0; i<Decorators.Count; i++){
                Decorators[i].UpdateBoxWidth(newWidth);
            }
        }
        ChildPoint?.UpdateBoxWidth(newWidth);
        ParentPoint?.UpdateBoxWidth(newWidth);
        ParentConnection?.UpdateProbabilityWeightBoxWidth(newWidth);
    }

}
}