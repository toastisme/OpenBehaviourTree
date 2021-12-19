using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUINode : CallableNode
{
    protected List<GUIDecorator> decorators;
    protected GUIStyle decoratorStyle;
    protected GUIStyle selectedDecoratorStyle;
    protected Vector2 initDecoratorPos = new Vector2(0f,0f);
    protected float decoratorHeight = 50f;
    public ConnectionPoint ChildPoint;
    public ConnectionPoint ParentPoint;
    private List<Connection> childNodes;
    protected Connection parentNode;
    public Action<GUINode> OnRemoveNode;
    protected BehaviourTree bt;

    public GUINode(string task,
                   Vector2 position, 
                   Vector2 size, 
                   GUIStyle nodeStyle, 
                   GUIStyle selectedStyle, 
                   GUIStyle ChildPointStyle, 
                   GUIStyle ParentPointStyle, 
                   GUIStyle callNumberStyle, 
                   GUIStyle decoratorStyle, 
                   GUIStyle selectedDecoratorStyle, 
                   Action<NodeBase> UpdatePanelDetails,
                   Action<ConnectionPoint> OnClickChildPoint, 
                   Action<ConnectionPoint> OnClickParentPoint, 
                   Action<GUINode> OnClickRemoveNode,
                   BehaviourTree behaviourTree
                   )
    {
        this.task = task;
        SetNodeTypeFromTask(task);
        rect = new Rect(position.x, position.y, size.x, size.y);
        initDecoratorPos = new Vector2(0, rect.height*.5f);
        callNumberRect = new Rect(position.x, position.y, size.x/6, size.x/6);
        style = nodeStyle;
        this.callNumberStyle = callNumberStyle;
        this.decoratorStyle = decoratorStyle; 
        this.selectedDecoratorStyle = selectedDecoratorStyle; 
        ChildPoint = new ConnectionPoint(this, ConnectionPointType.In, ChildPointStyle, OnClickChildPoint);
        if (IsRootNode()){
            ParentPoint = null;
        }
        else{
            ParentPoint = new ConnectionPoint(this, ConnectionPointType.Out, ParentPointStyle, OnClickParentPoint);
        }
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;
        this.UpdatePanelDetails = UpdatePanelDetails;
        this.bt = behaviourTree;
        childNodes = new List<Connection>();
        decorators = new List<GUIDecorator>();

    }

    public ConnectionPoint GetChildPoint(){return ChildPoint;}
    public ConnectionPoint GetParentPoint(){return ParentPoint;}

    public override void Drag(Vector2 delta)
    {
        base.Drag(delta);
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
        if (ChildPoint != null){
            ChildPoint.Draw();
        }
        if (ParentPoint != null){
            ParentPoint.Draw();
        }
        GUI.Box(rect, "\n" + name + "\n" + task, style);
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
                              rect.x + initDecoratorPos[0], rect.y + initDecoratorPos[1]+(decoratorHeight*decorators.Count+1)), 
                              rect.width,
                              decoratorHeight, 
                              this,
                              decoratorStyle, 
                              selectedDecoratorStyle, 
                              callNumberStyle,
                              UpdatePanelDetails,
                              OnClickRemoveDecorator));
        rect.height += decoratorHeight;
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