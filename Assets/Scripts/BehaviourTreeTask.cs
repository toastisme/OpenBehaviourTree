using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
    public abstract class BehaviourTreeTask  
    {
        BehaviourTreeBlackboard blackboard;
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
        
        public abstract IEnumerator ExecuteTask(System.Action<NodeState> NodeState);



    }
}