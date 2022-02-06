using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiSequenceSelector : CompositeGuiNode
{
    SequenceSelector sequenceSelector;
    public GuiSequenceSelector(
        Node node,
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
        this.sequenceSelector = (SequenceSelector)node;
        ApplyDerivedSettings();
    }
    protected override void ApplyDerivedSettings()
    {
        taskRectColor = NodeProperties.SequenceSelectorColor();
        defaultStyle = NodeProperties.GUINodeStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        defaultTaskStyle = NodeProperties.TaskNodeStyle();
        selectedTaskStyle = NodeProperties.SelectedTaskNodeStyle();
        activeTaskStyle = defaultTaskStyle;
        color = NodeProperties.DefaultColor();
        iconAndText = NodeProperties.SequenceSelectorContent();
    }
}
}
