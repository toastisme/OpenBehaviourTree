
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;
public class DecoratorTests
{
    TestMock testMock = new TestMock();
    public class DecoratorStateTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class DecoratorStateTest
         * Tests if Decorator gives expected states for different configurations
         */

        Decorator decorator;
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

            decorator = testMock.GetDecoratorMock();
            ActionNode actionNode = (ActionNode)decorator.ChildNodes[0];
            actionNode.LoadTask(this);
            Assert.AreEqual(decorator.CurrentState, NodeState.Idle);

            // testBool is false
            decorator.Evaluate();
            Assert.AreEqual(decorator.CurrentState, NodeState.Failed);

            decorator.ResetState();
            Assert.AreEqual(decorator.CurrentState, NodeState.Idle);

            decorator.invertCondition = true;
            decorator.Evaluate();
            Assert.AreEqual(decorator.CurrentState, NodeState.Running);
            yield return new WaitForSeconds(TestMock.ActionDuration() + .1f);
            decorator.Evaluate();
            Assert.AreEqual(decorator.CurrentState, NodeState.Succeeded);


            decorator.ResetState();
            Assert.AreEqual(decorator.CurrentState, NodeState.Idle);
            Assert.AreEqual(decorator.ChildNodes[0].CurrentState, NodeState.Idle);

            testFinished = true;

            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator DecoratorState_Test()
    {
        /*
         * Test if CurrentState is set correctly
         */

        yield return new MonoBehaviourTest<DecoratorStateTest>();
    }

}