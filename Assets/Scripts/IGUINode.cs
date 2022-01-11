using UnityEngine;

namespace Behaviour{
interface IGuiNode 
{
    void Draw();
    void Drag(Vector2 delta);
}
}