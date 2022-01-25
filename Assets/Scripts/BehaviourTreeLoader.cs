using UnityEngine;
using System.Collections.Generic;

namespace Behaviour{
public class BehaviourTreeLoader
{
    public static Node NodeFactory(NodeType nodeType, 
                            string taskName,
                            ref BehaviourTreeBlackboard blackboard,
                            bool invertCondition
                            ){
        switch(nodeType){
            case NodeType.Root:
                return new PrioritySelector(
                            taskName:"Root"
                            );
            case NodeType.SequenceSelector:
                return new SequenceSelector(
                        taskName:taskName
                );
            case NodeType.PrioritySelector:
                return new PrioritySelector(
                        taskName:taskName
                );
            case NodeType.ProbabilitySelector:
                return new ProbabilitySelector(
                        taskName:taskName,
                        blackboard:ref blackboard
                );
            case NodeType.ProbabilityWeight:
                return new ProbabilityWeight(
                        taskName:taskName
                );
            case NodeType.Decorator:
                return new Decorator(
                        taskName:taskName,
                        blackboard:ref blackboard,
                        invertCondition:invertCondition
                );
            case NodeType.Action:
                return new ActionNode(
                        taskName:taskName,
                        blackboard:ref blackboard
                );
            default:
                throw new System.Exception("Unknown node type");
        }

    }


    public static GuiNode GuiNodeFactory(
        Node node,
        string displayTask,
        string displayName,
        Vector2 pos,
        ref BehaviourTreeBlackboard blackboard
        ){
        switch (node.GetNodeType()){
            case NodeType.Root:
                return new GuiRootNode(
                    node:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    parentConnection:null,
                    UpdatePanelDetails:null,
                    OnRemoveNode:null,
                    OnClickChildPoint:null,
                    OnClickParentPoint:null,
                    blackboard:ref blackboard
                );
            case NodeType.SequenceSelector:
                return new GuiSequenceSelector(
                    node:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    parentConnection:null,
                    UpdatePanelDetails:null,
                    OnRemoveNode:null,
                    OnClickChildPoint:null,
                    OnClickParentPoint:null,
                    blackboard:ref blackboard
                );
            case NodeType.PrioritySelector:
                return new GuiPrioritySelector(
                    node:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    parentConnection:null,
                    UpdatePanelDetails:null,
                    OnRemoveNode:null,
                    OnClickChildPoint:null,
                    OnClickParentPoint:null,
                    blackboard:ref blackboard
                );
            case NodeType.ProbabilitySelector:
                return new GuiProbabilitySelector(
                    node:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    parentConnection:null,
                    UpdatePanelDetails:null,
                    OnRemoveNode:null,
                    OnClickChildPoint:null,
                    OnClickParentPoint:null,
                    blackboard:ref blackboard
                );
            case NodeType.ProbabilityWeight:
                return new GuiProbabilityWeight(
                    node:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    parentConnection:null,
                    UpdatePanelDetails:null,
                    blackboard:ref blackboard
                );
            case NodeType.Action:
                return new GuiActionNode(
                    node:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    parentConnection:null,
                    UpdatePanelDetails:null,
                    OnRemoveNode:null,
                    OnClickChildPoint:null,
                    OnClickParentPoint:null,
                    blackboard:ref blackboard
                );
            case NodeType.Decorator:
                return new GuiDecorator(
                    decorator:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    UpdatePanelDetails:null,
                    OnRemoveDecorator:null,
                    blackboard:ref blackboard,
                    parentGuiNode:null
                );
            default:
                throw new System.Exception("Unknown node type");
        }
    }
    public static Node ReadNodeFromSerializedNodes(
        ref int index,
        List<SerializableNode> serializedNodes,
        BehaviourTreeBlackboard blackboard
        ) 
        {

        /**
         *Recursive function to write all serializedNodes into node
         */

        var serializedNode = serializedNodes [index];

        var node = BehaviourTreeLoader.NodeFactory(
            nodeType:(NodeType)serializedNode.type,
            taskName:serializedNode.taskName,
            blackboard:ref blackboard,
            invertCondition: serializedNode.invertCondition
        );

        if (serializedNode.cooldown > 0){
            node.AddCooldown(timerVal:serializedNode.cooldown);
        }
        if (serializedNode.timeout > 0){
            node.AddTimeout(timerVal:serializedNode.timeout);
        }

        // The tree needs to be read in depth-first, since that's how we wrote it out.
        for (int i = 0; i != serializedNode.childCount; i++) {
            index++;
            node.ChildNodes.Add(
                ReadNodeFromSerializedNodes(
                    index:ref index,
                    serializedNodes:serializedNodes,
                    blackboard:blackboard
                )
            );
            node.ChildNodes[i].SetParentNode(node);
        }
        return node;
    }


}
}