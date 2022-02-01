using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class GuiActionWaitNode : GuiActionNode
{
    ActionWaitNode actionWaitNode;
    string displayTask;
    public GuiActionWaitNode(
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
    ){
        actionWaitNode = (ActionWaitNode)node;
    }

    public override void DrawDetails()
    {
        base.DrawDetails();
        float waitTime = actionWaitNode.WaitTime;
        bool success = float.TryParse(GUILayout.TextField(actionWaitNode.WaitTime.ToString(), 50), out waitTime);
        if (success){
            actionWaitNode.WaitTime = waitTime;
        }
        
    }
    protected override void DrawSelf()
    {
        GUI.backgroundColor = color;
        GUI.Box(rect, "", activeStyle);
        GUI.backgroundColor = taskRectColor; 
        GUI.Box(taskRect, 
        "\n" + DisplayName + "\n" + DisplayTask + " (" + actionWaitNode.WaitTime + " sec)", 
        activeStyle);
    }

}
}