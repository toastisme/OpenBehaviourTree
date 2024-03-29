using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace OpenBehaviourTree{
public class GuiCooldownNode : GuiTimerNode
{
    /**
    * \class OpenBehaviourTree.GuiCooldownNode
    * Displays a CooldownNode in the BehaviourTree class, using the BehaviourTreeEditor.
    */

    CooldownNode cooldownNode;
    public GuiCooldownNode(
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
        displayTask:"Cooldown",
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
        cooldownNode = (CooldownNode)timerNode;
        ApplyDerivedSettings();
    }

    protected override void ApplyDerivedSettings(){
        base.ApplyDerivedSettings();
        iconAndText = BehaviourTreeProperties.CooldownContent();
    }

    public override void DrawDetails(){
        /**
         * What details are displayed in the details panel of the BehaviourTreeEditor
         */
        base.DrawDetails();
        EditorGUI.BeginChangeCheck();
        cooldownNode.activateOnSuccess = EditorGUILayout.Toggle("Activate on success", 
                                                             cooldownNode.activateOnSuccess);
        cooldownNode.activateOnFailure = EditorGUILayout.Toggle("Activate on failure", 
                                                             cooldownNode.activateOnFailure);
        if (EditorGUI.EndChangeCheck()){
            TreeModified();
            NodeUpdated();
        }


    }

}
}