using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : Behaviour.BehaviourTreeTask
{
    Rigidbody rb;
    public override IEnumerator ExecuteTask(System.Action<Behaviour.NodeState> currentState){
        currentState(Behaviour.NodeState.Running);
        Debug.Log("Running task (get rb wait for 5 seconds)");
        yield return new WaitForSeconds(5);
        Debug.Log("Finished task");
        currentState(Behaviour.NodeState.Succeeded);
    }

    public override void Setup(MonoBehaviour monoBehaviour){
        rb = monoBehaviour.GetComponent<Rigidbody>();        
    }
}