using System;
using UnityEditor;
using UnityEngine;

namespace BehaviourBase{
    public class Connection
    {
        public ConnectionPoint childPoint;
        private CompositeGuiNode childNode;
        public ConnectionPoint parentPoint;
        private CompoiteGuiNode parentNode;
        public Action<Connection> OnClickRemoveConnection;
        private GuiProbabilityWeight probabilityWeight;

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
            Rect probabilityWeightRect = new Rect(centrePos.x, centrePos.y, size.x, size.y);

            probabilityWeight = new ProbabilityWeight(
                node:node,
                displayTask:displayTask,
                pos:centrePos,
                UpdatePanelDetails:UpdatePanelDetails,
                blackboard:ref blackboard,
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
            genericMenu.AddItem(new GUIContent("Remove connection"), false, () => OnClickRemoveConnection(this));
            genericMenu.ShowAsContext();

        }

        public string GetProbabilityWeightKey(){
            if (HasProbabilityWeight()){
                return probabilityWeight.displayTask;
            }
            return "";
        }
    }
}