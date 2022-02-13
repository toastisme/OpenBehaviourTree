using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Behaviour.BehaviourTreeTask
{

    /**
     * \class Wait
     * Wait task that can be run by an ActionNode in a BehaviourTree.
     */

    public float waitTime{get;set;}
    public float randomDeviation{get;set;}
    public Wait(){
    }
    public Wait(
        float waitTime,
        float randomDeviation
    ){
        this.waitTime = waitTime;
        this.randomDeviation = randomDeviation;
    }
    public override IEnumerator ExecuteTask(System.Action<Behaviour.NodeState> currentState){
        float wait = waitTime + Random.Range(-randomDeviation, randomDeviation);
        wait = wait < 0 ? 0 : wait;
        currentState(Behaviour.NodeState.Running);
        yield return new WaitForSeconds(wait);
        currentState(Behaviour.NodeState.Succeeded);
    }
}
