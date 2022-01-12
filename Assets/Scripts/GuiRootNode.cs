
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiRootNode : GuiPrioritySelector
{
    public GuiRootNode(
        PrioritySelector node,
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
        ApplyDerivedSettings();
        ApplyNodeTypeSettings(OnClickChildPoint, OnClickParentPoint);
    }
    protected override void ApplyDerivedSettings()
    {
        taskRectColor = NodeProperties.PrioritySelectorColor();
        defaultStyle = NodeProperties.PrioritySelectorStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = NodeProperties.DefaultColor();
    }

    protected override void ApplyNodeTypeSettings(
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){

        // Default GuiNode has a child and parent point
        ChildPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.In, 
                                            NodeProperties.ChildPointStyle(), 
                                            OnClickChildPoint);
        ParentPoint = null;

    }
}
}
