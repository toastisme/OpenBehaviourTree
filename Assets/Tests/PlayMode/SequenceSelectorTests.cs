using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;
public class SequenceSelectorTests
{
    public class SequenceSelectorStateTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class SequenceSelectorStateTest
         * Tests if SequenceSelector gives expected states for different configurations
         */

        SequenceSelector sequenceSelector;
        ActionNode actionNode1;
        ActionNode actionNode2;
        TestMock testMock = new TestMock();
        bool testFinished;

        public bool IsTestFinished{
            get {return testFinished;}
        }

        void Start(){

            sequenceSelector = new SequenceSelector(
                taskName:"Sequence Selector"
            );
            actionNode1 = testMock.GetActionNodeMock(
                fail:true
            );
            actionNode1.LoadTask(this);
            actionNode2 = testMock.GetActionNodeMock(
                fail:false
            );
            actionNode2.LoadTask(this);
            sequenceSelector.AddChildNode(
                actionNode1
            );
            sequenceSelector.AddChildNode(
                actionNode2
            );

            testFinished = false;
            StartCoroutine(RunTests());
        }

        IEnumerator RunTests(){

            // First child will run but fail
            sequenceSelector.Evaluate();
            Assert.AreEqual(NodeState.Running, sequenceSelector.CurrentState);
            Assert.AreEqual(NodeState.Running, sequenceSelector.ChildNodes[0].CurrentState);
            Assert.AreEqual(NodeState.Idle, sequenceSelector.ChildNodes[1].CurrentState);

            // sequenceSelector will stop when this fails, child node 2 is never run
            yield return new WaitForSeconds(TestMock.ActionDuration() + .1f);
            sequenceSelector.Evaluate();
            Assert.AreEqual(NodeState.Failed, sequenceSelector.CurrentState);
            Assert.AreEqual(NodeState.Failed, sequenceSelector.ChildNodes[0].CurrentState);
            Assert.AreEqual(NodeState.Idle, sequenceSelector.ChildNodes[1].CurrentState);

            sequenceSelector.ResetState();
            Assert.AreEqual(NodeState.Idle, sequenceSelector.CurrentState);
            Assert.AreEqual(NodeState.Idle, sequenceSelector.ChildNodes[0].CurrentState);
            Assert.AreEqual(NodeState.Idle, sequenceSelector.ChildNodes[1].CurrentState);

            testFinished = true;

            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator SequenceSelectorState_Test()
    {
        /*
         * Test if CurrentState is set correctly
         */

        yield return new MonoBehaviourTest<SequenceSelectorStateTest>();
    }

}