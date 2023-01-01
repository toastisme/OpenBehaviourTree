using UnityEngine;
using System.Collections.Generic;

namespace OpenBehaviourTree{
public class BehaviourTreeLoader
{

    /**
    * \class OpenBehaviourTree.BehaviourTreeLoader
    * Static methods to load serialized versions of Nodes and GuiNodes.
    */

    public static Node NodeFactory(
        SerializableNode serializedNode,
        ref BehaviourTreeBlackboard blackboard
    ){

        NodeType nodeType = (NodeType)serializedNode.type;
        string taskName = serializedNode.taskName;
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
            case NodeType.BoolDecorator:
                return new BoolDecorator(
                        taskName:taskName,
                        blackboard:ref blackboard,
                        invertCondition:serializedNode.invertCondition
                );
            case NodeType.Action:
                return new ActionNode(
                        taskName:taskName,
                        blackboard:ref blackboard
                );
            case NodeType.ActionWait:
                return new ActionWaitNode(
                        taskName:taskName,
                        blackboard:ref blackboard,
                        timerValue:serializedNode.value,
                        randomDeviation:serializedNode.randomDeviation,
                        valueKey:serializedNode.valueKey,
                        randomDeviationKey:serializedNode.randomDeviationKey
                );
            case NodeType.Timer:
                if (taskName == "Timeout"){
                    return new TimeoutNode(
                        blackboard: ref blackboard,
                        timerValue:serializedNode.value,
                        randomDeviation:serializedNode.randomDeviation,
                        valueKey:serializedNode.valueKey,
                        randomDeviationKey:serializedNode.randomDeviationKey
                    );
                }
                else if (taskName == "Cooldown"){
                    return new CooldownNode(
                        blackboard: ref blackboard,
                        timerValue:serializedNode.value,
                        randomDeviation:serializedNode.randomDeviation,
                        valueKey:serializedNode.valueKey,
                        randomDeviationKey:serializedNode.randomDeviationKey,
                        activateOnSuccess:serializedNode.activateOnSuccess,
                        activateOnFailure:serializedNode.activateOnFailure
                    );
                }
                break;
            default:
                throw new System.Exception("Unknown node type");
        }
        throw new System.Exception("Unknown node type");

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
                    TreeModified:null,
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
                    TreeModified:null,
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
                    TreeModified:null,
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
                    TreeModified:null,
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
                    TreeModified:null,
                    NodeUpdated:null,
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
                    TreeModified:null,
                    OnRemoveNode:null,
                    OnClickChildPoint:null,
                    OnClickParentPoint:null,
                    blackboard:ref blackboard
                );
            case NodeType.ActionWait:
                return new GuiActionWaitNode(
                    node:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    parentConnection:null,
                    UpdatePanelDetails:null,
                    TreeModified:null,
                    OnRemoveNode:null,
                    OnClickChildPoint:null,
                    OnClickParentPoint:null,
                    blackboard:ref blackboard
                );
            case NodeType.BoolDecorator:
                return new GuiBoolDecorator(
                    decorator:node,
                    displayTask:displayTask,
                    displayName:displayName,
                    pos:pos,
                    UpdatePanelDetails:null,
                    TreeModified:null,
                    NodeUpdated:null,
                    OnRemoveDecorator:null,
                    blackboard:ref blackboard,
                    parentGuiNode:null
                );
            case NodeType.Timer:
                if (displayTask == "Timeout"){
                    return new GuiTimeoutNode(
                    timerNode:(TimerNode)node,
                    displayName:displayName,
                    pos:pos,
                    UpdatePanelDetails:null,
                    TreeModified:null,
                    NodeUpdated:null,
                    OnRemoveDecorator:null,
                    blackboard:ref blackboard,
                    parentGuiNode:null
                    );
                }
                else if (displayTask == "Cooldown"){
                    return new GuiCooldownNode(
                    timerNode:(TimerNode)node,
                    displayName:displayName,
                    pos:pos,
                    UpdatePanelDetails:null,
                    TreeModified:null,
                    NodeUpdated:null,
                    OnRemoveDecorator:null,
                    blackboard:ref blackboard,
                    parentGuiNode:null
                    );
                }
                break;
            default:
                throw new System.Exception("Unknown node type");
        }
        throw new System.Exception("Unknown node type");
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
            serializedNode:serializedNode,
            blackboard:ref blackboard
        );

        // Depth first  
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