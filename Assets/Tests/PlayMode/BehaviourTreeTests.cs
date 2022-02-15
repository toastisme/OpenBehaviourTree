using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;

public class BehaviourTreeTests 
{
    public class BehaviourTreeLoadTreeTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class BehaviourTreeLoadTreeTest
         * Tests if BehaviourTree.LoadTree gives expected results
         */

         TestMock testMock = new TestMock();
        bool testFinished;

        public bool IsTestFinished{
            get {return testFinished;}
        }
         
         void Start(){
             testFinished = false;
             BehaviourTree bt = testMock.GetTestBehaviourTree();
             BehaviourTreeBlackboard blackboard = testMock.GetTestBlackboard();
             bt.LoadTree(monoBehaviour:this, blackboard:ref blackboard);
             CheckTree(bt);
         }

         void CheckTree(BehaviourTree bt){
            /*
             * Assumes a fixed layout of TestBehaviourTree
             */

            // Test root
            Assert.IsTrue(bt.rootNode is PrioritySelector);
            Assert.AreEqual(bt.rootNode.ChildNodes.Count, 3);

            // Test priority child node
            Node pcn = bt.rootNode.ChildNodes[0];
            Assert.IsTrue(pcn is PrioritySelector);
            Assert.IsTrue(pcn.HasTimeout());
            Assert.AreEqual(pcn.GetTimeout().GetTimerVal(), 2, 1E-5);
            Assert.AreEqual(pcn.ChildNodes.Count, 1);
            Assert.IsTrue(pcn.ChildNodes[0] is Decorator);
            Assert.AreEqual(pcn.ChildNodes[0].TaskName, "testBool");
            Assert.AreEqual(pcn.ChildNodes[0].ChildNodes.Count, 1);
            Assert.IsTrue(pcn.ChildNodes[0].ChildNodes[0] is ActionNode);
            Assert.AreEqual(pcn.ChildNodes[0].ChildNodes[0].TaskName, "ActionMockFail");

            // Test sequence child node
            Node scn = bt.rootNode.ChildNodes[1];
            Assert.IsTrue(scn is SequenceSelector);
            Assert.IsTrue(scn.HasCooldown());
            Assert.AreEqual(scn.GetCooldown().GetTimerVal(), 3, 1E-5);
            Assert.AreEqual(scn.ChildNodes.Count, 1);
            Assert.IsTrue(scn.ChildNodes[0] is ActionWaitNode);
            ActionWaitNode awn = (ActionWaitNode)scn.ChildNodes[0];
            Assert.AreEqual(awn.WaitTime, 10, 1E-5);
            Assert.AreEqual(awn.RandomDeviation, 1, 1E-5);

            // Test probability child node
            Node prcn = bt.rootNode.ChildNodes[2];
            Assert.IsTrue(prcn is ProbabilitySelector);
            Assert.IsTrue(prcn.HasCooldown());
            Assert.AreEqual(prcn.GetCooldown().GetTimerVal(), 4, 1E-5);
            Assert.IsTrue(prcn.HasTimeout());
            Assert.AreEqual(prcn.GetTimeout().GetTimerVal(), 2, 1E-5);
            Assert.AreEqual(prcn.ChildNodes.Count, 2);
            
            Assert.IsTrue(prcn.ChildNodes[0] is ProbabilityWeight);
            ProbabilityWeight pw1 = (ProbabilityWeight)prcn.ChildNodes[0];
            Assert.IsTrue(pw1.HasConstantWeight());
            Assert.AreEqual(pw1.GetWeight(), 2);
            Assert.AreEqual(pw1.ChildNodes.Count, 1);
            Assert.IsTrue(pw1.ChildNodes[0] is ActionNode);
            Assert.AreEqual(pw1.ChildNodes[0].TaskName, "ActionMockSucceed");

            Assert.IsTrue(prcn.ChildNodes[1] is ProbabilityWeight);
            pw1 = (ProbabilityWeight)prcn.ChildNodes[1];
            Assert.IsTrue(!pw1.HasConstantWeight());
            Assert.AreEqual(pw1.GetWeight(), 0);
            Assert.AreEqual(pw1.ChildNodes.Count, 1);
            Assert.IsTrue(pw1.ChildNodes[0] is ActionNode);
            Assert.AreEqual(pw1.ChildNodes[0].TaskName, "ActionMockFail");

            testFinished=true;

         }
    }

    [UnityTest]
    public IEnumerator BehaviourTreeLoadTree_Test()
    {
        /*
         * Test if LoadTree gives the expected result
         */

        yield return new MonoBehaviourTest<BehaviourTreeLoadTreeTest>();
    }


}
