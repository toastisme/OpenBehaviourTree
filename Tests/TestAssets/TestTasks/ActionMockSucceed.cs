using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMockSucceed : OpenBehaviourTree.BehaviourTreeTask
{
    /**
     * \class ActionMockSucceed
     *  Test action that succeeds after .5f seconds
     */

    public override IEnumerator ExecuteTask(System.Action<OpenBehaviourTree.NodeState> CurrentState){
        CurrentState(OpenBehaviourTree.NodeState.Running);
        yield return new WaitForSeconds(OpenBehaviourTree.TestMock.ActionDuration());
        CurrentState(OpenBehaviourTree.NodeState.Succeeded);
        
    }
}
