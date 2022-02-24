
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiActionNode : CompositeGuiNode
{
    /**
    * \class GuiActionNode
    * Displays an ActionNode in the BehaviourTree class using the BehaviourTreeEditor.
    * The ActionNode displayTask is the ActionNode.TaskName, and refers to a BehaviourTreeTask.
    * This contains a coroutine that is executed when the node is evaluated.
    */

    public ActionNode actionNode{get; private set;}
    public GuiActionNode(
        Node node,
        string displayTask,
        string displayName,
        Vector2 pos,
        Connection parentConnection,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action<CompositeGuiNode> OnRemoveNode,
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint,
        ref BehaviourTreeBlackboard blackboard
    ) : base(
        node:node,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        parentConnection:parentConnection,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        OnRemoveNode:OnRemoveNode,
        OnClickChildPoint:OnClickChildPoint,
        OnClickParentPoint:OnClickParentPoint,
        blackboard:ref blackboard
    )
    {
        this.actionNode = (ActionNode)node;
        ApplyDerivedSettings();
        ApplyNodeTypeSettings(OnClickChildPoint,OnClickParentPoint);
    }
    protected override void ApplyDerivedSettings()
    {
        taskRectColor = BehaviourTreeProperties.ActionColor();
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        defaultTaskStyle = BehaviourTreeProperties.TaskNodeStyle();
        selectedTaskStyle = BehaviourTreeProperties.SelectedTaskNodeStyle();
        activeTaskStyle = defaultTaskStyle;
        activeStyle = defaultStyle;
        color = BehaviourTreeProperties.DefaultColor();
        iconAndText = BehaviourTreeProperties.ActionContent();
    }

    protected override void ApplyNodeTypeSettings(
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){

        ChildPoint = null; // ActionNodes are leaf nodes and so have no children
        ParentPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.Out, 
                                            BehaviourTreeProperties.ParentPointStyle(), 
                                            OnClickParentPoint);

    }
}
}
