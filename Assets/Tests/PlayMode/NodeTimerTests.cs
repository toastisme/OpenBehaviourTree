
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;
public class NodeTimerTests{

    public class NodeTimerStateTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class NodeTimerStateTest
         * Tests if NodeTimer gives expected states for different configurations
         */
        NodeTimer timer;
        bool testFinished;

        public bool IsTestFinished{
            get {return testFinished;}
        }

        void Start(){
            timer = new NodeTimer(
                timerVal:TestMock.TimerDuration()
            );
            timer.LoadTask(this);
            testFinished = false;
            StartCoroutine(RunTests());
        }

        IEnumerator RunTests(){

            // Test initial conditions
            Assert.AreEqual(false, timer.IsActive());
            Assert.AreEqual(false, timer.TimerExceeded());
            Assert.AreEqual(TestMock.TimerDuration(), timer.GetTimerVal());

            timer.StartTimer();

            // Expected state while running
            Assert.AreEqual(true, timer.IsActive());
            Assert.AreEqual(false, timer.TimerExceeded());
            yield return new WaitForSeconds(TestMock.TimerDuration() + .1f);

            // Expected state after running
            Assert.AreEqual(false, timer.IsActive());
            Assert.AreEqual(true, timer.TimerExceeded());

            // Expected state after resetting
            timer.ResetTimer();
            Assert.AreEqual(false, timer.IsActive());
            Assert.AreEqual(false, timer.TimerExceeded());

            timer.StartTimer();
            Assert.AreEqual(true, timer.IsActive());
            Assert.AreEqual(false, timer.TimerExceeded());

            // Expected state after stopping 
            timer.StopTimer();
            Assert.AreEqual(false, timer.IsActive());
            Assert.AreEqual(false, timer.TimerExceeded());

            testFinished = true;
            yield return null;
        }

    }

    [UnityTest]
    public IEnumerator NodeTimerState_Test()
    {
        /*
         * Test if NodeTimer states are set correctly
         */

        yield return new MonoBehaviourTest<NodeTimerStateTest>();
    }


}