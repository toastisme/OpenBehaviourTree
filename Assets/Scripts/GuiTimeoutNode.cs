using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class GuiTimeoutNode : GuiTimerNode
{
    public GuiTimeoutNode(
        Node timerNode,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action TreeModified,
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
        OnRemoveDecorator:OnRemoveDecorator,
        blackboard: ref blackboard,
        parentGuiNode: parentGuiNode
    )
    {
    }

    protected override void ApplyDerivedSettings(){
        base.ApplyDerivedSettings();
        iconAndText = BehaviourTreeProperties.TimeoutContent();
    }
}
}
