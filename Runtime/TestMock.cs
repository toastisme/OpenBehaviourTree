using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Behaviour{
public class TestMock 
{

    /**
     * \class Behaviour.TestMock
     * Holds mock objects for tests
     */

    string testBlackboardPath = "Assets/Tests/TestAssets/TestBlackboard.asset";
    string testBehaviourTreePath = "Assets/Tests/TestAssets/TestTree.asset";

    public static float ActionDuration(){
        return .5f;
    }

    public static float TimerDuration(){
        return .5f;
    }

    public BehaviourTreeBlackboard GetTestBlackboard(){
        BehaviourTreeBlackboard bTemplate = (BehaviourTreeBlackboard)AssetDatabase.LoadAssetAtPath(
            testBlackboardPath,
            typeof(BehaviourTreeBlackboard)
        );
        return UnityEngine.Object.Instantiate(bTemplate);
    }

    public BehaviourTree GetTestBehaviourTree(){
        BehaviourTree bTemplate = (BehaviourTree)AssetDatabase.LoadAssetAtPath(
            testBehaviourTreePath,
            typeof(BehaviourTree)
        );
        return UnityEngine.Object.Instantiate(bTemplate);
    }

    public ActionNode GetActionNodeMock(bool fail=false){
        BehaviourTreeBlackboard blackboard = GetTestBlackboard();
        if (fail){
            return new ActionNode(
                taskName:"ActionMockFail",
                blackboard:ref blackboard 
            );
        }
        return new ActionNode(
            taskName:"ActionMockSucceed",
            blackboard:ref blackboard 
        );
    } 

    public ActionNode GetActionNodeMock(
        ref BehaviourTreeBlackboard blackboard,
        bool fail=false
        ){
        if (fail){
            return new ActionNode(
                taskName:"ActionMockFail",
                blackboard:ref blackboard 
            );
        }
        return new ActionNode(
            taskName:"ActionMockSucceed",
            blackboard:ref blackboard 
        );
    } 

    public ActionWaitNode GetActionWaitNodeMock(){
        BehaviourTreeBlackboard blackboard = GetTestBlackboard();
            return new ActionWaitNode(
                taskName:"Wait",
                blackboard:ref blackboard,
                timerValue:BehaviourTreeProperties.DefaultTimerVal(),
                randomDeviation:BehaviourTreeProperties.DefaultRandomDeviationVal()
            );
    }

    public BoolDecorator GetDecoratorMock(){
        BehaviourTreeBlackboard blackboard = GetTestBlackboard();
        return new BoolDecorator(
            taskName:"testBool",
            blackboard:ref blackboard,
            invertCondition:false,
            childNode:GetActionNodeMock(
                blackboard:ref blackboard,
                fail:false
            )
        );
    }

}
}
