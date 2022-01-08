using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public class CallableGuiNode : GuiNode
{
    CallNumberNode callNumber; // Displays when the node will be called in the tree
    protected CallableGuiNode(
        Node node,
        string displayTask,
        string displayName,
        Rect rect,
        Action<GuiNode> UpdatePanelDetails,
        ref BehaviourTreeBlackboard blackboard
    ) :base(
        node:node,
        displayTask:displayTask,
        displayName:displayName,
        rect:rect,
        UpdatePanelDetails:UpdatePanelDetails,
        blackboard: ref blackboard
    )
    {
        callNumber = new CallNumberNode();
        callNumber.NodeRect.position = rect.position;
    }

    public override void Drag(Vector2 delta){
        base.Drag();
        callNumber.Drag(delta);
    }

    public override void Draw(){}
    public void SetCallNumber(int num){
        callNumber.CallNumber = num;
    }
}
}