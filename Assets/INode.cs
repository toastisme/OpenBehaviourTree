using System.Collections.Generic;

interface INode
{
    NodeState Evaluate();
    void ResetState();
    Node GetRunningLeafNode();
    List<Node> GetChildNodes();
    Node GetParentNode();
}
