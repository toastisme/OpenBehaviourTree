using System.Collections.Generic;

namespace OpenBehaviourTree{

interface INode
{
    /**
     * \interface OpenBehaviourTree.INode
     * Basic interface that all Nodes need to provide.
     */

    NodeState Evaluate();
    void ResetState();
    Node GetRunningLeafNode();
    List<Node> GetChildNodes();
    Node GetParentNode();
}

}