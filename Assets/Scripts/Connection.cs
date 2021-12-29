using System;
using UnityEditor;
using UnityEngine;

namespace BehaviourBase{
    public class Connection
    {
        public ConnectionPoint childPoint;
        private AggregateNode childNode;
        public ConnectionPoint parentPoint;
        private AggregateNode parentNode;
        public Action<Connection> OnClickRemoveConnection;
        private ProbabilityWeight probabilityWeight;

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
                                         NodeStyles nodeStyles,
                                         NodeColors nodeColors,
                                         Action<AggregateNode> UpdatePanelDetails,
                                         ref BehaviourTreeBlackboard blackboard,
                                         Connection parentConnection
                                        ){
            Vector2 centrePos = GetCentrePos();
            Rect probabilityWeightRect = new Rect(centrePos.x, centrePos.y, size.x, size.y);

            probabilityWeight = new ProbabilityWeight("Constant weight (1)",
                                                      probabilityWeightRect,
                                                            nodeStyles,
                                                            nodeColors,
                                                            UpdatePanelDetails,
                                                            ref blackboard,
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

        public AggregateNode GetParentNode(){
            return parentNode;
        }
        public AggregateNode GetChildNode(){
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