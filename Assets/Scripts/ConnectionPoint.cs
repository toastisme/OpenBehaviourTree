using System;
using UnityEngine;

namespace Behaviour{
    public enum ConnectionPointType { In, Out }

    public class ConnectionPoint
    {
        private Rect rect;
        private Rect apparentRect;
        private Rect nodeRect;

        public ConnectionPointType type;

        public CompositeGuiNode node;

        public GUIStyle style;

        public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(CompositeGuiNode node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
        {
            this.node = node;
            nodeRect = node.GetRect();
            this.type = type;
            this.style = style;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, nodeRect.width -12f, 20f);
            apparentRect = new Rect(0, 0, nodeRect.width -12f, 20f);

        }

        public CompositeGuiNode GetNode(){return node;}
        public Rect GetRect(){
            return rect;
        }

        public Rect GetApparentRect(){
            return apparentRect;
        }

        public void UpdateOrigin(Vector2 origin){
            apparentRect = new Rect(rect.x, rect.y, rect.width, rect.height);
            apparentRect.position -= origin;
        }


        public void Draw()
        {
            nodeRect = node.GetRect();
            rect.x = nodeRect.x + (nodeRect.width * 0.5f) - rect.width * 0.5f;

            switch (type)
            {
                case ConnectionPointType.In:
                rect.y = nodeRect.y + nodeRect.height - rect.height * .5f;
                break;

                case ConnectionPointType.Out:
                rect.y = nodeRect.y - rect.height*.5f;
                break;
            }

            if (GUI.Button(apparentRect, "", style))
            {
                if (OnClickConnectionPoint != null)
                {
                    OnClickConnectionPoint(this);
                }
            }
        }
    }
}