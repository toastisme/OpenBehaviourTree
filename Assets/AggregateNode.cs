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

        // Constructor
        public AggregateNode(
            NodeType nodeType,
            string displayTask,
            string displayName,
            Rect rect,
            Node parentNode,
            GUIStyle defaultStyle,
            GUIStyle selectedStyle,
            GUIStyle callNumberStyle,
            Color color,
            Color callNumberColor,
            Action<Node> UpdatePanelDetails,
            NodeStyles nodeStyles,
            NodeColors nodeColors,
            Action<ConnectionPoint> OnClickChildPoint,
            Action<ConnectionPoint> OnClickParentPoint,
            Action<AggregateNode> OnRemoveNode,
            BehaviourTreeBlackboard blackboard
        ) : base(
            nodeType:nodeType,
            displayTask:displayTask,
            displayName:displayName,
            rect:rect,
            parentNode:parentNode,
            defaultStyle:defaultStyle,
            selectedStyle:selectedStyle,
            callNumberStyle:callNumberStyle,
            color:color,
            callNumberColor:callNumberColor,
            UpdatePanelDetails:UpdatePanelDetails
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
                        genericMenu.AddItem(new GUIContent("Add Decorator/" + boolName), false, () => OnClickAddDecorator(boolName));
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
                bool guiChangedFromNode =  base.ProcessEvents(e);
                if (guiChangedFromNode){
                    guiChanged = true;
                }
            }
            else{
                SetSelected(false);
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
            decorators.Remove(decorator);
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
                                defaultStyle:nodeStyles.decoratorStyle, 
                                selectedStyle:nodeStyles.selectedDecoratorStyle, 
                                callNumberStyle:nodeStyles.callNumberStyle,
                                color:nodeColors.decoratorColor,
                                callNumberColor:nodeColors.callNumberColor,
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
    }

}
