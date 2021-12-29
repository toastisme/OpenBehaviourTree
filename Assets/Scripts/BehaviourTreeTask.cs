using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourBase{
    public abstract class BehaviourTreeTask  
    {
        public abstract IEnumerator ExecuteTask(System.Action<NodeState> nodeState);

    }
}