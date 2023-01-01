using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace OpenBehaviourTree{
public class GuiTimeoutNode : GuiTimerNode
{
    /**
    * \class OpenBehaviourTree.GuiTimeoutNode
    * Base class for displaying TimeoutNodes in the BehaviourTree class
    */

    public GuiTimeoutNode(
        Node timerNode,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
        Action NodeUpdated,
        Action<GuiDecorator> OnRemoveDecorator,
        ref BehaviourTreeBlackboard blackboard,
        GuiNode parentGuiNode
    ) :base(
        timerNode:timerNode,
        displayTask:"Timeout",
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        NodeUpdated:NodeUpdated,
        OnRemoveDecorator:OnRemoveDecorator,
        blackboard: ref blackboard,
        parentGuiNode: parentGuiNode
    )
    {
        ApplyDerivedSettings();
    }

    protected override void ApplyDerivedSettings(){
        base.ApplyDerivedSettings();
        iconAndText = BehaviourTreeProperties.TimeoutContent();
    }
}
}
