using System.Collections.Generic;

namespace Behaviour{

interface INode
{
    /**
     * \interface INode
     * Basic interface that all Nodes need to provide.
     */

    NodeState Evaluate();
    void ResetState();
    Node GetRunningLeafNode();
    List<Node> GetChildNodes();
    Node GetParentNode();
}

}