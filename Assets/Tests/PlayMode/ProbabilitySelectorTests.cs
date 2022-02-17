
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;
public class ProbabilitySelectorTests
{
    public class ProbabilitySelectorStateTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class ProbabilitySelectorStateTest
         * Tests if ProbabilitySelector gives expected states for different configurations
         */

        ProbabilitySelector probabilitySelector;
        ActionNode actionNode1;
        ActionNode actionNode2;
        ProbabilityWeight probabilityWeight1;
        ProbabilityWeight probabilityWeight2;
        TestMock testMock = new TestMock();
        bool testFinished;

        public bool IsTestFinished{
            get {return testFinished;}
        }

        void Start(){
            BehaviourTreeBlackboard blackboard = testMock.GetTestBlackboard();

            probabilitySelector = new ProbabilitySelector(
                taskName:"Probability Selector",
                blackboard:ref blackboard
            );
            actionNode1 = testMock.GetActionNodeMock(
                fail:true
            );
            actionNode1.LoadTask(this);
            actionNode2 = testMock.GetActionNodeMock(
                fail:false
            );
            actionNode2.LoadTask(this);

            probabilityWeight1 = new ProbabilityWeight(
                taskName:"Constant Weight",
                parentNode:probabilitySelector,
                childNode:actionNode1
            );
            actionNode1.SetParentNode(probabilityWeight1);

            probabilityWeight2 = new ProbabilityWeight(
                taskName:"Constant Weight",
                parentNode:probabilitySelector,
                childNode:actionNode2
            );
            actionNode2.SetParentNode(probabilityWeight2);

            probabilitySelector.AddChildNode(
                probabilityWeight1
            );
            probabilitySelector.AddChildNode(
                probabilityWeight2
            );

            testFinished = false;
            StartCoroutine(RunTests());
        }

        IEnumerator RunTests(){

            bool shouldFail = false;
            probabilitySelector.Evaluate();
            Assert.AreEqual(NodeState.Running, probabilitySelector.CurrentState);
            Assert.IsTrue(actionNode1.CurrentState == NodeState.Running || actionNode2.CurrentState == NodeState.Running);
            if (actionNode1.CurrentState == NodeState.Running){
                Assert.AreEqual(NodeState.Idle, actionNode2.CurrentState);
                shouldFail = true;
            }
            else{
                Assert.AreEqual(NodeState.Idle, actionNode1.CurrentState);
            }
            yield return new WaitForSeconds(TestMock.ActionDuration() + .1f);

            probabilitySelector.Evaluate();
            if (shouldFail){
                Assert.AreEqual(NodeState.Failed, probabilitySelector.CurrentState);
                Assert.AreEqual(NodeState.Failed, actionNode1.CurrentState);
                Assert.AreEqual(NodeState.Idle, actionNode2.CurrentState);
            } 
            else{
                Assert.AreEqual(NodeState.Succeeded, probabilitySelector.CurrentState);
                Assert.AreEqual(NodeState.Succeeded, actionNode2.CurrentState);
                Assert.AreEqual(NodeState.Idle, actionNode1.CurrentState);
            }
            probabilitySelector.ResetState();
            Assert.AreEqual(NodeState.Idle, probabilitySelector.CurrentState);
            Assert.AreEqual(NodeState.Idle, actionNode1.CurrentState);
            Assert.AreEqual(NodeState.Idle, actionNode2.CurrentState);
            Assert.AreEqual(NodeState.Idle, probabilityWeight1.CurrentState);
            Assert.AreEqual(NodeState.Idle, probabilityWeight2.CurrentState);

            testFinished = true;

            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator ProbabilitySelectorState_Test()
    {
        /*
         * Test if CurrentState is set correctly
         */

        yield return new MonoBehaviourTest<ProbabilitySelectorStateTest>();
    }

}