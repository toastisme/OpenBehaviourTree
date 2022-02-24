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

            float epsilon = 1E-5f;

            // Test root
            Assert.IsTrue(bt.rootNode is PrioritySelector);
            Assert.AreEqual(bt.rootNode.ChildNodes.Count, 3);

            // Test BoolDecorator
            Node bd = bt.rootNode.ChildNodes[0];
            Assert.IsTrue(bd is BoolDecorator);
            BoolDecorator bdd = (BoolDecorator)bd;
            Assert.AreEqual(bd.ChildNodes.Count, 1);
            Assert.IsTrue(bdd.invertCondition == true);
            Assert.AreEqual(bd.TaskName, "testBool");

            // Test PrioritySelector
            Node ps = bd.ChildNodes[0];
            Assert.IsTrue(ps is PrioritySelector);
            PrioritySelector pss = (PrioritySelector)ps;
            Assert.AreEqual(ps.ChildNodes.Count, 2);
            Assert.AreEqual(ps.TaskName, "Priority Selector");

            // Test ActionWaitNode
            Node awn = ps.ChildNodes[0];
            Assert.IsTrue(awn is ActionWaitNode);
            ActionWaitNode awnn = (ActionWaitNode) awn;
            Assert.AreEqual(awnn.ChildNodes.Count, 0);
            Assert.AreEqual(awnn.TimerValue, 10, epsilon);
            Assert.AreEqual(awnn.RandomDeviation, 2, epsilon);
            Assert.AreEqual(awn.TaskName, "Wait");

            // TestActionMockFail
            Node an1 = ps.ChildNodes[1];
            Assert.IsTrue(an1 is ActionNode);
            Assert.AreEqual(an1.TaskName, "ActionMockFail");

            // Test TimeoutNode
            Node ton = bt.rootNode.ChildNodes[1];
            Assert.IsTrue(ton is TimeoutNode);
            TimeoutNode tonn = (TimeoutNode)ton;
            Assert.AreEqual(tonn.valueKey, "testInt");
            Assert.AreEqual(tonn.randomDeviationKey, "testFloat");
            Assert.AreEqual(ton.TaskName, "Timeout");
            Assert.AreEqual(ton.ChildNodes.Count, 1);

            // Test SequenceSelector
            Node ss = ton.ChildNodes[0];
            Assert.IsTrue(ss is SequenceSelector);
            Assert.AreEqual(ss.TaskName, "Sequence Selector");
            Assert.AreEqual(ss.ChildNodes.Count, 1);

            // Test ActionMockSucceed
            Node an2 = ss.ChildNodes[0];
            Assert.IsTrue(an2 is ActionNode);
            Assert.AreEqual(an2.TaskName, "ActionMockSucceed");

            // Test CooldownNode
            Node cn = bt.rootNode.ChildNodes[2];
            Assert.IsTrue(cn is CooldownNode);
            CooldownNode cnn = (CooldownNode)cn;
            Assert.AreEqual(cn.TaskName, "Cooldown");
            Assert.AreEqual(cn.ChildNodes.Count, 1);
            Assert.AreEqual(cnn.valueKey, "testFloat");
            Assert.AreEqual(cnn.RandomDeviation, 1, epsilon);

            // Test ProbabilitySelector
            Node pbs = cn.ChildNodes[0];
            Assert.IsTrue(pbs is ProbabilitySelector);
            Assert.AreEqual(pbs.TaskName, "Probability Selector");
            Assert.AreEqual(pbs.ChildNodes.Count, 2);

            // Test ProbabilityWeight with blackboard key as weight
            Node pw1 = pbs.ChildNodes[0];
            Assert.IsTrue(pw1 is ProbabilityWeight);
            ProbabilityWeight pww1 = (ProbabilityWeight)pw1;
            Assert.AreEqual(pww1.TaskName, "testInt");
            Assert.AreEqual(pw1.ChildNodes.Count, 1);
            Assert.IsTrue(pw1.ChildNodes[0] is ActionNode);

            // Test ProbabilityWeight with constant value as weight
            Node pw2 = pbs.ChildNodes[1];
            Assert.IsTrue(pw2 is ProbabilityWeight);
            ProbabilityWeight pww2 = (ProbabilityWeight)pw2;
            Assert.AreEqual(pw2.TaskName, "Constant Weight");
            Assert.AreEqual(pww2.GetWeight(), 1, epsilon);
            Assert.AreEqual(pw2.ChildNodes.Count, 1);

            // Test node having cooldown, timeout, and bool decorator
            Assert.IsTrue(pw2.ChildNodes[0] is CooldownNode);
            Assert.IsTrue(pw2.ChildNodes[0].ChildNodes[0] is TimeoutNode);
            Assert.IsTrue(pw2.ChildNodes[0].ChildNodes[0].ChildNodes[0] is BoolDecorator);

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

    public class BehaviourTreeResetTreeTest: MonoBehaviour, IMonoBehaviourTest{

        /**
         * \class BehaviourTreeResetTreeTest
         * Tests if BehaviourTree.ResetTree gives expected results
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
             ResetTreeTest(bt);
         }

         void UpdateStateRecursive(Node node, List<NodeState> ns, int idx){
             node.SetStateDebug(ns[idx]);
             idx++;
             idx = (idx % (ns.Count -1));
             foreach(Node childNode in node.ChildNodes){
                 UpdateStateRecursive(
                     node:childNode,
                     ns:ns,
                     idx:idx
                 );
             }
         }

         void CheckIdleRecursive(Node node){
             Assert.AreEqual(node.CurrentState, NodeState.Idle);
             foreach(Node childNode in node.ChildNodes){
                 CheckIdleRecursive(childNode);
             }
         }

         void CheckNotIdleRecursive(Node node){
             Assert.AreNotEqual(node.CurrentState, NodeState.Idle);
             foreach(Node childNode in node.ChildNodes){
                 CheckNotIdleRecursive(childNode);
             }
         }

         void ResetTreeTest(BehaviourTree bt){

            //Check all are initially idle
            CheckIdleRecursive(bt.rootNode);

            // Set nodes to different states other than Idle
            List<NodeState> ns = new List<NodeState>(){
                NodeState.Failed,
                NodeState.Succeeded,
                NodeState.Running,
                };

            UpdateStateRecursive(bt.rootNode, ns, 0);

            // Check none are now Idle
            CheckNotIdleRecursive(bt.rootNode);

            bt.ResetTree();
            //Check all are now idle
            CheckIdleRecursive(bt.rootNode);

            testFinished = true;

            
         }
    }

    [UnityTest]
    public IEnumerator BehaviourTreeResetTree_Test()
    {
        /*
         * Test if LoadTree gives the expected result
         */

        yield return new MonoBehaviourTest<BehaviourTreeResetTreeTest>();
    }


}
