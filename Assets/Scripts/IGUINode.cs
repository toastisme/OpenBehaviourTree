using System;
using UnityEngine;

namespace BehaviourBase{
    interface IGUINode 
    {
        void Drag(Vector2 delta);
        void Draw();
        bool ProcessEvents(Event e);
        void SetSelected(bool selected);
    }
}