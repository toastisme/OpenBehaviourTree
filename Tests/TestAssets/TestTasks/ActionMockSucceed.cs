using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMockSucceed : Behaviour.BehaviourTreeTask
{
    /**
     * \class ActionMockSucceed
     *  Test action that succeeds after .5f seconds
     */

    public override IEnumerator ExecuteTask(System.Action<Behaviour.NodeState> CurrentState){
        CurrentState(Behaviour.NodeState.Running);
        yield return new WaitForSeconds(Behaviour.TestMock.ActionDuration());
        CurrentState(Behaviour.NodeState.Succeeded);
        
    }
}
