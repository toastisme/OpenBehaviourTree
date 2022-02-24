using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiSequenceSelector : CompositeGuiNode
{
    /**
    * \class GuiSequenceSelector
    * Displays an SequenceSelector in the BehaviourTree class using the BehaviourTreeEditor.
    */

    SequenceSelector sequenceSelector;
    public GuiSequenceSelector(
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
        this.sequenceSelector = (SequenceSelector)node;
        ApplyDerivedSettings();
    }
    protected override void ApplyDerivedSettings()
    {
        taskRectColor = BehaviourTreeProperties.SequenceSelectorColor();
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        defaultTaskStyle = BehaviourTreeProperties.TaskNodeStyle();
        selectedTaskStyle = BehaviourTreeProperties.SelectedTaskNodeStyle();
        activeTaskStyle = defaultTaskStyle;
        color = BehaviourTreeProperties.DefaultColor();
        iconAndText = BehaviourTreeProperties.SequenceSelectorContent();
    }
}
}
