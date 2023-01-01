
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OpenBehaviourTree{
public class GuiRootNode : GuiPrioritySelector
{

    /**
     * \class OpenBehaviourTree.GuiRootNode
     * Derived GuiPrioritySelector class with no ParentPoint 
     * (thereby stopping it being able to be a child to another node).
     */

    public GuiRootNode(
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
        ApplyDerivedSettings();
        ApplyNodeTypeSettings(OnClickChildPoint, OnClickParentPoint);
    }
    protected override void ApplyDerivedSettings()
    {
        taskRectColor = BehaviourTreeProperties.PrioritySelectorColor();
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = BehaviourTreeProperties.DefaultColor();
        defaultTaskStyle = BehaviourTreeProperties.TaskNodeStyle();
        selectedTaskStyle = BehaviourTreeProperties.SelectedTaskNodeStyle();
        activeTaskStyle = defaultTaskStyle;
        iconAndText = BehaviourTreeProperties.PrioritySelectorContent();
    }

    protected override void ApplyNodeTypeSettings(
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){

        ChildPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.In, 
                                            BehaviourTreeProperties.ChildPointStyle(), 
                                            OnClickChildPoint);
                                            
        // Root nodes cannot have parents
        ParentPoint = null;

    }

    protected override bool IsRootNode(){return true;}
}
}
