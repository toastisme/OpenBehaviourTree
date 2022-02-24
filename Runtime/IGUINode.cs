using UnityEngine;

namespace Behaviour{
interface IGuiNode 
{
    /**
     * \interface Behaviour.IGuiNode
     * Basic interface that all GuiNodes need to provide.
     */

    void Draw();
    void Drag(Vector2 delta);
}
}