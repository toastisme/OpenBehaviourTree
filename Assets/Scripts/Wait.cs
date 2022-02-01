using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Behaviour.BehaviourTreeTask
{

    public float waitTime{get;set;}
    public Wait(){
    }
    public Wait(
        float waitTime
    ){
        this.waitTime = waitTime;
    }
    public override IEnumerator ExecuteTask(System.Action<Behaviour.NodeState> currentState){
        currentState(Behaviour.NodeState.Running);
        Debug.Log($"Running wait task ({waitTime})");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Finished task");
        currentState(Behaviour.NodeState.Succeeded);
    }
}
