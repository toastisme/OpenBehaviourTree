using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class NodeBasedEditor : EditorWindow
{
    // Bookkeeping
    public BehaviourTree bt;
    private NodeBase selectedNode;

    // Styles
    private GUIStyle guiNodeStyle;
    private GUIStyle selectedGuiNodeStyle;
    private GUIStyle actionStyle;
    private GUIStyle selectedActionStyle;
    private GUIStyle sequenceSelectorStyle;
    private GUIStyle selectedSequenceSelectorStyle;
    private GUIStyle prioritySelectorStyle;
    private GUIStyle selectedPrioritySelectorStyle;
    private GUIStyle probabilitySelectorStyle;
    private GUIStyle selectedProbabilitySelectorStyle;
    private GUIStyle childPointStyle;
    private GUIStyle parentPointStyle;
    private GUIStyle callNumberStyle;
    private GUIStyle decoratorStyle;
    private GUIStyle selectedDecoratorStyle;
    private GUIStyle probabilityWeightStyle;
    private GUIStyle selectedProbabilityWeightStyle;
    private Vector2 guiNodeSize;
    private Vector2 subNodeSize; 

    private ConnectionPoint selectedChildPoint;
    private ConnectionPoint selectedParentPoint;

    // Movement
    private Vector2 offset;
    private Vector2 drag;
    private bool drawingLine;

    private Rect detailsPanel;
    private string[] toolbarStrings = {"Selected Node", "Blackboard"};
    private int toolbarInt = 0;

    List<NodeType> decisionNodeTypes = new List<NodeType>{NodeType.PrioritySelector, 
                                                        NodeType.ProbabilitySelector,
                                                        NodeType.SequenceSelector};
    List<string> customTaskNames;

    // Blackboard
    string[] activeBlackboardKey = {"","",""};
    bool renamingBlackboardKey = false;

    /*
    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
    }
    */

    public void SetScriptableObject(BehaviourTree behaviourTree){
        bt = behaviourTree;
        if (bt.nodes == null){
            AddRootNode();
        }
    }

    private void OnEnable()
    {
        guiNodeSize = NodeProperties.GUINodeSize();
        subNodeSize = NodeProperties.SubNodeSize();
        guiNodeStyle = NodeProperties.GUINodeStyle(); 
        sequenceSelectorStyle = NodeProperties.SequenceSelectorStyle();
        prioritySelectorStyle = NodeProperties.PrioritySelectorStyle();
        probabilitySelectorStyle = NodeProperties.ProbabilitySelectorStyle();
        actionStyle = NodeProperties.ActionStyle();
        decoratorStyle = NodeProperties.DecoratorStyle();
        probabilityWeightStyle = NodeProperties.ProbabilityWeightStyle();
        callNumberStyle = NodeProperties.CallNumberStyle();
        childPointStyle = NodeProperties.ChildPointStyle();
        parentPointStyle = NodeProperties.ParentPointStyle();

        selectedGuiNodeStyle = NodeProperties.SelectedGUINodestyle();
        selectedSequenceSelectorStyle = NodeProperties.SelectedSequenceSelectorStyle();
        selectedPrioritySelectorStyle = NodeProperties.SelectedPrioritySelectorStyle();
        selectedProbabilitySelectorStyle = NodeProperties.SelectedProbabilitySelectorStyle();
        selectedActionStyle = NodeProperties.SelectedActionStyle();
        selectedDecoratorStyle = NodeProperties.SelectedDecoratorStyle();
        selectedProbabilityWeightStyle = NodeProperties.SelectedProbabilityWeightStyle();

        customTaskNames = GetCustomTaskNames();
    }

    private void AddRootNode(){

        bt.nodes = new List<GUINode>();
        bt.nodes.Add(new GUINode(NodeType.Root,
                              new Vector2(0,0), 
                              guiNodeSize,
                              guiNodeStyle, 
                              selectedGuiNodeStyle, 
                              childPointStyle, 
                              parentPointStyle, 
                              callNumberStyle,
                              decoratorStyle,
                              selectedDecoratorStyle,
                              UpdatePanelDetails,
                              OnClickChildPoint, 
                              OnClickParentPoint, 
                              OnClickRemoveNode,
                              bt));
    }

    private void ResetRootNode(){
        bt.nodes[0].SetName("");
    }

    private void OnGUI()
    {
        if (MousePosOnGrid(Event.current.mousePosition)){
            ProcessNodeEvents(Event.current);
        }
        ProcessEvents(Event.current);

        if (GUI.changed) {
            Repaint();
            UpdateCallNumbers(bt.nodes[0], 1);
        }

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);
        DrawDetailsPanel();
    }

    private void DrawDetailsPanel(){
        BeginWindows();
        detailsPanel = new Rect(position.width*.8f, 0, position.width*.2f, position.height);
        detailsPanel = GUILayout.Window(1, detailsPanel, DrawDetailInfo, "Details");
        EndWindows();
    }

    public void UpdatePanelDetails(NodeBase node){
        selectedNode = node;
    }

    void DrawDetailInfo(int unusedWindowID)
    {
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
        switch(toolbarInt){
            case 0:
                DrawNodeDetails(unusedWindowID);
                break;
            case 1:
                DrawBlackboardDetails(unusedWindowID);
                break;
        }
    }

    void DrawNodeDetails(int unusedWindowID){
        if (GUILayout.Button("Clear All")){
            for (int i=bt.nodes.Count-1; i>0;i--){
                if (selectedNode == bt.nodes[i]){
                    selectedNode = null;
                }
                RemoveNode(bt.nodes[i]);
            }
            ResetRootNode();
        }
        if (selectedNode != null){
            GUILayout.Label("Task: " + selectedNode.GetTask());
            GUILayout.Label("Name");
            selectedNode.SetName(GUILayout.TextField(selectedNode.GetName(), 50));
        }
    }

    void DrawKey(string keyName, string keyType){
        if (keyName == activeBlackboardKey[0] && renamingBlackboardKey){
            activeBlackboardKey[1] = GUILayout.TextField(activeBlackboardKey[1], 50);
        }
        else if(keyName == activeBlackboardKey[0] && !renamingBlackboardKey){
            // update name if possible and draw button 
            string newName = activeBlackboardKey[1];
            if (newName != activeBlackboardKey[0]){
                if (!bt.blackboard.GetAllKeyNames().Contains(newName) && newName != ""){
                    bt.blackboard.RenameKey(keyName, newName);
                    if (keyType == "bool"){
                        RefreshDecoratorTasks(keyName, newName);
                    }
                }
                else{
                    newName = activeBlackboardKey[0];
                }
                if (GUILayout.Button(newName + " (" + keyType + ")")){
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Rename"), false, () => OnClickRenameBlackboardKey(keyName));
                    menu.AddItem(new GUIContent("Remove"), false, () => OnClickRemoveBlackboardKey(keyName));
                    menu.ShowAsContext();
                }
            }
            activeBlackboardKey[0] = "";
            activeBlackboardKey[1] = "";
        }
        else{
            // draw button
            if (GUILayout.Button(keyName + " (" + keyType + ")")){
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Rename"), false, () => OnClickRenameBlackboardKey(keyName));
                menu.AddItem(new GUIContent("Remove"), false, () => OnClickRemoveBlackboardKey(keyName));
                menu.ShowAsContext();
            }
        }
    }
    void DrawBlackboardDetails(int unusedWindowID){
        if (GUILayout.Button("New Key")){
            GenericMenu genericMenu = new GenericMenu();
            foreach (string keyType in bt.blackboard.GetKeyTypes()){
                genericMenu.AddItem(new GUIContent(keyType), false, () => OnClickAddBlackboardKey(keyType)); 
            }
            genericMenu.ShowAsContext();
        }

        // Display current keys
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
        }
    }

    void OnClickRemoveBlackboardKey(string keyName){
        bt.blackboard.RemoveKey(keyName);
    }

    void OnClickRenameBlackboardKey(string keyName){
        renamingBlackboardKey = true;
        activeBlackboardKey[0] = keyName;
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
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
        if (bt.nodes != null)
        {
            for (int i = 0; i < bt.nodes.Count; i++)
            {
                bt.nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (bt.connections != null)
        {
            for (int i = 0; i < bt.connections.Count; i++)
            {
                bt.connections[i].Draw();
            } 
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (drawingLine){
                        ProcessCreateContextMenu(e.mousePosition);
                    }
                    else{
                        ClearConnectionSelection();
                    }
                }

                if (e.button == 1)
                {
                    if (renamingBlackboardKey){
                        renamingBlackboardKey = false;
                    }
                    ClearConnectionSelection();
                }
            break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {

                    if (MousePosOnGrid(e.mousePosition)){
                        OnDrag(e.delta);
                    }
                }
            break;
        }
    }

    bool MousePosOnGrid(Vector2 mousePos){
        if (detailsPanel.Contains(mousePos)){
            return false;
        }
        return true;
    }

    private void ProcessNodeEvents(Event e)
    {
        bool guiChanged = false;
        if (bt.nodes != null)
        {
            for (int i = bt.nodes.Count - 1; i >= 0; i--)
            {
                if (bt.nodes[i].ProcessEvents(e)){
                    guiChanged = true;
                }
           }
        }

        if (selectedNode != null){
            if (!selectedNode.IsSelected()){
                selectedNode = null;
            }
        }

        if (bt.connections != null){
            for (int i = bt.connections.Count - 1; i>=0; i--){
                if (bt.connections[i].ProcessProbabilityWeightEvents(e)){
                    guiChanged = true;
                }
            }
        }
        if (guiChanged){
            GUI.changed = true;
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedChildPoint != null && selectedParentPoint == null)
        {
            Handles.DrawBezier(
                selectedChildPoint.GetRect().center,
                e.mousePosition,
                selectedChildPoint.GetRect().center,
                selectedChildPoint.GetRect().center,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
            drawingLine = true;
        }
        else{
            drawingLine = false;
        }
    }

    private void ProcessCreateContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        foreach(NodeType nodeType in decisionNodeTypes){
            string nodeTypeString = NodeBase.GetDefaultStringFromNodeType(nodeType);
            genericMenu.AddItem(new GUIContent(nodeTypeString), false, () => OnClickAddNode(mousePosition, nodeType, nodeTypeString)); 
        }
        foreach(string taskName in customTaskNames){
            genericMenu.AddItem(new GUIContent("Actions/"+taskName), false, () => OnClickAddNode(mousePosition, NodeType.Action, taskName)); 
        }
        genericMenu.ShowAsContext();

    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (bt.nodes != null)
        {
            for (int i = 0; i < bt.nodes.Count; i++)
            {
                bt.nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition, NodeType nodeType, string name)
    {
        if (bt.nodes == null)
        {
            bt.nodes = new List<GUINode>();
        }

        if (nodeType == NodeType.Action){
            bt.nodes.Add(new GUIActionNode(
                                name,
                                mousePosition, 
                                guiNodeSize,
                                actionStyle, 
                                selectedActionStyle, 
                                childPointStyle, 
                                parentPointStyle, 
                                callNumberStyle,
                                decoratorStyle,
                                selectedDecoratorStyle,
                                UpdatePanelDetails,
                                OnClickChildPoint, 
                                OnClickParentPoint, 
                                OnClickRemoveNode,
                                bt,
                                customTaskNames));
        }
        else{
            GUIStyle nodeStyle;
            GUIStyle selectedNodeStyle;
            switch(nodeType){
                case NodeType.SequenceSelector:
                    nodeStyle = sequenceSelectorStyle;
                    selectedNodeStyle = selectedSequenceSelectorStyle;
                    break;
                case NodeType.PrioritySelector:
                    nodeStyle = prioritySelectorStyle;
                    selectedNodeStyle = selectedPrioritySelectorStyle;
                    break;
                case NodeType.ProbabilitySelector:
                    nodeStyle = probabilitySelectorStyle;
                    selectedNodeStyle = selectedProbabilitySelectorStyle;
                    break;
                default:
                    throw new Exception("Unknown node type");
            }
            bt.nodes.Add(new GUINode(
                                nodeType,
                                mousePosition, 
                                guiNodeSize,
                                nodeStyle, 
                                selectedNodeStyle, 
                                childPointStyle, 
                                parentPointStyle, 
                                callNumberStyle,
                                decoratorStyle,
                                selectedDecoratorStyle,
                                UpdatePanelDetails,
                                OnClickChildPoint, 
                                OnClickParentPoint, 
                                OnClickRemoveNode,
                                bt));
        }
        selectedParentPoint = bt.nodes[bt.nodes.Count-1].GetParentPoint();
        CreateConnection();
        ClearConnectionSelection();
        int numNodes = UpdateCallNumbers(bt.nodes[0], 1);
    }

    

    private void OnClickChildPoint(ConnectionPoint ChildPoint)
    {
        selectedChildPoint = ChildPoint;

        if (selectedParentPoint != null)
        {
            if (selectedParentPoint.node != selectedChildPoint.node)
            {
                if (selectedParentPoint.node.GetName() != "Root"){
                    CreateConnection();
                    ClearConnectionSelection(); 
                }
                else
                {
                    ClearConnectionSelection(); 
                }
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
                if (selectedParentPoint.node.GetName() != "Root"){
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else
                {
                    ClearConnectionSelection();
                }
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(GUINode node)
    {
        RemoveNode(node);
    }

    private void RemoveNode(GUINode node){
        if (bt.connections != null)
        {
            bt.connections.Remove(node.GetParentNode());
            node.RemoveParentNode();
            
            List<Connection> childNodes = node.GetChildNodes();
            if (childNodes != null){
                for (int i=childNodes.Count-1; i>0; i--){
                    node.RemoveChildNode(childNodes[i]);
                    bt.connections.Remove(childNodes[i]);
                }
            }
        }

        bt.nodes.Remove(node);

    }

    private void OnClickRemoveConnection(Connection connection)
    {
        bt.connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (bt.connections == null)
        {
            bt.connections = new List<Connection>();
        }

        Connection newConnection = new Connection(selectedChildPoint, selectedParentPoint, OnClickRemoveConnection);
        GUINode parentNode = selectedChildPoint.GetNode();
        GUINode childNode = selectedParentPoint.GetNode();
        parentNode.AddChildNode(newConnection);
        childNode.SetParentNode(newConnection);
        if (parentNode.GetTask() == "Probability Selector"){
            newConnection.AddProbabilityWeight(subNodeSize,
                                               probabilityWeightStyle,
                                               selectedProbabilityWeightStyle,
                                               UpdatePanelDetails,
                                               bt);
        } 
        bt.connections.Add(newConnection);
    }

    private void ClearConnectionSelection()
    {
        selectedChildPoint = null;
        selectedParentPoint = null;
    }

    private int UpdateCallNumbers(GUINode startingNode, int callNumber){

        // Update decorators first 
        List<GUIDecorator> decorators = startingNode.GetDecorators();
        if (decorators != null){
            foreach(GUIDecorator decorator in decorators){
                decorator.SetCallNumber(callNumber);
                callNumber++;
            }
        }

        // Update nodes
        startingNode.SetCallNumber(callNumber);
        callNumber++;

        // Update child nodes
        startingNode.RefreshChildOrder();
        foreach(Connection connection in startingNode.GetChildNodes()){
            callNumber = UpdateCallNumbers(connection.GetChildNode(), callNumber);
        }
        return callNumber;
    }

    private void RefreshDecoratorTasks(string oldKeyName, string newKeyName){
        if (bt.nodes!=null){
            foreach(GUINode node in bt.nodes){
                node.RefreshDecoratorTasks(oldKeyName, newKeyName);
            }
        }
    }

     List<BehaviourTreeTask> GetCustomTasks()
     {
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

}