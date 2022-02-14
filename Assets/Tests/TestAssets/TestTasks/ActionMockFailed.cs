using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMockFail : Behaviour.BehaviourTreeTask
{

    /**
     * \class ActionMockFail
     *  Test action that fails after .5f seconds
     */

    public override IEnumerator ExecuteTask(System.Action<Behaviour.NodeState> CurrentState){
        CurrentState(Behaviour.NodeState.Running);
        yield return new WaitForSeconds(Behaviour.TestMock.ActionDuration());
        CurrentState(Behaviour.NodeState.Failed);
        
    }
}
