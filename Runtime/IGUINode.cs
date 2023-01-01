using UnityEngine;

namespace OpenBehaviourTree{
interface IGuiNode 
{
    /**
     * \interface OpenBehaviourTree.IGuiNode
     * Basic interface that all GuiNodes need to provide.
     */

    void Draw();
    void Drag(Vector2 delta);
}
}