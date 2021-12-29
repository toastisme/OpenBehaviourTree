
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;

namespace BehaviourBase{
    public class ActionNode : AggregateNode
    {    
        /**
        * \class ActionNode
        * Represents an action node in the BehaviourTree class.
        */

        BehaviourTreeTask btTask;
        Task task;

        public ActionNode(
            string displayTask,
            string displayName,
            Rect rect,
            AggregateNode parentNode,
            Action<AggregateNode> UpdatePanelDetails,
            NodeStyles nodeStyles,
            NodeColors nodeColors,
            Action<ConnectionPoint> OnClickChildPoint,
            Action<ConnectionPoint> OnClickParentPoint,
            Action<AggregateNode> OnRemoveNode,
            ref BehaviourTreeBlackboard blackboard
        ) :base(
            nodeType:NodeType.Action,
            displayTask:displayTask,
            displayName:displayName,
            rect:rect,
            parentNode:parentNode,
            UpdatePanelDetails:UpdatePanelDetails,
            nodeStyles:nodeStyles,
            nodeColors:nodeColors,
            OnClickChildPoint:OnClickChildPoint,
            OnClickParentPoint:OnClickParentPoint,
            OnRemoveNode:OnRemoveNode,
            blackboard:ref blackboard
        ){
            btTask = LoadTask(displayTask);
            SetupTask();
        }

        private BehaviourTreeTask LoadTask(string displayTask){
            Type type = TypeUtils.GetType(displayTask); // Full class name (include the namespaces)
            ConstructorInfo constructor = TypeUtils.ResolveEmptyConstructor(type);
            object[] EMPTY_PARAMETERS = new object[0]; // This can be made into a static variable
            
            // Invoke the constructor
            // Can also be cast to Shoot but I'd like to point out a possible usage
            return (BehaviourTreeTask)constructor.Invoke(EMPTY_PARAMETERS);
            
        }

        private void SetupTask(){
            if(task != null){
                if (task.Running){
                    task.Stop();
                }
            }
            task = new Task(btTask.ExecuteTask((x)=>nodeState=x), false);
        }

        public override NodeState Evaluate(){

            /**
            * If the node is idle with no action running, starts the action.
            * Returns the state of the node.
            */

            if (nodeState == NodeState.Idle && !task.Running){
                task.Start();
            }
            return nodeState;        
        }

        public override void ResetState(){

            /**
            * Stops the action if running.
            * Sets the NodeState to IDLE.
            */

            if (task.Running){
                task.Stop();
            }
            SetupTask();
            nodeState = NodeState.Idle;
        }

        public override Node GetRunningLeafNode(){

            /**
            * Returns this.name if action is running.
            * Else null.
            */

            if (nodeState == NodeState.Running){
                return this;
            }
            return null;
        }

    }

}