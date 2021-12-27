using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace BehaviourBase{
    public class NodeBasedEditor : EditorWindow
    {
        // Node Properties
        public NodeColors nodeColors;
        public NodeStyles nodeStyles;
        public NodeSizes nodeSizes;
        List<NodeType> decisionNodeTypes;

        // Bookkeeping
        public BehaviourTree bt;
        private AggregateNode selectedNode;
        private ConnectionPoint selectedChildPoint;
        private ConnectionPoint selectedParentPoint;

        // Movement
        private Vector2 offset;
        private Vector2 drag;
        private bool drawingLine;

        // Details panel
        private Rect detailsPanel;
        private string[] toolbarStrings = {"Selected Node", "Blackboard"};
        private int toolbarInt = 0;

        List<string> customTaskNames;
        Texture2D arrowTexture;

        // Blackboard
        string[] activeBlackboardKey = {"","",""};
        bool renamingBlackboardKey = false;

        public void SetScriptableObject(BehaviourTree behaviourTree){
            bt = behaviourTree;
            if (bt.nodes == null){
                AddRootNode();
            }
        }

        private void OnEnable()
        {
            nodeSizes = new NodeSizes();
            nodeStyles = new NodeStyles();
            nodeColors = new NodeColors();
            decisionNodeTypes = new List<NodeType>{NodeType.PrioritySelector, 
                                                NodeType.ProbabilitySelector,
                                                NodeType.SequenceSelector};
            customTaskNames = GetCustomTaskNames();
            arrowTexture = NodeProperties.GetArrowTexture();
        }

        private void AddRootNode(){

            bt.nodes = new List<AggregateNode>();
            bt.nodes.Add(new AggregateNode(
                                nodeType:NodeType.Root,
                                displayTask:"Root",
                                displayName:"",
                                new Rect(0,0, 
                                        nodeSizes.guiNodeSize[0], 
                                        nodeSizes.guiNodeSize[1]), 
                                null,
                                UpdatePanelDetails,
                                nodeStyles,
                                nodeColors,
                                OnClickChildPoint, 
                                OnClickParentPoint, 
                                OnClickRemoveNode,
                                ref bt.blackboard
                                ));
        }

        private void ResetRootNode(){
            bt.nodes[0].displayName = "";
            bt.nodes[0].SetPosition(new Vector2(0,0));
        }

        private void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            DrawNodes();
            DrawConnections();

            DrawConnectionLine(Event.current);
            DrawDetailsPanel();

            if (MousePosOnGrid(Event.current.mousePosition)){
                ProcessNodeEvents(Event.current);
            }
            ProcessEvents(Event.current);

            if (GUI.changed) {
                Repaint();
                UpdateCallNumbers(bt.nodes[0], 1);
            }

        }

        private void DrawDetailsPanel(){
            BeginWindows();
            detailsPanel = new Rect(position.width*.8f, 0, position.width*.2f, position.height);
            detailsPanel = GUILayout.Window(1, detailsPanel, DrawDetailInfo, "Details");
            EndWindows();
        }

        public void UpdatePanelDetails(AggregateNode node){
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
                GUILayout.Label("Task: " + selectedNode.displayTask);
                GUILayout.Label("Name");
                selectedNode.displayName = GUILayout.TextField(selectedNode.displayName, 50);
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
                if (!selectedNode.isSelected){
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
                Vector2 startPos = selectedChildPoint.GetRect().center;
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


        private void DrawConnectionArrow(Vector2 startPos, Vector2 mousePos){
            float angle = Mathf.Acos(Vector2.Dot(Vector2.up, (startPos - mousePos).normalized));  
            angle *= 180f/Mathf.PI;
            if (startPos.x - mousePos.x > 0){
                angle *= -1;
            }
            Debug.Log(mousePos + " " + angle);
            GUIUtility.RotateAroundPivot(angle, mousePos);
            GUI.DrawTexture(new Rect(mousePos.x - arrowTexture.width/2f, 
                                    mousePos.y - arrowTexture.height/2f, 
                                    arrowTexture.width, 
                                    arrowTexture.height), 
                                    arrowTexture, 
                                    ScaleMode.StretchToFill);
            GUIUtility.RotateAroundPivot(-angle, mousePos);
            
        }


        private void ProcessCreateContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            foreach(NodeType nodeType in decisionNodeTypes){
                string nodeTypeString = Node.GetDefaultStringFromNodeType(nodeType);
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

        private void OnClickAddNode(Vector2 mousePosition, NodeType nodeType, string displayTask)
        {
            if (bt.nodes == null)
            {
                bt.nodes = new List<AggregateNode>();
            }

            switch(nodeType){
                case NodeType.SequenceSelector:
                    bt.nodes.Add(
                        new SequenceSelector(
                            displayTask,
                            "",
                            new Rect(
                                mousePosition.x,
                                mousePosition.y,
                                nodeSizes.guiNodeSize[0],
                                nodeSizes.guiNodeSize[1] 
                            ),
                            selectedChildPoint.node,
                            UpdatePanelDetails,
                            nodeStyles,
                            nodeColors,
                            OnClickChildPoint,
                            OnClickParentPoint,
                            OnClickRemoveNode,
                            ref bt.blackboard
                        )
                    );
                    break;
                case NodeType.PrioritySelector:
                    bt.nodes.Add(
                        new PrioritySelector(
                            displayTask,
                            "",
                            new Rect(
                                mousePosition.x,
                                mousePosition.y,
                                nodeSizes.guiNodeSize[0],
                                nodeSizes.guiNodeSize[1] 
                            ),
                            selectedChildPoint.node,
                            UpdatePanelDetails,
                            nodeStyles,
                            nodeColors,
                            OnClickChildPoint,
                            OnClickParentPoint,
                            OnClickRemoveNode,
                            ref bt.blackboard
                        )
                    );
                    break;
                case NodeType.ProbabilitySelector:
                    bt.nodes.Add(
                        new ProbabilitySelector(
                            displayTask,
                            "",
                            new Rect(
                                mousePosition.x,
                                mousePosition.y,
                                nodeSizes.guiNodeSize[0],
                                nodeSizes.guiNodeSize[1] 
                            ),
                            selectedChildPoint.node,
                            UpdatePanelDetails,
                            nodeStyles,
                            nodeColors,
                            OnClickChildPoint,
                            OnClickParentPoint,
                            OnClickRemoveNode,
                            ref bt.blackboard
                        )
                    );
                    break;
                case NodeType.Action:
                    bt.nodes.Add(
                        new ActionNode(
                            displayTask,
                            "",
                            new Rect(
                                mousePosition.x,
                                mousePosition.y,
                                nodeSizes.guiNodeSize[0],
                                nodeSizes.guiNodeSize[1] 
                            ),
                            selectedChildPoint.node,
                            UpdatePanelDetails,
                            nodeStyles,
                            nodeColors,
                            OnClickChildPoint,
                            OnClickParentPoint,
                            OnClickRemoveNode,
                            ref bt.blackboard
                        )
                    );
                    break;
                default:
                    throw new Exception($"Unexpected node type {nodeType}");
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
                    if (selectedParentPoint.node.nodeType != NodeType.Root){
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
                    if (selectedParentPoint.node.nodeType != NodeType.Root){
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

        private void OnClickRemoveNode(AggregateNode node)
        {
            RemoveNode(node);
        }

        private void RemoveNode(AggregateNode node){
            if (bt.connections != null)
            {
                bt.connections.Remove(node.GetParentConnection());
                node.RemoveParentConnection();
                
                List<Connection> childConnections = node.GetChildConnections();
                if (childConnections != null){
                    for (int i=childConnections.Count-1; i>0; i--){
                        bt.connections.Remove(childConnections[i]);
                        node.RemoveChildConnection(childConnections[i]);
                    }
                }
            }

            bt.nodes.Remove(node);

        }

        private void OnClickRemoveConnection(Connection connection)
        {
            bt.connections.Remove(connection);
            connection.GetChildNode().RemoveParentConnection();
            connection.GetParentNode().RemoveChildConnection(connection);
        }

        private void CreateConnection()
        {
            if (bt.connections == null)
            {
                bt.connections = new List<Connection>();
            }

            Connection newConnection = new Connection(selectedChildPoint, selectedParentPoint, OnClickRemoveConnection);
            AggregateNode parentNode = selectedChildPoint.GetNode();
            AggregateNode childNode = selectedParentPoint.GetNode();
            parentNode.AddChildConnection(newConnection);
            childNode.SetParentConnection(newConnection);
            if (parentNode.nodeType == NodeType.ProbabilitySelector){
                newConnection.AddProbabilityWeight(nodeSizes.subNodeSize,
                                                nodeStyles,
                                                nodeColors,
                                                UpdatePanelDetails,
                                                ref bt.blackboard,
                                                newConnection);
            } 
            bt.connections.Add(newConnection);
            drawingLine = false;
        }

        private void ClearConnectionSelection()
        {
            selectedChildPoint = null;
            selectedParentPoint = null;
        }

        private int UpdateCallNumbers(AggregateNode startingNode, int callNumber){

            // Update decorators first 
            List<Decorator> decorators = startingNode.GetDecorators();
            if (decorators != null){
                foreach(Decorator decorator in decorators){
                    decorator.SetCallNumber(callNumber);
                    callNumber++;
                }
            }

            // Update nodes
            startingNode.SetCallNumber(callNumber);
            callNumber++;

            // Update child nodes
            startingNode.RefreshChildOrder();
            foreach(Connection connection in startingNode.GetChildConnections()){
                callNumber = UpdateCallNumbers(connection.GetChildNode(), callNumber);
            }
            return callNumber;
        }

        private void RefreshDecoratorTasks(string oldKeyName, string newKeyName){
            if (bt.nodes!=null){
                foreach(AggregateNode node in bt.nodes){
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
}