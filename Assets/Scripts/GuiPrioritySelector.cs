using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiPrioritySelector : CompositeGuiNode
{
    /**
    * \class GuiPrioritySelector
    * Displays an PrioritySelector in the BehaviourTree class using the BehaviourTreeEditor.
    */

    PrioritySelector prioritySelector; // The PrioritySelector being displayed
    public GuiPrioritySelector(
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
        this.prioritySelector = (PrioritySelector)node;
        ApplyDerivedSettings();
    }
    protected override void ApplyDerivedSettings()
    {
        taskRectColor = BehaviourTreeProperties.PrioritySelectorColor();
        defaultStyle = BehaviourTreeProperties.GUINodeStyle();
        selectedStyle = BehaviourTreeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        defaultTaskStyle = BehaviourTreeProperties.TaskNodeStyle();
        selectedTaskStyle = BehaviourTreeProperties.SelectedTaskNodeStyle();
        activeTaskStyle = defaultTaskStyle;
        color = BehaviourTreeProperties.DefaultColor();
        iconAndText= BehaviourTreeProperties.PrioritySelectorContent();
    }
}
}
