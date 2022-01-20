using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Behaviour{
    public class BehaviourTreeEditor : EditorWindow
    {
        List<NodeType> decisionNodeTypes;

        // Bookkeeping
        public BehaviourTree bt;
        public List<CompositeGuiNode> guiNodes;
        public List<Connection> connections;
        private GuiNode selectedNode;
        private ConnectionPoint selectedChildPoint;
        private ConnectionPoint selectedParentPoint;

        // Movement
        private Vector2 offset;
        private Vector2 drag;
        private bool drawingLine;

        // Details panel
        private Rect detailsPanel;
        private string[] toolbarStrings = {"Selected Node", "Blackboard"};
        private enum ToolbarTab{
            SelectedNode = 0,
            Blackboard = 1
        }
        private int toolbarInt;

        List<string> customTaskNames;
        Texture2D arrowTexture;

        // Blackboard
        string[] activeBlackboardKey = {"","",""};
        bool renamingBlackboardKey = false;

        public void SetScriptableObject(BehaviourTree behaviourTree){
            bt = behaviourTree;
            guiNodes = new List<CompositeGuiNode>();
            connections = new List<Connection>();
            if (bt.rootNode == null){
                AddRootNode();
            }
            else{
                LoadFromRoot(
                    rootNode:bt.rootNode,
                    nodeMetaData:bt.nodeMetaData
                );
            }
            bt.guiRootNode = guiNodes[0];
        }

        private void OnEnable()
        {
            customTaskNames = GetCustomTaskNames();
            decisionNodeTypes = new List<NodeType>{NodeType.PrioritySelector, 
                                                NodeType.ProbabilitySelector,
                                                NodeType.SequenceSelector};
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
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            if (bt != null && guiNodes != null ){
                DrawConnections();
                DrawConnectionLine(Event.current);
                DrawNodes();

                DrawDetailsPanel();

                if (MousePosOnGrid(Event.current.mousePosition)){
                    ProcessNodeEvents(Event.current);
                }
                ProcessEvents(Event.current);

                if (GUI.changed) {
                    Repaint();
                    UpdateCallNumbers(guiNodes[0], 1);
                }
            }

        }

        private void DrawDetailsPanel(){
            BeginWindows();
            detailsPanel = new Rect(position.width*.8f, 
                                    0, 
                                    position.width*.2f, 
                                    position.height);
            detailsPanel = GUILayout.Window(1, 
                                            detailsPanel, 
                                            DrawDetailInfo, 
                                            "Details");
            EndWindows();
        }

        public void UpdatePanelDetails(GuiNode node){
            selectedNode = node;
        }

        void DrawDetailInfo(int unusedWindowID)
        {
            if (GUILayout.Button("Save")){
                SaveTree();
            }
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
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
            if (GUILayout.Button("Clear All")){
                for (int i=guiNodes.Count-1; i>0;i--){
                    if (selectedNode == guiNodes[i]){
                        selectedNode = null;
                    }
                    RemoveNode(guiNodes[i]);
                }
                ResetRootNode();
            }
            if (selectedNode != null && selectedNode.IsSelected == false){
                selectedNode = null;
            }
            if (selectedNode != null){
                selectedNode.DrawDetails();
            }
        }

        void DrawKey(string keyName, string keyType){

            // Active text field for renaming
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
                        else if (keyType == "float" || keyType == "int"){
                            RefreshProbabilityWeightTasks(keyName, newName);
                        }
                    }
                    else{
                        newName = activeBlackboardKey[0];
                    }
                    if (GUILayout.Button(newName + " (" + keyType + ")")){
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
                }
                activeBlackboardKey[0] = "";
                activeBlackboardKey[1] = "";
            }
            else{
                // draw button
                if (GUILayout.Button(keyName + " (" + keyType + ")")){
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

        private void ProcessEvents(Event e)
        {
            //drag = Vector2.zero;

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
                            e.Use();
                        }
                    }
                break;
                case EventType.KeyDown:
                    if (renamingBlackboardKey){
                        renamingBlackboardKey = false;
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
            GuiNode currentSeletedNode = selectedNode; 
            if (guiNodes != null)
            {
                for (int i = guiNodes.Count - 1; i >= 0; i--)
                {
                    if (guiNodes[i].ProcessEvents(e)){
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
                    if (connections[i].ProcessProbabilityWeightEvents(e)){
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

        private void ProcessCreateContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            foreach(NodeType nodeType in decisionNodeTypes){
                string nodeTypeString = NodeProperties.GetDefaultStringFromNodeType(nodeType);
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
            }

            GUI.changed = true;
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
                            pos:mousePosition,
                            parentConnection:null,
                            UpdatePanelDetails:UpdatePanelDetails,
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
                            pos:mousePosition,
                            parentConnection:null,
                            UpdatePanelDetails:UpdatePanelDetails,
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
                            pos:mousePosition,
                            parentConnection:null,
                            UpdatePanelDetails:UpdatePanelDetails,
                            OnRemoveNode:OnClickRemoveNode,
                            OnClickChildPoint:OnClickChildPoint,
                            OnClickParentPoint:OnClickParentPoint,
                            blackboard:ref bt.blackboard
                        )
                    );
                    break;
                case NodeType.Action:
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
                            pos:mousePosition,
                            parentConnection:null,
                            UpdatePanelDetails:UpdatePanelDetails,
                            OnRemoveNode:OnClickRemoveNode,
                            OnClickChildPoint:OnClickChildPoint,
                            OnClickParentPoint:OnClickParentPoint,
                            blackboard:ref bt.blackboard
                        )
                    );
                    break;
                default:
                    throw new Exception($"Unexpected node type {nodeType}");
            }

            selectedParentPoint = guiNodes[guiNodes.Count-1].ParentPoint;
            CreateConnection();
            ClearConnectionSelection();
            guiNodes[guiNodes.Count-1].SetParentConnection(connections[connections.Count-1]);
            int numNodes = UpdateCallNumbers(guiNodes[0], 1);
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
                    CreateConnection();
                    ClearConnectionSelection();
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
            node.BtNode.Unlink();

            // Remove the GuiNode
            guiNodes.Remove(node);
            GUI.changed = true;

        }

        private void OnClickRemoveConnection(Connection connection)
        {
            connections.Remove(connection);
            connection.GetChildNode().RemoveParentConnection();
            connection.GetParentNode().RemoveChildConnection(connection);
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
                                  string taskName="Constant weight",
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

            //Update Gui
            connection.AddProbabilityWeight(
                node:probabilityWeight,
                displayTask:taskName,
                displayName:displayName,
                UpdatePanelDetails:UpdatePanelDetails,
                blackboard:ref bt.blackboard
            );
        }

        void AddProbabilityWeight(Connection connection, 
                                  GuiProbabilityWeight guiProbabilityWeight
                                  ){
            // Update behaviour tree
            Node childNode = connection.GetChildNode().BtNode;
            Node parentNode = connection.GetParentNode().BtNode;
            ProbabilityWeight probabilityWeight = new ProbabilityWeight(
                taskName:guiProbabilityWeight.BtNode.TaskName,
                parentNode:parentNode,
                childNode:childNode
            );              
            parentNode.RemoveChildNode(childNode);
            parentNode.AddChildNode(probabilityWeight);
            childNode.SetParentNode(probabilityWeight);

            //Update Gui
            connection.AddProbabilityWeight(
                node:probabilityWeight,
                displayTask:guiProbabilityWeight.DisplayTask,
                displayName:guiProbabilityWeight.DisplayName,
                UpdatePanelDetails:UpdatePanelDetails,
                blackboard:ref bt.blackboard
            );
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
        }

        private void CreateConnection(
            ConnectionPoint childPoint, 
            ConnectionPoint parentPoint, 
            GuiProbabilityWeight probabilityWeight){
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
                AddProbabilityWeight(newConnection, probabilityWeight);
                connections.Add(newConnection);
                GUI.changed = true;
            }

        }

        private void ClearConnectionSelection()
        {
            selectedChildPoint = null;
            selectedParentPoint = null;
        }

        private int UpdateCallNumbers(CompositeGuiNode startingNode, int callNumber){

            // Update decorators first 
            List<GuiDecorator> decorators = startingNode.Decorators;
            if (decorators != null){
                foreach(GuiDecorator decorator in decorators){
                    decorator.SetCallNumber(callNumber);
                    callNumber++;
                }
            }

            // Update guiNodes
            startingNode.SetCallNumber(callNumber);
            callNumber++;

            // Update child guiNodes
            startingNode.RefreshChildOrder();
            foreach(Connection connection in startingNode.ChildConnections){
                callNumber = UpdateCallNumbers(connection.GetChildNode(), callNumber);
            }
            return callNumber;
        }

        private void ResetCallNumber(CompositeGuiNode startingNode){
            // Update decorators first 
            List<GuiDecorator> decorators = startingNode.Decorators;
            if (decorators != null){
                foreach(GuiDecorator decorator in decorators){
                    decorator.SetCallNumber(-1);
                }
            }

            // Update guiNodes
            startingNode.SetCallNumber(-1);

            foreach(Connection connection in startingNode.ChildConnections){
                ResetCallNumber(connection.GetChildNode());
            }

        }

        private void RefreshDecoratorTasks(string oldKeyName, string newKeyName){
            if (guiNodes!=null){
                foreach(CompositeGuiNode node in guiNodes){
                    node.RefreshDecoratorTasks(oldKeyName, newKeyName);
                }
            }
        }

        private void RefreshProbabilityWeightTasks(string oldKeyName, string newKeyName){
            if (guiNodes!=null){
                foreach(CompositeGuiNode node in guiNodes){
                    if (node is GuiProbabilitySelector probabilitySelectorNode){
                        if (probabilitySelectorNode.ChildConnections != null){
                            foreach(Connection connection in probabilitySelectorNode.ChildConnections){
                                connection.RefreshProbabilityWeightTask(oldKeyName, newKeyName);
                            }
                        }
                    }
                }
            }
        }

        bool WeightKeyInUse(string keyName){
            if (guiNodes!=null){
                foreach(CompositeGuiNode node in guiNodes){
                    if (node is GuiProbabilitySelector probabilitySelectorNode){
                        if (probabilitySelectorNode.ChildConnections != null){
                            foreach(Connection connection in probabilitySelectorNode.ChildConnections){
                                if (connection.GetProbabilityWeightKey() == keyName){
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }


        bool BoolKeyInUse(string keyName){
            if (guiNodes != null){
                foreach(CompositeGuiNode node in guiNodes){
                    if (node.Decorators != null){
                        foreach (GuiDecorator decorator in node.Decorators){
                            if (decorator.DisplayTask == keyName){
                                return true;
                            }
                        }
                    }                    
                }
            }
            return false;
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
            AssetDatabase.SaveAssets();
        }

        void AddNodeAndConnections(
            Node node, 
            ref int idx,
            List<GuiNodeData> nodeMetaData,
            int parentGuiNodeIdx=-1,
            List<GuiDecorator> decorators=null,
            GuiProbabilityWeight probabilityWeight=null){

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
                    decorators:decorators
                    );
            }

            else if (node is ProbabilityWeight weightNode){
                probabilityWeight = (GuiProbabilityWeight)guiNode;
                probabilityWeight.SetEditorActions(
                    UpdatePanelDetails:UpdatePanelDetails
                );
                idx++;
                AddNodeAndConnections(
                    node:node.ChildNodes[0],
                    idx:ref idx, 
                    nodeMetaData:nodeMetaData,
                    parentGuiNodeIdx:parentGuiNodeIdx,
                    decorators:decorators
                    );
            }
            else {
                // Assumed to be CompositeNode -> now need to make connections
                CompositeGuiNode cgn = (CompositeGuiNode)guiNode;
                if (decorators != null){
                    for(int i=decorators.Count-1; i>=0; i--){
                        cgn.AddDecorator(decorators[i]);
                    }
                }
                cgn.SetEditorActions(
                    UpdatePanelDetails:UpdatePanelDetails,
                    OnRemoveNode:OnClickRemoveNode,
                    OnClickChildPoint:OnClickChildPoint,
                    OnClickParentPoint:OnClickParentPoint
                );
                if (parentGuiNodeIdx != -1){
                    CreateConnection(
                        parentPoint:cgn.ParentPoint, 
                        childPoint:guiNodes[parentGuiNodeIdx].ChildPoint,
                        probabilityWeight:probabilityWeight
                        );
                }
                guiNodes.Add(cgn);
                parentGuiNodeIdx=guiNodes.Count-1;
                idx++;
                decorators = null;
                probabilityWeight = null;
                foreach(Node child in node.ChildNodes){
                    AddNodeAndConnections(
                        node:child,
                        idx:ref idx, 
                        nodeMetaData:nodeMetaData,
                        parentGuiNodeIdx:parentGuiNodeIdx,
                        decorators:decorators,
                        probabilityWeight:probabilityWeight
                        );
                    
                }

            }
        }


    }
}