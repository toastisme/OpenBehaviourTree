using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiPrioritySelector : CompositeGuiNode
{
    PrioritySelector prioritySelector;
    public GuiPrioritySelector(
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
        blackboard:blackboard
    )
    {
        this.prioritySelector = prioritySelector;
        ApplyDerivedSettings();
    }
    protected override void ApplyDerivedSettings()
    {
        color = NodeProperties.PrioritySelectorColor();
        defaultStyle = NodeProperties.PrioritySelectorStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        taskRectColor = NodeProperties.DefaultColor();
    }
}
}
