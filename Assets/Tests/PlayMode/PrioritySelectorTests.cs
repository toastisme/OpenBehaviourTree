
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;
public class PrioritySelectorTests
{
    public class PrioritySelectorStateTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class PrioritySelectorStateTest
         * Tests if PrioritySelector gives expected states for different configurations
         */

        PrioritySelector prioritySelector;
        ActionNode actionNode1;
        ActionNode actionNode2;
        TestMock testMock = new TestMock();
        bool testFinished;

        public bool IsTestFinished{
            get {return testFinished;}
        }

        void Start(){

            prioritySelector = new PrioritySelector(
                taskName:"Priority Selector"
            );
            actionNode1 = testMock.GetActionNodeMock(
                fail:true
            );
            actionNode1.LoadTask(this);
            actionNode2 = testMock.GetActionNodeMock(
                fail:false
            );
            actionNode2.LoadTask(this);
            prioritySelector.AddChildNode(
                actionNode1
            );
            prioritySelector.AddChildNode(
                actionNode2
            );

            testFinished = false;
            StartCoroutine(RunTests());
        }

        IEnumerator RunTests(){

            // First child will run but fail
            prioritySelector.Evaluate();
            Assert.AreEqual(NodeState.Running, prioritySelector.CurrentState);
            Assert.AreEqual(NodeState.Running, prioritySelector.ChildNodes[0].CurrentState);
            Assert.AreEqual(NodeState.Idle, prioritySelector.ChildNodes[1].CurrentState);

            // prioritySelector will continue running with second node
            yield return new WaitForSeconds(TestMock.ActionDuration() + .1f);
            prioritySelector.Evaluate();
            Assert.AreEqual(NodeState.Running, prioritySelector.CurrentState);
            Assert.AreEqual(NodeState.Failed,prioritySelector.ChildNodes[0].CurrentState);
            Assert.AreEqual(NodeState.Running,prioritySelector.ChildNodes[1].CurrentState);

            yield return new WaitForSeconds(TestMock.ActionDuration() + .1f);
            prioritySelector.Evaluate();
            Assert.AreEqual(NodeState.Succeeded, prioritySelector.CurrentState);
            Assert.AreEqual(NodeState.Failed,prioritySelector.ChildNodes[0].CurrentState);
            Assert.AreEqual(NodeState.Succeeded,prioritySelector.ChildNodes[1].CurrentState);

            prioritySelector.ResetState();
            Assert.AreEqual(NodeState.Idle, prioritySelector.CurrentState);
            Assert.AreEqual(NodeState.Idle, prioritySelector.ChildNodes[0].CurrentState);
            Assert.AreEqual(NodeState.Idle, prioritySelector.ChildNodes[1].CurrentState);

            testFinished = true;

            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator PrioritySelectorState_Test()
    {
        /*
         * Test if CurrentState is set correctly
         */

        yield return new MonoBehaviourTest<PrioritySelectorStateTest>();
    }

}