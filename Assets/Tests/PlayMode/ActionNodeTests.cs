using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;
public class ActionNodeTests
{

    TestMock testMock = new TestMock();

    public class ActionNodeStateTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class ActionNodeStateTest
         * Tests if CurrentState is correctly set to 
         * NodeState.Idle, NodeState.Running, NodeState.Failed, NodeState.Succeeded
         */

        ActionNode actionNode;
        NodeState expectedInitialState = NodeState.Idle;
        NodeState expectedTransientState = NodeState.Running;
        NodeState expectedOutcomeState;
        TestMock testMock = new TestMock();

        bool testFinished;

        public bool IsTestFinished{
            get {return testFinished;}
        }

        void Start(){
            testFinished = false;
            StartCoroutine(RunTests());
        }

        IEnumerator RunTests(){

            // Succeed test
            expectedOutcomeState = NodeState.Succeeded;
            actionNode = testMock.GetActionNodeMock(fail:false);
            actionNode.LoadTask(this);
            Assert.IsTrue(actionNode.CurrentState == expectedInitialState);
            actionNode.Evaluate();
            yield return new WaitForSeconds(.1f);
            Assert.IsTrue(actionNode.CurrentState == expectedTransientState);
            yield return new WaitForSeconds(TestMock.ActionDuration());
            Assert.IsTrue(actionNode.CurrentState == expectedOutcomeState);
            actionNode.ResetState();
            Assert.IsTrue(actionNode.CurrentState == expectedInitialState);

            // Fail test
            expectedOutcomeState = NodeState.Failed;
            actionNode = testMock.GetActionNodeMock(fail:true);
            actionNode.LoadTask(this);
            Assert.IsTrue(actionNode.CurrentState == expectedInitialState);
            actionNode.Evaluate();
            yield return new WaitForSeconds(.1f);
            Assert.IsTrue(actionNode.CurrentState == expectedTransientState);
            yield return new WaitForSeconds(TestMock.ActionDuration());
            Assert.IsTrue(actionNode.CurrentState == expectedOutcomeState);
            actionNode.ResetState();
            Assert.IsTrue(actionNode.CurrentState == expectedInitialState);

            testFinished = true;

        }
    }

    [Test]
    public void ActionNodeTaskName_Test()
    {

        /*
         * Test if TaskName is set correctly
         */

        ActionNode actionNode;
        actionNode = testMock.GetActionNodeMock(fail:false);
        Assert.IsTrue(actionNode.TaskName == "ActionMockSucceed");
        
        actionNode = testMock.GetActionNodeMock(fail:true);
        Assert.IsTrue(actionNode.TaskName == "ActionMockFail");

    }

    [UnityTest]
    public IEnumerator ActionNodeState_Test()
    {
        /*
         * Test if CurrentState is set correctly
         */

        yield return new MonoBehaviourTest<ActionNodeStateTest>();
    }

}
