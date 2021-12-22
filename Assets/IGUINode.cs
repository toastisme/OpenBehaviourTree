using System;
using UnityEngine;
interface IGUINode 
{
    void Drag(Vector2 delta);
    void Draw();
    bool ProcessEvents(Event e);
    void SetSelected(bool selected);
}
