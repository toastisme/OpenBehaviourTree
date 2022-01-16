using System;
using UnityEditor;
using UnityEngine;

namespace Behaviour{
    public class Connection
    {
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

        public void AddProbabilityWeight(ProbabilityWeight node,
                                         string displayTask,
                                         string displayName,
                                         Action<GuiNode> UpdatePanelDetails,
                                         ref BehaviourTreeBlackboard blackboard
                                        ){
            Vector2 centrePos = GetCentrePos();
            Vector2 size = NodeProperties.SubNodeSize();
            Rect probabilityWeightRect = new Rect(centrePos.x, centrePos.y, size.x, size.y);

            probabilityWeight = new GuiProbabilityWeight(
                node:node,
                displayTask:displayTask,
                displayName:displayName,
                pos:centrePos,
                UpdatePanelDetails:UpdatePanelDetails,
                blackboard:ref blackboard,
                parentConnection:this
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
                4f
            );

            if (HasProbabilityWeight()){
                probabilityWeight.Draw();
            }

            else if (Handles.Button((childPoint.GetRect().center + parentPoint.GetRect().center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
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

        public void RefreshProbabilityWeightTask(string oldKeyName, string newKeyName){
            if (GetProbabilityWeightKey() == oldKeyName){
                probabilityWeight.SetTask(newKeyName);
            }
        }
    }
}