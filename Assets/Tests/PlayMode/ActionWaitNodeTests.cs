using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;

public class ActionWaitNodeTests 
{

    TestMock testMock = new TestMock();

    [Test]
    public void ActionWaitNodeValues_Test()
    {

        /*
         * Test if WaitTime and RandomDeviation are set correctly
         */

        ActionWaitNode actionWaitNode = testMock.GetActionWaitNodeMock();
        actionWaitNode.AddMisc1(5f);
        Assert.AreEqual(actionWaitNode.WaitTime, 5f, 1E-5);
        actionWaitNode.AddMisc2(8f);
        Assert.AreEqual(actionWaitNode.RandomDeviation, 8f, 1E-5);

    }
}
