
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiRootNode : GuiPrioritySelector
{
    public GuiPrioritySelector(
        PrioritySelector node,
        string displayTask,
        string displayName,
        Rect rect,
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
        rect:rect,
        parentConnection:parentConnection,
        UpdatePanelDetails:UpdatePanelDetails,
        OnRemoveNode:OnRemoveNode,
        OnClickChildPoint:OnClickChildPoint,
        OnClickParentPoint:OnClickParentPoint,
        blackboard:blackboard
    )
    {
        ApplyDerivedSettings();
        ApplyNodeTypeSettings(OnClickChildPoint, OnClickParentPoint);
    }
    protected override void ApplyDerivedSettings()
    {
        color = NodeProperties.PrioritySelectorColor();
        defaultStyle = NodeProperties.PrioritySelectorStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        taskRectColor = NodeProperties.DefaultColor();
    }

    protected override void ApplyNodeTypeSettings(
        Action<ConnectionPoint> OnClickChildPoint,
        Action<ConnectionPoint> OnClickParentPoint
    ){

        // Default GuiNode has a child and parent point
        ChildPoint = new ConnectionPoint(this, 
                                            ConnectionPointType.In, 
                                            nodeStyles.childPointStyle, 
                                            OnClickChildPoint);
        ParentPoint = null;

    }
}
}
