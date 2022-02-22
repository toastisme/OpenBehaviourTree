using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Behaviour{

public enum BehaviourTreeEditorMode{
    Editor,
    Runtime,
}

public class BehaviourTreeEditor : EditorWindow
{
    /**
    * \class BehaviourTreeEditor
    * Class to edit and visualise BehaviourTree assets.
    */


    BehaviourTreeEditorMode mode;

    // Bookkeeping
    private bool saved;
    private bool activeBlackboard;

    // Hack to avoid it showing unsaved changes when loading
    private bool loadingTree; 
    public BehaviourTree bt;
    public List<CompositeGuiNode> guiNodes;
    public List<Connection> connections;
    private GuiNode selectedNode;
    private ConnectionPoint selectedChildPoint;
    private ConnectionPoint selectedParentPoint;
    List<string> customTaskNames;

    // Movement
    private Vector2 offset;
    private Vector2 drag;
    private bool drawingLine;

    private Rect mainWindow;

    // Details panel
    private Rect detailsPanel;
    private GUIContent[] toolbarButtons;
    private enum ToolbarTab{
        SelectedNode = 0,
        Blackboard = 1
    }
    private int toolbarInt;

    // Shorthand
    List<NodeType> decisionNodeTypes;

    // Zoom
    private const float zoomMin = 0.5f;
    private const float zoomMax = 1.5f;
    private float currentZoom = 1.0f;
    private Vector2 zoomCoordsOrigin = Vector2.zero;

    private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
    {
        return (screenCoords - mainWindow.TopLeft()) / currentZoom + zoomCoordsOrigin;
    }

    // Blackboard
    string[] activeBlackboardKey = {"",""};
    bool renamingBlackboardKey = false;
    Dictionary<string, GUIStyle> blackboardTypeStyles;

    // GUIContent for buttons
    GUIContent blackboardButton;
    GUIContent addKeyButton; 
    GUIContent clearTreeButton;
    GUIContent saveButton;
    GUIContent selectedNodeButton;

    public void SetScriptableObject(
        BehaviourTree behaviourTree,
        BehaviourTreeEditorMode mode=BehaviourTreeEditorMode.Editor){
        bt = behaviourTree;
        guiNodes = new List<CompositeGuiNode>();
        connections = new List<Connection>();
        if (bt.rootNode == null){
            AddRootNode();
        }
        else{
            loadingTree=true;
            LoadFromRoot(
                rootNode:bt.rootNode,
                nodeMetaData:bt.nodeMetaData
            );
            loadingTree=false;
        }
        bt.guiRootNode = guiNodes[0];
        this.mode = mode;
        saved = true;

        activeBlackboard = bt.blackboard != null? true : false;
    }

    private void OnEnable()
    {
        customTaskNames = GetCustomTaskNames();
        LoadButtons();
        LoadBlackboardTypeStyles();
        LoadDecisionNodeTypes();
        SetPanelLayout();
    }

    private void SetPanelLayout(){
        detailsPanel = new Rect(position.width*.8f, 
                                    0, 
                                    position.width*.2f, 
                                    position.height);

        mainWindow = new Rect(
            0,
            0,
            position.width*.8f,
            position.height
        );
    }

    private void LoadDecisionNodeTypes(){
        decisionNodeTypes = new List<NodeType>{NodeType.PrioritySelector, 
                                            NodeType.ProbabilitySelector,
                                            NodeType.SequenceSelector};
    }

    private void LoadButtons(){

        blackboardButton = BehaviourTreeProperties.BlackboardContent();
        addKeyButton = BehaviourTreeProperties.AddKeyContent();
        clearTreeButton = BehaviourTreeProperties.ClearTreeContent();
        saveButton = BehaviourTreeProperties.SaveContent();
        selectedNodeButton = BehaviourTreeProperties.SelectedNodeContent();
        toolbarButtons = new GUIContent[]{selectedNodeButton, blackboardButton};
    }

    private void LoadBlackboardTypeStyles(){
        blackboardTypeStyles = new Dictionary<string, GUIStyle>();
        blackboardTypeStyles["int"] = BehaviourTreeProperties.BlackboardIntStyle();
        blackboardTypeStyles["float"] = BehaviourTreeProperties.BlackboardFloatStyle();
        blackboardTypeStyles["bool"] = BehaviourTreeProperties.BlackboardBoolStyle();
        blackboardTypeStyles["string"] = BehaviourTreeProperties.BlackboardStringStyle();
        blackboardTypeStyles["GameObject"] = BehaviourTreeProperties.BlackboardGameObjectStyle();
        blackboardTypeStyles["Vector3"] = BehaviourTreeProperties.BlackboardVector3Style();
        blackboardTypeStyles["Vector2"] = BehaviourTreeProperties.BlackboardVector2Style();
    }

    private void AddRootNode(){

        if (bt.rootNode == null){
            bt.rootNode = new PrioritySelector(
                taskName:"Root"
            );
        }
        guiNodes.Add(new GuiRootNode(
                            node:bt.rootNode,
                            displayTask:"Root",
                            displayName:"",
                            Vector2.zero,
                            null,
                            UpdatePanelDetails,
                            TreeModified,
                            OnClickRemoveNode,
                            OnClickChildPoint, 
                            OnClickParentPoint, 
                            ref bt.blackboard
                            ));
        guiNodes[0].callNumber.CallNumber = 1;
    }

    private void ResetRootNode(){
        guiNodes[0].DisplayName = "";
        guiNodes[0].SetPosition(new Vector2(0,0));
    }

    private void OnGUI()
    {
        // Scale event delta with zoom level
        Event.current.delta /= currentZoom;

        SetPanelLayout();
        DrawStaticComponents();
        DrawDynamicComponents();
        UpdateBlackboard();

        if (mode == BehaviourTreeEditorMode.Editor){
            if (MousePosOnGrid(Event.current.mousePosition)){
                ProcessNodeEvents(Event.current);
            }
            ProcessEvents(Event.current);
        }

        else if (mode == BehaviourTreeEditorMode.Runtime){
            ProcessEventsRuntime(Event.current);
        }

        if (GUI.changed) {
            Repaint();
            if (guiNodes != null){
                UpdateCallNumbers(guiNodes[0], 1);
            }
        }

    }


    private void DrawDynamicComponents(){

        /**
            * Draws components affected by zoom controls
            */ 

        EditorZoomArea.Begin(currentZoom, mainWindow);
        UpdateOrigin(zoomCoordsOrigin);

        if (bt != null && guiNodes != null ){

            DrawConnections();

            if (mode == BehaviourTreeEditorMode.Editor){
                DrawConnectionLine(Event.current);
                DrawNodes();
            }

            else if (mode == BehaviourTreeEditorMode.Runtime){
                DrawNodes();
            }

        }
        EditorZoomArea.End();

    }

    private void DrawStaticComponents(){

        /**
            * Draws components indepdent of zoom controls
            */

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        if (mode == BehaviourTreeEditorMode.Editor){
            DrawDetailsPanel();
        }

    }

    public void UpdateOrigin(Vector2 origin){

        /**
            * Updates all nodes and connections with the origin
            * to draw relative from 
            * (i.e used to keep position when changing zoom)
            */

        if (guiNodes != null){
            for(int i=0; i<guiNodes.Count; i++){
                guiNodes[i].UpdateOrigin(origin);
            }
        }
        if (connections != null){
            for(int j=0; j<connections.Count; j++){
                connections[j].UpdateOrigin(origin);
            }
        }
    }

    private void DrawDetailsPanel(){

        /**
         * Side panel with basic controls and info
         */

        BeginWindows();
        detailsPanel = GUILayout.Window(1, 
                                        detailsPanel, 
                                        DrawDetailInfo, 
                                        "Details");
        EndWindows();
    }

    public void UpdatePanelDetails(GuiNode node){
        if (selectedNode != null && selectedNode != node){
            selectedNode.SetSelected(false);
        }
        selectedNode = node;
    }

    void DrawDetailInfo(int unusedWindowID)
    {
        if (GUILayout.Button(saveButton)){
            SaveTree();
        }
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarButtons);
        ToolbarTab activeToolbarTab = (ToolbarTab)toolbarInt;
        switch(activeToolbarTab){
            case ToolbarTab.SelectedNode:
                DrawNodeDetails(unusedWindowID);
                break;
            case ToolbarTab.Blackboard:
                DrawBlackboardDetails(unusedWindowID);
                break;
        }
    }

    void DrawNodeDetails(int unusedWindowID){

        // Update details panel with selected node
        if (selectedNode != null && selectedNode.IsSelected == false){
            selectedNode = null;
        }
        if (selectedNode != null){
            selectedNode.DrawDetails();
        }

        // Draw clear tree button away from other buttons
        GUILayout.FlexibleSpace(); 
        if (GUILayout.Button(clearTreeButton)){
            for (int i=guiNodes.Count-1; i>0;i--){
                if (selectedNode == guiNodes[i]){
                    selectedNode = null;
                }
                RemoveNode(guiNodes[i]);
            }
            ResetRootNode();
            zoomCoordsOrigin = Vector2.zero;
            currentZoom=1.0f;
        }
    }

    void DrawKey(string keyName, string keyType){

        /**
            * Logic here is controlled using activeBlackboardKey[currentName, newName]
            * newName stores a possible new name when editing a key name.
            * After editing a name, all the decorators/probability weights need to be told
            * in case they are using the key
            */

        bool EditingName(){
            return (keyName == activeBlackboardKey[0] && renamingBlackboardKey);
        }

        bool NameUpdated(){
            return (keyName == activeBlackboardKey[0] && !renamingBlackboardKey);
        }

        bool ValidNewName(string newName){
            if (!bt.blackboard.GetAllKeyNames().Contains(newName) && newName != ""){
                return true;
            }
            return false;
        }

        void DrawEditNameField(){
            GUILayout.BeginHorizontal();
            activeBlackboardKey[1] = GUILayout.TextField(
                activeBlackboardKey[1], 
                50,
                GUILayout.MinWidth(175),
                GUILayout.MaxWidth(175));
            GUILayout.Label(keyType, blackboardTypeStyles[keyType]);
            GUILayout.EndHorizontal();
        }

        void DrawKeyButton(){
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(keyName, GUILayout.MinWidth(175), GUILayout.MaxWidth(175))){
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Rename"), false, () => OnClickRenameBlackboardKey(keyName));
                if ((keyType == "float" || keyType == "int") && WeightKeyInUse(keyName)){
                    menu.AddDisabledItem(new GUIContent("Remove (being used by a node)"));
                }
                else if (keyType == "bool" && BoolKeyInUse(keyName)){
                    menu.AddDisabledItem(new GUIContent("Remove (being used by a node)"));
                }
                else{
                    menu.AddItem(new GUIContent("Remove"), false, () => OnClickRemoveBlackboardKey(keyName));
                }
                menu.ShowAsContext();
            }
            GUILayout.Label(keyType, blackboardTypeStyles[keyType]);
            GUILayout.EndHorizontal();
        }

        // Active text field for renaming
        if (EditingName()){
            DrawEditNameField();
        }
        else if(NameUpdated()){

            // update name if possible and draw button 
            string newName = activeBlackboardKey[1];
            if (newName != activeBlackboardKey[0]){
                if (ValidNewName(newName)){
                    bt.blackboard.RenameKey(keyName, newName);
                    if (keyType == "bool"){
                        RefreshDecoratorTasks(keyName, newName);
                    }
                    else if (keyType == "float" || keyType == "int"){
                        RefreshProbabilityWeightTasks(keyName, newName);
                    }
                    TreeModified();
                }
                else{
                    newName = activeBlackboardKey[0];
                }
                DrawKeyButton();
            }
            activeBlackboardKey[0] = "";
            activeBlackboardKey[1] = "";
        }
        else{
            DrawKeyButton();
        }
    }


    void DrawBlackboardDetails(int unusedWindowID){

        EditorGUI.BeginDisabledGroup(BlackboardInUse());
        bt.blackboard = (BehaviourTreeBlackboard)EditorGUILayout.ObjectField(bt.blackboard, typeof(BehaviourTreeBlackboard), false);
        EditorGUI.EndDisabledGroup();

        if (bt.blackboard == null){
            return;
        }

        // Add key button    
        if (GUILayout.Button(addKeyButton)){
            GenericMenu genericMenu = new GenericMenu();
            foreach(string keyType in bt.blackboard.GetKeyTypes()){
                genericMenu.AddItem(new GUIContent(keyType), false, () => OnClickAddBlackboardKey(keyType)); 
            }
            genericMenu.ShowAsContext();
        }

        // Display current keys
        GUILayout.Label("Keys");
        ICollection keyNamesCollection = bt.blackboard.GetAllKeyNames().Keys;
        ICollection keyTypesCollection = bt.blackboard.GetAllKeyNames().Values;
        String[] keyNames = new String[keyNamesCollection.Count];
        String[] keyTypes = new String[keyNamesCollection.Count];
        keyNamesCollection.CopyTo(keyNames,0);
        keyTypesCollection.CopyTo(keyTypes,0);
        for (int i=0; i<keyNames.Length; i++){
            DrawKey(keyNames[i], keyTypes[i]);

        }
    }

    void OnClickAddBlackboardKey(string keyType){
        if (bt.blackboard != null && bt.blackboard.GetKeyTypes().Contains(keyType)){
            bt.blackboard.AddKey(keyType);
            TreeModified();
        }
    }

    void OnClickRemoveBlackboardKey(string keyName){
        bt.blackboard.RemoveKey(keyName);
        TreeModified();
    }

    void OnClickRenameBlackboardKey(string keyName){
        renamingBlackboardKey = true;
        activeBlackboardKey[0] = keyName;
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {

        /**
            * Background grid of the editor
            */

        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (guiNodes != null)
        {
            for (int i = 0; i < guiNodes.Count; i++)
            {
                guiNodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            } 
        }
    }

    private void ProcessZoomEvent(Event e){

        /**
            * Event handler when mouse wheel is used
            */ 

        Vector2 screenCoordsMousePos = e.mousePosition;
        Vector2 delta = e.delta;
        Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
        float zoomDelta = -delta.y / 150.0f;
        float oldZoom = currentZoom;
        currentZoom += zoomDelta;
        currentZoom = Mathf.Clamp(currentZoom, zoomMin, zoomMax);
        zoomCoordsOrigin += (zoomCoordsMousePos - zoomCoordsOrigin) - (oldZoom / currentZoom) * (zoomCoordsMousePos - zoomCoordsOrigin);
        e.Use();
    }

    private void ProcessEvents(Event e)
    {
        /**
            * Events not on nodes or connections
            */

        switch (e.type)
        {
            case EventType.ScrollWheel:
                ProcessZoomEvent(e);
                break;
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (drawingLine){
                        ProcessCreateContextMenu(e.mousePosition);
                    }
                    else{
                        ClearConnectionSelection();
                    }
                    renamingBlackboardKey = false;
                }

                if (e.button == 1)
                {
                    renamingBlackboardKey = false;
                    ClearConnectionSelection();
                }
            break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    renamingBlackboardKey = false;

                    if (MousePosOnGrid(e.mousePosition)){
                        OnDrag(e.delta);
                        e.Use();
                    }
                }
            break;
            case EventType.KeyDown:
                renamingBlackboardKey = false;
            break;
        }
    }

    private void ProcessEventsRuntime(Event e)
    {
        /**
            * Used to restrict controls at runtime
            */ 

        switch (e.type)
        {
            case EventType.MouseDrag:
                if (e.button == 0)
                {

                    if (MousePosOnGrid(e.mousePosition)){
                        OnDrag(e.delta);
                        e.Use();
                    }
                }
            break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        bool guiChanged = false;
        GuiNode currentSeletedNode = selectedNode; 
        Vector2 zoomMousePos = (e.mousePosition - mainWindow.TopLeft()) / currentZoom;
        if (guiNodes != null)
        {
            for (int i = guiNodes.Count - 1; i >= 0; i--)
            {
                if (guiNodes[i].ProcessEvents(e, zoomMousePos)){
                    guiChanged = true;
                }
        }
        }

        if (selectedNode != null){
            if (!selectedNode.IsSelected){
                selectedNode = null;
            }
            else if (selectedNode != currentSeletedNode){
                toolbarInt = (int)ToolbarTab.SelectedNode; 
            }
        }

        if (connections != null){
            for (int i = connections.Count - 1; i>=0; i--){
                if (connections[i].ProcessProbabilityWeightEvents(e, zoomMousePos)){
                    guiChanged = true;
                }
            }
        }
        if (guiChanged){
            GUI.changed = true;
        }
    }

    private void ProcessCreateContextMenu(Vector2 mousePosition)
    {
        /**
            * Menu when clicking on a node
            */ 

        GenericMenu genericMenu = new GenericMenu();
        foreach(NodeType nodeType in decisionNodeTypes){
            string nodeTypeString = BehaviourTreeProperties.GetDefaultStringFromNodeType(nodeType);
            genericMenu.AddItem(new GUIContent(nodeTypeString), false, () => OnClickAddNode(mousePosition, nodeType, nodeTypeString)); 
        }
        foreach(string taskName in customTaskNames){
            genericMenu.AddItem(new GUIContent("Actions/"+taskName), false, () => OnClickAddNode(mousePosition, NodeType.Action, taskName)); 
        }
        genericMenu.ShowAsContext();

    }

    private void OnDrag(Vector2 delta)
    {
        if (guiNodes != null)
        {
            for (int i = 0; i < guiNodes.Count; i++)
            {
                guiNodes[i].Drag(delta);
            }
            TreeModified();
        }

        GUI.changed = true;
    }

    bool MousePosOnGrid(Vector2 mousePos){
        if (detailsPanel.Contains(mousePos)){
            return false;
        }
        return true;
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedChildPoint != null && selectedParentPoint == null)
        {
            Vector2 startPos = selectedChildPoint.GetScaledRect().center;
            Handles.DrawBezier(
                startPos,
                e.mousePosition,
                startPos,
                startPos,
                Color.white,
                null,
                4f
            );
            GUI.changed = true;
            drawingLine = true;
        }
        else{
            drawingLine = false;
        }
    }


    private void OnClickAddNode(Vector2 mousePosition, 
                                NodeType nodeType, 
                                string displayTask, 
                                string displayName="")
    {
        if (guiNodes == null)
        {
            guiNodes = new List<CompositeGuiNode>();
        }

        Node parentNode = selectedChildPoint.node.BtNode;

        switch(nodeType){
            case NodeType.SequenceSelector:
                SequenceSelector sequenceNode = new SequenceSelector(
                    taskName:displayTask,
                    parentNode:parentNode
                );
                parentNode.AddChildNode(sequenceNode);
                guiNodes.Add(
                    new GuiSequenceSelector(
                        node:sequenceNode,
                        displayTask:displayTask,
                        displayName:displayName,
                        pos:ConvertScreenCoordsToZoomCoords(mousePosition),
                        parentConnection:null,
                        UpdatePanelDetails:UpdatePanelDetails,
                        TreeModified:TreeModified,
                        OnRemoveNode:OnClickRemoveNode,
                        OnClickChildPoint:OnClickChildPoint,
                        OnClickParentPoint:OnClickParentPoint,
                        blackboard:ref bt.blackboard
                    )
                );
                break;
            case NodeType.PrioritySelector:
                PrioritySelector priorityNode = new PrioritySelector(
                    taskName:displayTask,
                    parentNode:parentNode
                );
                parentNode.AddChildNode(priorityNode);
                guiNodes.Add(
                    new GuiPrioritySelector(
                        node:priorityNode,
                        displayTask:displayTask,
                        displayName:displayName,
                        pos:ConvertScreenCoordsToZoomCoords(mousePosition),
                        parentConnection:null,
                        UpdatePanelDetails:UpdatePanelDetails,
                        TreeModified:TreeModified,
                        OnRemoveNode:OnClickRemoveNode,
                        OnClickChildPoint:OnClickChildPoint,
                        OnClickParentPoint:OnClickParentPoint,
                        blackboard:ref bt.blackboard
                    )
                );
                break;
            case NodeType.ProbabilitySelector:
                ProbabilitySelector probNode = new ProbabilitySelector(
                    taskName:displayTask,
                    blackboard:ref bt.blackboard,
                    parentNode:parentNode
                );
                parentNode.AddChildNode(probNode);
                guiNodes.Add(
                    new GuiProbabilitySelector(
                        node:probNode,
                        displayTask:displayTask,
                        displayName:displayName,
                        pos:ConvertScreenCoordsToZoomCoords(mousePosition),
                        parentConnection:null,
                        UpdatePanelDetails:UpdatePanelDetails,
                        TreeModified:TreeModified,
                        OnRemoveNode:OnClickRemoveNode,
                        OnClickChildPoint:OnClickChildPoint,
                        OnClickParentPoint:OnClickParentPoint,
                        blackboard:ref bt.blackboard
                    )
                );
                break;
            case NodeType.Action:
                if (displayTask == "Wait"){
                    ActionWaitNode actionNode = new ActionWaitNode(
                        taskName:displayTask,
                        blackboard:ref bt.blackboard,
                        timerValue:BehaviourTreeProperties.DefaultTimerVal(),
                        randomDeviation:BehaviourTreeProperties.DefaultRandomDeviationVal(),
                        parentNode:parentNode
                    );
                    parentNode.AddChildNode(actionNode);
                    guiNodes.Add(
                        new GuiActionWaitNode(
                            node:actionNode,
                            displayTask:displayTask,
                            displayName:displayName,
                            pos:ConvertScreenCoordsToZoomCoords(mousePosition),
                            parentConnection:null,
                            UpdatePanelDetails:UpdatePanelDetails,
                            TreeModified:TreeModified,
                            OnRemoveNode:OnClickRemoveNode,
                            OnClickChildPoint:OnClickChildPoint,
                            OnClickParentPoint:OnClickParentPoint,
                            blackboard:ref bt.blackboard
                        )
                    );
                }
                else{
                    ActionNode actionNode = new ActionNode(
                        taskName:displayTask,
                        blackboard:ref bt.blackboard,
                        parentNode:parentNode
                    );
                    parentNode.AddChildNode(actionNode);
                    guiNodes.Add(
                        new GuiActionNode(
                            node:actionNode,
                            displayTask:displayTask,
                            displayName:displayName,
                            pos:ConvertScreenCoordsToZoomCoords(mousePosition),
                            parentConnection:null,
                            UpdatePanelDetails:UpdatePanelDetails,
                            TreeModified:TreeModified,
                            OnRemoveNode:OnClickRemoveNode,
                            OnClickChildPoint:OnClickChildPoint,
                            OnClickParentPoint:OnClickParentPoint,
                            blackboard:ref bt.blackboard
                        )
                    );
                }
                break;
            default:
                throw new Exception($"Unexpected node type {nodeType}");
        }

        selectedParentPoint = guiNodes[guiNodes.Count-1].ParentPoint;
        CreateConnection();
        ClearConnectionSelection();
        guiNodes[guiNodes.Count-1].SetParentConnection(connections[connections.Count-1]);
        int numNodes = UpdateCallNumbers(guiNodes[0], 1);
        TreeModified();
    }

    

    private void OnClickChildPoint(ConnectionPoint ChildPoint)
    {
        selectedChildPoint = ChildPoint;

        if (selectedParentPoint != null)
        {
            if (selectedParentPoint.node != selectedChildPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection(); 
                TreeModified();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickParentPoint(ConnectionPoint ParentPoint)
    {
        selectedParentPoint = ParentPoint;

        if (selectedChildPoint != null)
        {
            if (selectedParentPoint.node != selectedChildPoint.node)
            {
                if (selectedParentPoint.node.HasParentConnection()){
                    connections.Remove(selectedParentPoint.node.ParentConnection);
                    selectedParentPoint.node.RemoveParentConnection();
                }
                CreateConnection();
                ClearConnectionSelection();
                TreeModified();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(CompositeGuiNode node)
    {
        RemoveNode(node);
    }

    private void RemoveNode(CompositeGuiNode node){

        // Remove connections 
        if (connections != null)
        {
            connections.Remove(node.ParentConnection);
            node.RemoveParentConnection();
            
            List<Connection> childConnections = node.ChildConnections;
            if (childConnections != null){
                for (int i=childConnections.Count-1; i>=0; i--){
                    ResetCallNumber(childConnections[i].GetChildNode());
                    connections.Remove(childConnections[i]);
                    node.RemoveChildConnection(childConnections[i]);

                }
            }
        }
        if (node == selectedNode){
            selectedNode = null;
        }
        // Remove corresponding node on the BehaviourTree
        node.BtNode.Unlink(false);
        node.BtNode.OnStateChange -= NodeStateChangeHandler;

        // Remove the GuiNode
        guiNodes.Remove(node);
        TreeModified();
        GUI.changed = true;

    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
        connection.GetChildNode().RemoveParentConnection();
        connection.GetParentNode().RemoveChildConnection(connection);
        TreeModified();
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        Connection newConnection = new Connection(selectedChildPoint, selectedParentPoint, OnClickRemoveConnection);
        CompositeGuiNode parentNode = selectedChildPoint.GetNode();
        CompositeGuiNode childNode = selectedParentPoint.GetNode();
        parentNode.AddChildConnection(newConnection);
        childNode.SetParentConnection(newConnection);
        if (parentNode.GetNodeType() == NodeType.ProbabilitySelector){
            AddProbabilityWeight(
                newConnection
            );
        } 
        connections.Add(newConnection);
        drawingLine = false;
        GUI.changed = true;
    }

    void AddProbabilityWeight(Connection connection, 
                                string taskName="Constant Weight",
                                string displayName=""){

        // Update behaviour tree
        Node childNode = connection.GetChildNode().BtNode;
        Node parentNode = connection.GetParentNode().BtNode;
        ProbabilityWeight probabilityWeight = new ProbabilityWeight(
            taskName:taskName,
            parentNode:parentNode,
            childNode:childNode
        );              
        parentNode.RemoveChildNode(childNode);
        parentNode.AddChildNode(probabilityWeight);
        childNode.SetParentNode(probabilityWeight);

        //Update GUI
        connection.AddProbabilityWeight(
            node:probabilityWeight,
            displayTask:taskName,
            displayName:displayName,
            UpdatePanelDetails:UpdatePanelDetails,
            TreeModified:TreeModified,
            NodeUpdated:connection.GetChildNode().NodeUpdated,
            blackboard:ref bt.blackboard
        );
        TreeModified();
    }

    void AddProbabilityWeight(Connection connection, 
                                GuiProbabilityWeight guiProbabilityWeight,
                                bool updateRelationships=true
                                ){

        // Update behaviour tree
        if (updateRelationships){
            Node childNode = connection.GetChildNode().BtNode;
            Node parentNode = connection.GetParentNode().BtNode;
            parentNode.RemoveChildNode(childNode);
            parentNode.AddChildNode(guiProbabilityWeight.BtNode);
            childNode.SetParentNode(guiProbabilityWeight.BtNode);
        }

        //Update GUI
        connection.AddProbabilityWeight(
            guiNode:guiProbabilityWeight
        );
        TreeModified();
    }
    private void CreateConnection(ConnectionPoint childPoint, ConnectionPoint parentPoint){

        if (connections == null)
        {
            connections = new List<Connection>();
        }

        Connection newConnection = new Connection(childPoint, parentPoint, OnClickRemoveConnection);
        CompositeGuiNode parentNode = childPoint.GetNode();
        CompositeGuiNode childNode = parentPoint.GetNode();
        parentNode.AddChildConnection(newConnection);
        childNode.SetParentConnection(newConnection);
        if (parentNode.GetNodeType() == NodeType.ProbabilitySelector){
            AddProbabilityWeight(newConnection);
        } 
        connections.Add(newConnection);
        GUI.changed = true;
        TreeModified();
    }

    private void CreateConnection(
        ConnectionPoint childPoint, 
        ConnectionPoint parentPoint, 
        GuiProbabilityWeight probabilityWeight,
        bool updateRelationships=true){

        if (probabilityWeight == null){
            CreateConnection(childPoint:childPoint, parentPoint:parentPoint);
        }
        else{
            if (connections == null)
            {
                connections = new List<Connection>();
            }

            Connection newConnection = new Connection(childPoint, parentPoint, OnClickRemoveConnection);
            CompositeGuiNode parentNode = childPoint.GetNode();
            CompositeGuiNode childNode = parentPoint.GetNode();
            parentNode.AddChildConnection(newConnection);
            childNode.SetParentConnection(newConnection);
            AddProbabilityWeight(newConnection, probabilityWeight, updateRelationships);
            connections.Add(newConnection);
            GUI.changed = true;
        }
        TreeModified();

    }

    private void ClearConnectionSelection()
    {
        selectedChildPoint = null;
        selectedParentPoint = null;
    }

    private int UpdateCallNumbers(CompositeGuiNode startingNode, int callNumber){

        /**
            * Recursively update call numbers depth first from startingNode
            */

        // Update decorators first 
        List<GuiDecorator> decorators = startingNode.Decorators;
        if (decorators != null){
            for(int i=0; i<decorators.Count; i++){
                decorators[i].SetCallNumber(callNumber);
                callNumber++;
            }
        }

        // Update guiNode
        startingNode.SetCallNumber(callNumber);
        callNumber++;

        // Update child guiNodes
        startingNode.RefreshChildOrder();
        for(int j=0; j<startingNode.ChildConnections.Count; j++){
            callNumber = UpdateCallNumbers(
                startingNode.ChildConnections[j].GetChildNode(), 
                callNumber
                );
        }
        return callNumber;
    }

    private void ResetCallNumber(CompositeGuiNode startingNode){

        // Update decorators first 
        List<GuiDecorator> decorators = startingNode.Decorators;
        if (decorators != null){
            for(int i=0; i<decorators.Count; i++){
                decorators[i].SetCallNumber(-1);
            }
        }

        // Update guiNodes
        startingNode.SetCallNumber(-1);

        for(int j=0; j<startingNode.ChildConnections.Count; j++){
            ResetCallNumber(startingNode.ChildConnections[j].GetChildNode());
        }

    }

    private void RefreshDecoratorTasks(string oldKeyName, string newKeyName){

        /**
            * Updates the task of decorators with newKeyName if they their
            * task was oldKeyName
            */
        
        if (guiNodes == null){
            return;
        }
        for(int i=0; i<guiNodes.Count; i++){
            guiNodes[i].RefreshDecoratorTasks(oldKeyName, newKeyName);
        }
    }

    private void RefreshProbabilityWeightTasks(string oldKeyName, string newKeyName){

        /**
            * Updates the task of probability weights with newKeyName if they their
            * task was oldKeyName
            */

        if (guiNodes == null){
            return;
        }
        for(int i=0; i<guiNodes.Count; i++){
            if (guiNodes[i] is GuiProbabilitySelector psn){
                if (psn.ChildConnections != null){
                    for(int j=0; j<psn.ChildConnections.Count; j++){
                        psn.ChildConnections[j].RefreshProbabilityWeightTask(oldKeyName, newKeyName);
                    }
                }
            }
        }
    }

    bool WeightKeyInUse(string keyName){

        /**
            * Finds all probability weights and checks 
            * if their task is keyName
            */

        if (guiNodes == null){
            return false;
        }
        for(int i=0; i<guiNodes.Count; i++){
            if (guiNodes[i] is GuiProbabilitySelector psn){
                if (psn.ChildConnections == null){
                    continue;
                }
                for(int j=0; j<psn.ChildConnections.Count; j++){
                    if (psn.ChildConnections[j].GetProbabilityWeightKey() == keyName){
                        return true;
                    }
                }
            }
        }
        return false;
    }


    bool BoolKeyInUse(string keyName){

        /**
            * Finds all decorators and checks 
            * if their task is keyName
            */

        if (guiNodes == null){
            return false;
        }
        for(int i=0; i<guiNodes.Count; i++){
            if (guiNodes[i].Decorators == null){
                continue;
            }
            for(int j=0; j<guiNodes[i].Decorators.Count; j++){
                if (guiNodes[i].Decorators[j].DisplayTask == keyName){
                    return true;
                }
            }
        }
        return false;
    }

    List<BehaviourTreeTask> GetCustomTasks()
    {
        /**
            * Gets all classes that inherit from BehaviourTreeTask
            */ 

        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(BehaviourTreeTask)))
            .Select(type => Activator.CreateInstance(type) as BehaviourTreeTask).ToList();
    }

    List<string> GetCustomTaskNames(){
        List<string> taskNames = new List<string>();
        GetCustomTasks().ForEach(x => taskNames.Add(x.GetType().ToString()));
        return taskNames;
    }

    void LoadFromRoot(Node rootNode, List<GuiNodeData> nodeMetaData){
        guiNodes = new List<CompositeGuiNode>();
        connections = new List<Connection>();
        int idx = 0;
        AddNodeAndConnections(
            node:rootNode, 
            idx:ref idx, 
            nodeMetaData:nodeMetaData
            );
        UpdateCallNumbers(guiNodes[0], 1);
    }

    void SaveTree(){
        if (bt != null){
            EditorUtility.SetDirty(bt);
        }
        if (bt.blackboard != null){
            EditorUtility.SetDirty(bt.blackboard);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        saved=true;
        saveButton.text = "Saved";

    }

    void NodeStateChangeHandler(){
        Repaint();
    }

    void AddNodeAndConnections(
        Node node, 
        ref int idx,
        List<GuiNodeData> nodeMetaData,
        int parentGuiNodeIdx=-1,
        List<GuiDecorator> decorators=null,
        GuiProbabilityWeight probabilityWeight=null){

        /**
            * Recursively fills guiNodes and connections based on
            * node.
            */ 

        GuiNodeData metaData = nodeMetaData[idx];
        var guiNode = BehaviourTreeLoader.GuiNodeFactory(
            node:node,
            displayTask:node.TaskName,
            displayName:metaData.displayName,
            pos:new Vector2(metaData.xPos, metaData.yPos),
            blackboard:ref bt.blackboard
        );

        if (node is Decorator decorator){
            if (decorators == null){
                decorators = new List<GuiDecorator>();
            }
            decorators.Add((GuiDecorator)guiNode);
            idx++;
            AddNodeAndConnections(
                node:node.ChildNodes[0],
                idx:ref idx, 
                nodeMetaData:nodeMetaData,
                parentGuiNodeIdx:parentGuiNodeIdx,
                decorators:decorators,
                probabilityWeight:probabilityWeight
                );
        }

        else if (node is ProbabilityWeight weightNode){
            probabilityWeight = (GuiProbabilityWeight)guiNode;
            probabilityWeight.SetEditorActions(
                UpdatePanelDetails:UpdatePanelDetails,
                TreeModified:TreeModified
            );
            idx++;
            AddNodeAndConnections(
                node:node.ChildNodes[0],
                idx:ref idx, 
                nodeMetaData:nodeMetaData,
                parentGuiNodeIdx:parentGuiNodeIdx,
                decorators:decorators,
                probabilityWeight:probabilityWeight
                );
        }
        else {

            // Assumed to be CompositeNode -> now need to make connections
            CompositeGuiNode cgn = (CompositeGuiNode)guiNode;

            // Decorators
            if (decorators != null){
                for(int i=decorators.Count-1; i>=0; i--){
                    cgn.AddDecorator(decorators[i]);
                }
            }

            // Editor specific functions
            cgn.SetEditorActions(
                UpdatePanelDetails:UpdatePanelDetails,
                TreeModified:TreeModified,
                OnRemoveNode:OnClickRemoveNode,
                OnClickChildPoint:OnClickChildPoint,
                OnClickParentPoint:OnClickParentPoint
            );

            // Connections
            if (parentGuiNodeIdx != -1){
                CreateConnection(
                    parentPoint:cgn.ParentPoint, 
                    childPoint:guiNodes[parentGuiNodeIdx].ChildPoint,
                    probabilityWeight:probabilityWeight,
                    updateRelationships:false
                    );
            }

            cgn.BtNode.OnStateChange += NodeStateChangeHandler;
            guiNodes.Add(cgn);
            parentGuiNodeIdx=guiNodes.Count-1;
            idx++;
            decorators = null;
            probabilityWeight = null;

            for(int i=0; i<node.ChildNodes.Count; i++){
                AddNodeAndConnections(
                    node:node.ChildNodes[i],
                    idx:ref idx, 
                    nodeMetaData:nodeMetaData,
                    parentGuiNodeIdx:parentGuiNodeIdx,
                    decorators:decorators,
                    probabilityWeight:probabilityWeight
                    );
            }
        }
    }

    void TreeModified(){
        if (saved && !loadingTree){
            saved = false;
            saveButton.text = "Unsaved Changes";
        }
    }


    void UpdateBlackboard(){

        /**
         * Updates all nodes with the current BehaviourTreeBlackboard
         * (Used when a different blackboard is added, or its removed)
         */

        if (activeBlackboard && bt.blackboard == null){
            for (int i=0; i<guiNodes.Count; i++){
                guiNodes[i].UpdateBlackboard(ref bt.blackboard);
            }
            for (int j=0; j<connections.Count; j++){
                connections[j].UpdateBlackboard(ref bt.blackboard);
            }
            activeBlackboard = false;
        }
        else if (!activeBlackboard && bt.blackboard != null){
            if (guiNodes!= null){
            for (int i=0; i<guiNodes.Count; i++){
                guiNodes[i].UpdateBlackboard(ref bt.blackboard);
            }
            for (int j=0; j<connections.Count; j++){
                connections[j].UpdateBlackboard(ref bt.blackboard);
            }
            activeBlackboard = true;
            }
        }
    }

    bool BlackboardInUse(){

        /**
         * Checks if any nodes are using blackboard keys
         * (Used to avoid the blackboard being removed when in use)
         */

        if (!activeBlackboard){
            return false;
        }
        for (int i=0;i<guiNodes.Count;i++){
            if (guiNodes[i].Decorators!=null && guiNodes[i].Decorators.Count>0){
                return true;
            }
            if (guiNodes[i] is GuiProbabilitySelector ps){
                if (ps.ChildConnections != null){
                    for(int j=0; j<ps.ChildConnections.Count; j++){
                        if (!ps.ChildConnections[j].probabilityWeight.HasConstantWeight()){
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

}
}