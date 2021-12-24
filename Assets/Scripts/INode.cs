using System.Collections.Generic;

namespace BehaviourBase{

    interface INode
    {
        NodeState Evaluate();
        void ResetState();
        Node GetRunningLeafNode();
        List<Node> GetChildNodes();
        Node GetParentNode();
    }

}