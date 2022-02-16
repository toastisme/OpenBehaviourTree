using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Behaviour;
public class ProbabilityWeightTests 
{

    [Test]
    public void HasConstantWeight_Test(){
        ProbabilityWeight pw = new ProbabilityWeight(
            taskName:"Constant Weight"
        );
        Assert.IsTrue(pw.HasConstantWeight());

        pw.TaskName="testInt";  
        Assert.IsTrue(!pw.HasConstantWeight());
    }

    [Test]
    public void SetWeight_Test(){
        ProbabilityWeight pw = new ProbabilityWeight(
            taskName:"Constant Weight"
        );
        pw.SetWeight(5f);
        Assert.IsTrue(Mathf.Abs(5f-pw.GetWeight()) < 1e-5);
        
        pw.AddMisc1(10f);
        Assert.IsTrue(Mathf.Abs(10f-pw.GetWeight()) < 1e-5);

    }

}
