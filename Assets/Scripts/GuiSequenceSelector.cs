using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiSequenceSelector : CompositeGuiNode
{
    SequenceSelector sequenceSelector;
    public GuiSequenceSelector(
        SequenceSelector node,
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
        this.sequenceSelector = sequenceSelector;
        ApplyDerivedSettings();
    }
    protected override void ApplyDerivedSettings()
    {
        color = NodeProperties.SequenceSelectorColor();
        defaultStyle = NodeProperties.SequenceSelectorStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        taskRectColor = NodeProperties.DefaultColor();
    }
}
}
