
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiActionNode : CompositeGuiNode
{
    ActionNode actionNode;
    public GuiActionNode(
        ActionNode node,
        string displayTask,
        string displayName,
        Vector2 pos,
        Connection parentConnection,
        Action<GuiNode> UpdatePanelDetails,
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
        OnRemoveNode:OnRemoveNode,
        OnClickChildPoint:OnClickChildPoint,
        OnClickParentPoint:OnClickParentPoint,
        blackboard:ref blackboard
    )
    {
        this.actionNode = node;
        ApplyDerivedSettings();
        ApplyNodeTypeSettings(OnClickChildPoint,OnClickParentPoint);
    }
    protected override void ApplyDerivedSettings()
    {
        taskRectColor = NodeProperties.ActionColor();
        defaultStyle = NodeProperties.ActionStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = NodeProperties.DefaultColor();
    }

    protected override void ApplyNodeTypeSettings(
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){

        // Default GuiNode has a child and parent point
        ChildPoint = null;
        ParentPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.Out, 
                                            NodeProperties.ParentPointStyle(), 
                                            OnClickParentPoint);

    }
}
}