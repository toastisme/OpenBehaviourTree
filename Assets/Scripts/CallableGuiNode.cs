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
        Action TreeModified,
        ref BehaviourTreeBlackboard blackboard
    ) :base(
        node:node,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        TreeModified:TreeModified,
        blackboard: ref blackboard
    )
    {
        callNumber = new CallNumberNode();
        SetDefaultCallNumberPos();
    }

    public override void Drag(Vector2 delta){
        base.Drag(delta);
        callNumber.Drag(delta);
    }

    public override void UpdateOrigin(Vector2 origin){
        base.UpdateOrigin(origin);
        callNumber.UpdateOrigin(origin);
    }

    public override void Draw(){}
    public void SetCallNumber(int num){
        callNumber.CallNumber = num;
    }

    public virtual void SetDefaultCallNumberPos(){
        Vector2 callNumberPos = rect.position;
        callNumberPos += new Vector2(rect.size.x, 0);
        callNumberPos -= new Vector2(callNumber.GetRect().width, 0);
        callNumber.SetPosition(callNumberPos);
    }


}
}