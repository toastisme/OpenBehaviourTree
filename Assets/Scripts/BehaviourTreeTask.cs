using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourBase{
    public abstract class BehaviourTreeTask  
    {
        BehaviourTreeBlackboard blackboard;

        public void SetBlackboard(ref BehaviourTreeBlackboard blackboard){
            this.blackboard = blackboard;
        }

        public virtual void Setup(){
            /*
             * Load components needed in ExecuteTask here
             */
        }
        
        public abstract IEnumerator ExecuteTask(System.Action<NodeState> nodeState);



    }
}