using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NodeBasedEditor : EditorWindow
{
    // Bookkeeping
    private List<GUINode> nodes;
    private GUINode selectedNode;
    private List<Connection> connections;

    // Styles
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle ChildPointStyle;
    private GUIStyle ParentPointStyle;
    private Vector2 nodeSize = new Vector2(200, 100);

    private ConnectionPoint selectedChildPoint;
    private ConnectionPoint selectedParentPoint;

    // Movement
    private Vector2 offset;
    private Vector2 drag;
    private bool drawingLine;

    private Rect detailsPanel;

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.normal.textColor = Color.white;

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle.normal.textColor = Color.white;

        ChildPointStyle = new GUIStyle();
        ChildPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn.png") as Texture2D;
        ChildPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn on.png") as Texture2D;
        ChildPointStyle.border = new RectOffset(4, 4, 12, 12);

        ParentPointStyle = new GUIStyle();
        ParentPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn.png") as Texture2D;
        ParentPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn on.png") as Texture2D;
        ParentPointStyle.border = new RectOffset(4, 4, 12, 12);
        AddRootNode();
    }

    private void AddRootNode(){

        nodes = new List<GUINode>();
        nodes.Add(new GUINode("Root",
                              new Vector2(0,0), 
                              nodeSize[0], 
                              nodeSize[1], 
                              nodeStyle, 
                              selectedNodeStyle, 
                              ChildPointStyle, 
                              ParentPointStyle, 
                              UpdatePanelDetails,
                              OnClickChildPoint, 
                              OnClickParentPoint, 
                              OnClickRemoveNode));
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        DrawDetailsPanel();

        if (GUI.changed) Repaint();
    }

    private void DrawDetailsPanel(){
        BeginWindows();
        detailsPanel = new Rect(position.width*.8f, 0, position.width*.2f, position.height);
        detailsPanel = GUILayout.Window(1, detailsPanel, DrawDetailInfo, "Details");
        EndWindows();
    }

    public void UpdatePanelDetails(GUINode node){
        selectedNode = node;
    }

    void DrawDetailInfo(int unusedWindowID)
    {
        if (selectedNode != null){
            GUILayout.Label("Task: " + selectedNode.GetTask());
            GUILayout.Label("Name");
            selectedNode.SetName(GUILayout.TextField(selectedNode.GetName(), 50));
        }
        
        GUI.DragWindow();
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
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
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
                    ClearConnectionSelection();
                }
            break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
            break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedChildPoint != null && selectedParentPoint == null)
        {
            Handles.DrawBezier(
                selectedChildPoint.rect.center,
                e.mousePosition,
                selectedChildPoint.rect.center,
                selectedChildPoint.rect.center,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
            drawingLine = true;
        }

        else if (selectedParentPoint != null && selectedChildPoint == null)
        {
            Handles.DrawBezier(
                selectedParentPoint.rect.center,
                e.mousePosition,
                selectedParentPoint.rect.center,
                selectedParentPoint.rect.center,
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

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Selector"), false, () => OnClickAddNode(mousePosition, "Selector")); 
        genericMenu.AddItem(new GUIContent("Add Sequence"), false, () => OnClickAddNode(mousePosition, "Sequence")); 
        genericMenu.ShowAsContext();
    }

    private void ProcessCreateContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Selector"), false, () => OnClickAddNode(mousePosition, "Selector")); 
        genericMenu.AddItem(new GUIContent("Add Sequence"), false, () => OnClickAddNode(mousePosition, "Sequence")); 
        genericMenu.ShowAsContext();

    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition, string name)
    {
        if (nodes == null)
        {
            nodes = new List<GUINode>();
        }

        nodes.Add(new GUINode(
                              name,
                              mousePosition, 
                              nodeSize[0], 
                              nodeSize[1], 
                              nodeStyle, 
                              selectedNodeStyle, 
                              ChildPointStyle, 
                              ParentPointStyle, 
                              UpdatePanelDetails,
                              OnClickChildPoint, 
                              OnClickParentPoint, 
                              OnClickRemoveNode));
        selectedParentPoint = nodes[nodes.Count-1].GetParentPoint();
        CreateConnection();
        ClearConnectionSelection();
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
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].ChildPoint == node.ChildPoint || connections[i].ParentPoint == node.ParentPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedChildPoint, selectedParentPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedChildPoint = null;
        selectedParentPoint = null;
    }
}