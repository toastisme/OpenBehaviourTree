using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Behaviour{
public class CallableGuiNode : GuiNode
{
    public CallNumberNode callNumber; // Displays when the node will be called in the tree
    protected CallableGuiNode(
        Node node,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        ref BehaviourTreeBlackboard blackboard
    ) :base(
        node:node,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        blackboard: ref blackboard
    )
    {
        callNumber = new CallNumberNode();
        callNumber.SetPosition(rect.position);
    }

    public override void Drag(Vector2 delta){
        base.Drag(delta);
        callNumber.Drag(delta);
    }

    public override void Draw(){}
    public void SetCallNumber(int num){
        callNumber.CallNumber = num;
    }
}
}