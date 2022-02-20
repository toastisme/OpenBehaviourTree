using System;
using UnityEditor;
using UnityEngine;

namespace Behaviour{
public class Connection
{
    /**
     * \class Connection
     * Stores and displays a connection between CompositeGuiNodes,
     * and a GuiProbabilyWeight if a connection with a GuiProbabilitySelector
     * parent node.
     */

    public ConnectionPoint childPoint;
    public CompositeGuiNode childNode;
    public ConnectionPoint parentPoint;
    private CompositeGuiNode parentNode;
    public Action<Connection> OnClickRemoveConnection;
    public GuiProbabilityWeight probabilityWeight;

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

    public void AddProbabilityWeight(GuiProbabilityWeight guiNode){
        probabilityWeight = guiNode;
        guiNode.SetParentConnection(this);
        Vector2 size = BehaviourTreeProperties.SubNodeSize();
        probabilityWeightOffset = new Vector2(size.x*.5f, size.y);
    }

    public void AddProbabilityWeight(ProbabilityWeight node,
                                        string displayTask,
                                        string displayName,
                                        Action<GuiNode> UpdatePanelDetails,
                                        Action TreeModified,
                                        Action NodeUpdated,
                                        ref BehaviourTreeBlackboard blackboard
                                    ){
        Vector2 centrePos = GetCentrePos();
        Vector2 size = BehaviourTreeProperties.SubNodeSize();
        Rect probabilityWeightRect = new Rect(centrePos.x, centrePos.y, size.x, size.y);

        probabilityWeight = new GuiProbabilityWeight(
            node:node,
            displayTask:displayTask,
            displayName:displayName,
            pos:centrePos,
            UpdatePanelDetails:UpdatePanelDetails,
            TreeModified:TreeModified,
            NodeUpdated:NodeUpdated,
            blackboard:ref blackboard,
            parentConnection:this
                                                        );
        probabilityWeightOffset = new Vector2(size.x*.5f, size.y);
    }
    public void UpdateWeightPosition(){
        if (HasProbabilityWeight()){
            probabilityWeight.Move(childPoint.GetRect().center - probabilityWeightOffset);
        }
    }

    public bool HasProbabilityWeight(){
        return (probabilityWeight != null);
    }

    public bool ProcessProbabilityWeightEvents(Event e, Vector2 mousePos){
        if (HasProbabilityWeight()){
            UpdateWeightPosition();
            return probabilityWeight.ProcessEvents(e, mousePos);
        }
        return false;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            childPoint.GetScaledRect().center,
            parentPoint.GetScaledRect().center,
            childPoint.GetScaledRect().center,
            parentPoint.GetScaledRect().center,
            Color.white,
            null,
            4f
        );

        if (HasProbabilityWeight()){
            probabilityWeight.Draw();
        }

        else if (Handles.Button((childPoint.GetScaledRect().center + parentPoint.GetScaledRect().center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            ProcessContextMenu();
        }
    }

    public CompositeGuiNode GetParentNode(){
        return parentNode;
    }
    public CompositeGuiNode GetChildNode(){
        return childNode;
    }

    public void UpdateOrigin(Vector2 origin){
        if (HasProbabilityWeight()){
            probabilityWeight.UpdateOrigin(origin);
        }
    }

    public Vector2 GetCentrePos(){
        return (childPoint.GetRect().center + parentNode.GetRect().center)*.5f;        
    }
    private void ProcessContextMenu(){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove connection"), false, () => RemoveConnection());
        genericMenu.ShowAsContext();

    }

    public void RemoveConnection(){
        if (HasProbabilityWeight()){
            probabilityWeight.SetSelected(false);
        }
        OnClickRemoveConnection(this);
    }

    public string GetProbabilityWeightKey(){
        if (HasProbabilityWeight()){
            return probabilityWeight.DisplayTask;
        }
        return "";
    }

    public void UpdateProbabilityWeightBoxWidth(float newWidth){
        probabilityWeight?.UpdateBoxWidth(newWidth);
        probabilityWeightOffset = new Vector2(
            newWidth*.5f, 
            probabilityWeightOffset.y);
    }

    public float GetRequiredProbabilityWeightBoxWidth(){
        return probabilityWeight.GetRequiredBoxWidth();
    }

    public void RefreshProbabilityWeightTask(string oldKeyName, string newKeyName){
        if (GetProbabilityWeightKey() == oldKeyName){
            probabilityWeight.SetTask(newKeyName);
        }
    }

    public void UpdateBlackboard(ref BehaviourTreeBlackboard newBlackboard){
        if (HasProbabilityWeight()){
            probabilityWeight.UpdateBlackboard(ref newBlackboard);
        }
    }
}
}