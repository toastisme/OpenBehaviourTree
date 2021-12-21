using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    public ConnectionPoint childPoint;
    private GUINode childNode;
    public ConnectionPoint parentPoint;
    private GUINode parentNode;
    public Action<Connection> OnClickRemoveConnection;
    private GUIProbabilityWeight probabilityWeight;

    private Vector2 probabilityWeightOffset;


    public Connection(ConnectionPoint parentPoint,
                      ConnectionPoint childPoint,
                      Action<Connection> OnClickRemoveConnection
                      )
    {
        this.childPoint = childPoint;
        this.childNode = childPoint.GetNode();
        this.parentPoint = parentPoint;
        this.parentNode = parentPoint.GetNode();
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }   

    public void AddProbabilityWeight(Vector2 size, 
                                     GUIStyle nodeStyle,
                                     GUIStyle selectedNodeStyle,
                                     Color nodeColor,
                                     Action<NodeBase> UpdatePanelDetails,
                                     BehaviourTree behaviourTree,
                                     Connection parentConnection
                                     ){

        probabilityWeight = new GUIProbabilityWeight("Constant weight (1)",
                                                        GetCentrePos(),
                                                        size,
                                                        nodeStyle,
                                                        selectedNodeStyle,
                                                        nodeColor,
                                                        UpdatePanelDetails,
                                                        behaviourTree,
                                                        this
                                                        );
        probabilityWeightOffset = new Vector2(size.x*.5f, 0);
    }
    public void UpdateWeightPosition(){
        if (HasProbabilityWeight()){
            probabilityWeight.Move(GetCentrePos() - probabilityWeightOffset);
        }
    }

    public bool HasProbabilityWeight(){
        return (probabilityWeight != null);
    }

    public bool ProcessProbabilityWeightEvents(Event e){
        if (HasProbabilityWeight()){
            UpdateWeightPosition();
            return probabilityWeight.ProcessEvents(e);
        }
        return false;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            childPoint.GetRect().center,
            parentPoint.GetRect().center,
            childPoint.GetRect().center,
            parentPoint.GetRect().center,
            Color.white,
            null,
            2f
        );

        if (HasProbabilityWeight()){
            probabilityWeight.Draw();
        }

        else if (Handles.Button((childPoint.GetRect().center + parentPoint.GetRect().center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            ProcessContextMenu();
        }
    }

    public GUINode GetParentNode(){
        return parentNode;
    }
    public GUINode GetChildNode(){
        return childNode;
    }

    public Vector2 GetCentrePos(){
        return (childPoint.GetRect().center + parentNode.GetRect().center)*.5f;        
    }
    private void ProcessContextMenu(){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove connection"), false, () => OnClickRemoveConnection(this));
        genericMenu.ShowAsContext();

    }

}