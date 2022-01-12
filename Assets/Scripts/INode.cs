using System.Collections.Generic;

namespace Behaviour{

    interface INode
    {
        NodeState Evaluate();
        void ResetState();
        Node GetRunningLeafNode();
        List<Node> GetChildNodes();
        Node GetParentNode();
    }

}