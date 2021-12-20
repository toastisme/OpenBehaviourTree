using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUINode : CallableNode
{
    protected List<GUIDecorator> decorators;
    protected GUIStyle decoratorStyle;
    protected GUIStyle selectedDecoratorStyle;
    protected GUIStyle subNodeStyle;
    protected Vector2 initDecoratorPos = new Vector2(0f,0f);
    protected Vector2 subNodePos;
    protected Vector2 subNodeSize = NodeProperties.SubNodeSize();
    public ConnectionPoint childPoint;
    public ConnectionPoint parentPoint;
    private List<Connection> childNodes;
    protected Connection parentNode;
    public Action<GUINode> OnRemoveNode;
    protected BehaviourTree bt;
    protected Rect subNodeRect;

    public GUINode(NodeType nodeType,
                   Vector2 position, 
                   Vector2 size, 
                   GUIStyle nodeStyle, 
                   GUIStyle selectedStyle, 
                   GUIStyle subNodeStyle,
                   GUIStyle childPointStyle, 
                   GUIStyle parentPointStyle, 
                   GUIStyle callNumberStyle, 
                   GUIStyle decoratorStyle, 
                   GUIStyle selectedDecoratorStyle, 
                   Action<NodeBase> UpdatePanelDetails,
                   Action<ConnectionPoint> OnClickchildPoint, 
                   Action<ConnectionPoint> OnClickparentPoint, 
                   Action<GUINode> OnClickRemoveNode,
                   BehaviourTree behaviourTree
                   )
    {
        this.nodeType = nodeType;
        this.task = NodeBase.GetDefaultStringFromNodeType(nodeType);
        SetNodeTypeFromTask(task);
        rect = new Rect(position.x, position.y, size.x, size.y);
        subNodeRect = new Rect(position.x, position.y+subNodeSize.y*.5f, subNodeSize.x, subNodeSize.y);
        this.subNodeStyle = subNodeStyle;
        initDecoratorPos = new Vector2(0, rect.height*.21f);
        callNumberRect = new Rect(subNodeRect.position.x, subNodeRect.position.y, size.x/6, size.x/6);
        style = nodeStyle;
        this.callNumberStyle = callNumberStyle;
        this.decoratorStyle = decoratorStyle; 
        this.selectedDecoratorStyle = selectedDecoratorStyle; 
        if (nodeType == NodeType.Action){
            childPoint = null;
        }
        else{
            childPoint = new ConnectionPoint(this, ConnectionPointType.In, childPointStyle, OnClickchildPoint);
        }
        if (IsRootNode()){
            parentPoint = null;
        }
        else{
            parentPoint = new ConnectionPoint(this, ConnectionPointType.Out, parentPointStyle, OnClickparentPoint);
        }
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.bt = behaviourTree;
        childNodes = new List<Connection>();
        decorators = new List<GUIDecorator>();

    }

    public ConnectionPoint GetChildPoint(){return childPoint;}
    public ConnectionPoint GetParentPoint(){return parentPoint;}

    public override void Drag(Vector2 delta)
    {
        base.Drag(delta);
        subNodeRect.position += delta; 
        if (childNodes != null){
            foreach(Connection childNode in childNodes){
                childNode.GetChildNode().Drag(delta);
            }
        }
        if (decorators != null){
            foreach(GUIDecorator decorator in decorators){
                if (!decorator.IsSelected()){
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
        GUI.Box(rect, "", style);
        GUI.Box(subNodeRect, "\n" + name + "\n" + task, subNodeStyle);
        GUI.Box(callNumberRect, callNumber.ToString(), callNumberStyle);
        foreach (GUIDecorator decorator in decorators){
            decorator.Draw();
        }
    }

    public List<GUIDecorator> GetDecorators(){
        return decorators;
    }

    protected override void ProcessContextMenu()
    {
        if (!IsRootNode()){
            GenericMenu genericMenu = new GenericMenu();
            Dictionary<string, bool> boolKeys = bt.blackboard.GetBoolKeys();
            if (boolKeys == null || boolKeys.Count == 0){
                genericMenu.AddDisabledItem(new GUIContent("Add blackboard bool keys to use as decorators"));
            }
            else{            
                foreach(string boolName in bt.blackboard.GetBoolKeys().Keys){
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
            foreach(GUIDecorator decorator in decorators){
                bool guiChangedFromDecorator = decorator.ProcessEvents(e);
                if (!guiChanged && guiChangedFromDecorator){
                    guiChanged = true;
                }
                if (!decoratorSelected && decorator.IsSelected()){
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

    protected void OnClickRemoveDecorator(GUIDecorator decorator){
        decorators.Remove(decorator);
    }

    protected void OnClickAddDecorator(string conditionName){
        decorators.Add(new GUIDecorator(conditionName,
                              new Vector2(
                              rect.x + initDecoratorPos[0], rect.y + initDecoratorPos[1]+(subNodeSize[1]*decorators.Count+1)), 
                              subNodeSize[0],
                              subNodeSize[1], 
                              this,
                              decoratorStyle, 
                              selectedDecoratorStyle, 
                              callNumberStyle,
                              UpdatePanelDetails,
                              OnClickRemoveDecorator));
        rect.height += subNodeSize[1];
        subNodeRect.y += subNodeSize[1];
        callNumberRect.y += subNodeSize[1];
        decorators[decorators.Count -1].SetCallNumber(callNumber);
        callNumber++;
        GUI.changed = true;
    }

    public List<Connection> GetChildNodes(){return childNodes;}
    public Connection GetParentNode(){return parentNode;}
    public void SetChildNodes(List<Connection> childNodes){this.childNodes = childNodes;}
    public void AddChildNode(Connection connection){
        this.childNodes.Add(connection);
    }
    public void RemoveChildNode(Connection connection){
        childNodes.Remove(connection);
    }

    public void RefreshChildOrder(){
        /**
         * Orders childNodes by x position
         */
        if (childNodes != null){
            childNodes.Sort((x,y) => x.GetChildNode().GetXPos().CompareTo(y.GetChildNode().GetXPos()));
        }

    }

    public float GetXPos(){
        return rect.x;
    }
    public void RemoveParentNode(){
        this.parentNode = null;
    }
    public void SetParentNode(Connection connection){
        this.parentNode = connection;       
    }

    public bool IsRootNode(){
        return (nodeType == NodeType.Root);
    }
    public void RefreshDecoratorTasks(string oldKeyName, string newKeyName){
        if (decorators != null){
            foreach(GUIDecorator decorator in decorators){
                if (decorator.GetTask() == oldKeyName){
                    decorator.SetTask(newKeyName);
                }
            }
        }
    }



}