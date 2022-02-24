using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public abstract class BehaviourTreeTask  
{

    /**
    * \class Behaviour.BehaviourTreeTask
    * Base class for all BehaviourTree tasks called from ActionNodes.
    * Setup is used to get GameObject components needed at runtime
    * (e.g. NavMeshAgent, RigidBody etc.).
    * ExecuteTask is the coroutine that is run when Evaluate is called on
    * the ActionNode. ExecuteTask is given the ActionNode NodeState,
    * and can set it based on the internals of the task.
    * (i.e setting it to NodeState.Running when starting the task, and
    * NodeState.Succeeded when finishing successfully)
    */

    protected BehaviourTreeBlackboard blackboard;
    MonoBehaviour monoBehaviour;

    public void SetBlackboard(ref BehaviourTreeBlackboard blackboard){
        this.blackboard = blackboard;
    }

    public virtual void Setup(MonoBehaviour monoBehaviour){
        /*
        * Load components needed in ExecuteTask here
        */
        this.monoBehaviour = monoBehaviour;
    }
    
    public abstract IEnumerator ExecuteTask(System.Action<NodeState> currentState);



}
}