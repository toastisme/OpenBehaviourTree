using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;
public class ActionNodeTests
{

    TestMock testMock = new TestMock();

    public class ActionNodeLoadTaskTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class ActionNodeLoadTaskTest
         * Tests if ActionNode.LoadTask gives expected results
         */
        ActionNode actionNode;
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

            // Test succeed task loaded correctly
            actionNode = testMock.GetActionNodeMock(fail:false);
            actionNode.LoadTask(this);
            Assert.IsTrue(actionNode.btTask is ActionMockSucceed);

            // Test fail task loaded correctly
            actionNode = testMock.GetActionNodeMock(fail:true);
            actionNode.LoadTask(this);
            Assert.IsTrue(actionNode.btTask is ActionMockFail);
            testFinished = true;
            yield return null;

        }



    }

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


            // Cooldown test
            NodeState expectedCooldownState = NodeState.Failed;
            float cooldownDuration = 1f;
            NodeTimer cooldown = new NodeTimer(timerVal:cooldownDuration);
            actionNode.AddCooldown(cooldown);
            actionNode.nodeCooldown.LoadTask(this);

            actionNode.Evaluate();
            yield return new WaitForSeconds(.1f);
            Assert.IsTrue(actionNode.CurrentState == expectedTransientState);
            yield return new WaitForSeconds(TestMock.ActionDuration());
            Assert.IsTrue(actionNode.CurrentState == expectedOutcomeState);
            // Cooldown started on this evaluate call
            actionNode.Evaluate();
            Assert.IsTrue(actionNode.CooldownActive());
            actionNode.Evaluate();
            Assert.IsTrue(actionNode.CurrentState == expectedCooldownState);
            yield return new WaitForSeconds(.2f);
            // Cooldown still active
            actionNode.Evaluate();
            Assert.IsTrue(actionNode.CurrentState == expectedCooldownState);
            yield return new WaitForSeconds(cooldownDuration);
            // Cooldown exceeded, state is reset and task runs on the next evaluate
            actionNode.Evaluate();
            Assert.IsTrue(actionNode.CurrentState == expectedTransientState);
            actionNode.ResetState();
            actionNode.RemoveCooldown();

            // Timeout test
            NodeState expectedTimeoutState = NodeState.Failed;
            float timeoutDuration = TestMock.ActionDuration()*.5f;
            NodeTimer timeout = new NodeTimer(timerVal:timeoutDuration);
            actionNode.AddTimeout(timeout);
            actionNode.nodeTimeout.LoadTask(this);

            actionNode.Evaluate();
            yield return new WaitForSeconds(.1f);
            Assert.IsTrue(actionNode.CurrentState == expectedTransientState);
            Assert.IsTrue(actionNode.TimeoutActive());
            yield return new WaitForSeconds(timeoutDuration);
            Assert.IsTrue(actionNode.TimeoutExceeded());
            actionNode.Evaluate();
            Assert.IsTrue(actionNode.CurrentState == expectedTimeoutState);
            actionNode.Evaluate();
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

    [UnityTest]
    public IEnumerator ActionNodeLoadTask_Test(){
        /*
         * Test if LoadTask gives expected outcome
         */

         yield return new MonoBehaviourTest<ActionNodeLoadTaskTest>();
    }

}
