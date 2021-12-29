using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BehaviourBase{
    public class AggregateNode : Node 
    {
        /**
        * \class AggregateNode
        * Base class for covering editor methods for any node
        that can visually hold decorators and have connections to other nodes.
        */

        // GUI variables
        protected List<Decorator> decorators;
        protected NodeStyles nodeStyles;
        protected NodeColors nodeColors;
        protected Vector2 initDecoratorPos = new Vector2(0f,0f);
        protected Vector2 subNodePos;
        protected Vector2 subNodeSize = NodeProperties.SubNodeSize();
        public ConnectionPoint childPoint;
        public ConnectionPoint parentPoint;
        protected List<Connection> childConnections;
        protected Connection parentConnection;
        public Action<AggregateNode> OnRemoveNode;
        protected BehaviourTreeBlackboard blackboard;
        protected Rect subNodeRect;
        protected Action<AggregateNode> UpdatePanelDetails;

        // Constructor
        public AggregateNode(
            NodeType nodeType,
            string displayTask,
            string displayName,
            Rect rect,
            Node parentNode,
            Action<AggregateNode> UpdatePanelDetails,
            NodeStyles nodeStyles,
            NodeColors nodeColors,
            Action<ConnectionPoint> OnClickChildPoint,
            Action<ConnectionPoint> OnClickParentPoint,
            Action<AggregateNode> OnRemoveNode,
            ref BehaviourTreeBlackboard blackboard
        ) : base(
            nodeType:nodeType,
            displayTask:displayTask,
            displayName:displayName,
            rect:rect,
            parentNode:parentNode,
            defaultStyle:nodeStyles.guiNodeStyle,
            selectedStyle:nodeStyles.selectedGuiNodeStyle,
            callNumberStyle:nodeStyles.callNumberStyle,
            color:nodeColors.GetColor(nodeType),
            callNumberColor:nodeColors.callNumberColor
        )
        {
            this.nodeStyles = nodeStyles;
            this.nodeColors = nodeColors;
            this.subNodeRect = new Rect(rect.x, 
                                   rect.y+subNodeSize.y*.5f, 
                                   subNodeSize.x, 
                                   subNodeSize.y);
            this.initDecoratorPos = new Vector2(0, rect.height*.21f);
            this.callNumberRect.position = subNodeRect.position;
            this.blackboard = blackboard;
            this.decorators = new List<Decorator>();
            this.childConnections = new List<Connection>();
            this.OnRemoveNode = OnRemoveNode;
            ApplyNodeTypeSettings(nodeType, 
                                  OnClickParentPoint, 
                                  OnClickChildPoint);
            this.UpdatePanelDetails = UpdatePanelDetails;
        }

        // GUINode methods
        void ApplyNodeTypeSettings(NodeType nodeType,
                                   Action<ConnectionPoint> OnClickParentPoint,
                                   Action<ConnectionPoint> OnClickChildPoint){
            switch(nodeType){
                case NodeType.Root:
                    parentPoint = null;
                    childPoint = new ConnectionPoint(this, 
                                                     ConnectionPointType.In, 
                                                     nodeStyles.childPointStyle, 
                                                     OnClickChildPoint);
                    break;
                case NodeType.Action:
                    childPoint = null;
                    parentPoint = new ConnectionPoint(this, 
                                                      ConnectionPointType.Out, 
                                                      nodeStyles.parentPointStyle, 
                                                      OnClickParentPoint);
                    break; 
                case NodeType.Decorator:
                    childPoint = null;
                    parentPoint = null;
                    break;
                case NodeType.ProbabilityWeight:
                    childPoint = null;
                    parentPoint = null;
                    break;
                default:
                    childPoint = new ConnectionPoint(this, 
                                                     ConnectionPointType.In, 
                                                     nodeStyles.childPointStyle, 
                                                     OnClickChildPoint);
                    parentPoint = new ConnectionPoint(this, 
                                                      ConnectionPointType.Out, 
                                                      nodeStyles.parentPointStyle, 
                                                      OnClickParentPoint);
                break;
            }
        }

        public ConnectionPoint GetParentPoint(){return parentPoint;}
        public List<Decorator> GetDecorators(){return decorators;}

        public override void Drag(Vector2 delta)
        {
            base.Drag(delta);
            subNodeRect.position += delta; 
            if (decorators != null){
                foreach(Decorator decorator in decorators){
                    if (!decorator.isSelected){
                        decorator.Drag(delta);
                    }
                }
            }
        }

        public void DragWithChildren(Vector2 delta){
            base.Drag(delta);
            subNodeRect.position += delta; 
            if (childConnections != null){
                foreach(Node childNode in childNodes){
                    childNode.Drag(delta);
                }
            }
            if (decorators != null){
                foreach(Decorator decorator in decorators){
                    if (!decorator.isSelected){
                        decorator.Drag(delta);
                    }
                }
            }

        }

        public override void Draw()
        {
            if (childPoint != null){
                childPoint.Draw();
            }
            if (parentPoint != null){
                parentPoint.Draw();
            }
            Color currentColor = GUI.backgroundColor;
            if (nodeState == NodeState.Running){
                GUI.color = nodeColors.runningTint;
            }
            GUI.backgroundColor = nodeColors.defaultColor;
            if (isSelected){
                GUI.Box(rect, "", nodeStyles.selectedGuiNodeStyle);
            }
            else{
                GUI.Box(rect, "", nodeStyles.guiNodeStyle);
            }
            GUI.backgroundColor = nodeColors.GetColor(nodeType);
            GUI.Box(subNodeRect, "\n" + displayName + "\n" + displayTask, nodeStyles.GetStyle(nodeType));
            GUI.backgroundColor = nodeColors.callNumberColor;
            GUI.Box(callNumberRect, callNumber.ToString(), nodeStyles.callNumberStyle);
            foreach (Decorator decorator in decorators){
                decorator.Draw();
            }
            GUI.backgroundColor = currentColor;
            GUI.color = nodeColors.defaultTint;
        }
        protected override void ProcessContextMenu()
        {
            if (!IsRootNode()){
                GenericMenu genericMenu = new GenericMenu();
                Dictionary<string, bool> boolKeys = blackboard.GetBoolKeys();
                if (boolKeys == null || boolKeys.Count == 0){
                    genericMenu.AddDisabledItem(new GUIContent("Add blackboard bool keys to use as decorators"));
                }
                else{            
                    foreach(string boolName in blackboard.GetBoolKeys().Keys){
                        if (!DecoratorKeyActive(boolName)){
                            genericMenu.AddItem(new GUIContent("Add Decorator/" + boolName), false, () => OnClickAddDecorator(boolName));
                        }
                        else{
                            genericMenu.AddDisabledItem(new GUIContent("Add Decorator/" + boolName));
                        }
                    }
                }
                genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
                genericMenu.ShowAsContext();
            }
        }
        public override bool ProcessEvents(Event e)
        {
            bool guiChanged = false;
            bool decoratorSelected = false;
            if (decorators != null){
                foreach(Decorator decorator in decorators){
                    bool guiChangedFromDecorator = decorator.ProcessEvents(e);
                    if (!guiChanged && guiChangedFromDecorator){
                        guiChanged = true;
                    }
                    if (!decoratorSelected && decorator.isSelected){
                        decoratorSelected = true;
                    }
                }
            }
            if (!decoratorSelected){
                bool guiChangedFromNode =  ProcessSubNodeEvents(e);
                if (guiChangedFromNode){
                    guiChanged = true;
                }
            }
            else{
                SetSelected(false);
            }
            return guiChanged;
        }


        public virtual bool ProcessSubNodeEvents(Event e){
            bool guiChanged = false;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            guiChanged = true;
                            SetSelected(true);
                            UpdatePanelDetails(this);
                        }
                        else
                        {
                            guiChanged = true;
                            SetSelected(false);
                        }
                    }

                    if (e.button == 1 && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        DragWithChildren(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }
            return guiChanged;
        }

        protected void OnClickRemoveNode()
        {
            if (OnRemoveNode != null)
            {
                OnRemoveNode(this);
            }
        }

        protected void OnClickRemoveDecorator(Decorator decorator){
            int idx = decorators.FindIndex(a => a==decorator);
            if (idx != -1){

                decorators.Remove(decorator);

                // Resize node 
                rect.height -= subNodeSize[1];
                subNodeRect.y -= subNodeSize[1];
                callNumberRect.y -= subNodeSize[1];

                // Move all decorators below the removed one up
                Vector2 moveVec = new Vector2(0, -subNodeSize[1]);
                for (int i = idx; i < decorators.Count; i++){
                    decorators[i].Drag(moveVec);
                }
                GUI.changed = true;
            }
            else{
                throw new Exception("Decorator not found in decorators list.");
            }

        }

        protected void OnClickAddDecorator(string taskDisplayName){
            Decorator decorator = new Decorator( 
                                blackboard:ref blackboard,
                                displayTask:taskDisplayName,
                                displayName:"",
                                rect:new Rect(
                                rect.x + initDecoratorPos[0], 
                                rect.y + initDecoratorPos[1]+(subNodeSize[1]*decorators.Count+1),
                                subNodeSize[0],
                                subNodeSize[1] 
                                ),
                                parentNode:this.parentNode,
                                nodeStyles:nodeStyles, 
                                nodeColors:nodeColors,
                                UpdatePanelDetails:UpdatePanelDetails,
                                OnRemoveDecorator:OnClickRemoveDecorator,
                                childNode:this
                                );
            decorator.SetCallNumber(callNumber);
            callNumber++;
            this.parentNode = decorator;
            decorators.Add(decorator);
                    
            rect.height += subNodeSize[1];
            subNodeRect.y += subNodeSize[1];
            callNumberRect.y += subNodeSize[1];
            GUI.changed = true;
        }
        public List<Connection> GetChildConnections(){return childConnections;}
        public Connection GetParentConnection(){return parentConnection;}
        public void AddChildConnection(Connection connection){
            this.childConnections.Add(connection);
            this.childNodes.Add(connection.GetChildNode());
        }
        public void RemoveChildConnection(Connection connection){
            this.childConnections.Remove(connection);
            this.childNodes.Remove(connection.GetChildNode());
        }
        public void RemoveParentConnection(){
            this.parentConnection.GetParentNode().RemoveChildConnection(this.parentConnection);
            this.parentConnection = null;
        }

        public void RefreshChildOrder(){
            /**
            * Orders childConnections by x position
            */
            if (childConnections != null){
                childConnections.Sort((x,y) => x.GetChildNode().GetXPos().CompareTo(y.GetChildNode().GetXPos()));
            }
            childNodes = new List<Node>();
            foreach(Connection connection in childConnections){
                childNodes.Add(connection.GetChildNode());
            }

        }

        public float GetXPos(){
            return rect.x;
        }
        public void SetParentConnection(Connection connection){
            this.parentConnection = connection;       
        }
        public void RefreshDecoratorTasks(string oldKeyName, string newKeyName){
            if (decorators != null){
                foreach(Decorator decorator in decorators){
                    if (decorator.displayTask == oldKeyName){
                        decorator.displayTask = newKeyName;
                    }
                }
            }
        }

        public override void SetPosition(Vector2 pos){
            Drag(pos - rect.position);

        }

        private bool DecoratorKeyActive(string boolName){
            if (decorators != null){
                for (int i=0; i < decorators.Count; i++){
                    if (decorators[i].displayTask == boolName){
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void DrawDetails(){
            GUILayout.Label("Task: " + displayTask);
            GUILayout.Label("Name");
            displayName = GUILayout.TextField(displayName, 50);
        }
    }

}
