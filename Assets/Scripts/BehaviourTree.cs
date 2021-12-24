using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourBase{
    public class BehaviourTree : ScriptableObject 
    {
        public List<AggregateNode> nodes;
        public List<Connection> connections;
        public BehaviourTreeBlackboard blackboard;
    }
}
