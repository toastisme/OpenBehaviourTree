using System;
using UnityEngine;

namespace BehaviourBase{
    public enum ConnectionPointType { In, Out }

    public class ConnectionPoint
    {
        private Rect rect;
        private Rect nodeRect;

        public ConnectionPointType type;

        public AggregateNode node;

        public GUIStyle style;

        public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(AggregateNode node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
        {
            this.node = node;
            nodeRect = node.GetRect();
            this.type = type;
            this.style = style;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, nodeRect.width -12f, 20f);
        }

        public AggregateNode GetNode(){return node;}
        public Rect GetRect(){
            return rect;
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

            if (GUI.Button(rect, "", style))
            {
                if (OnClickConnectionPoint != null)
                {
                    OnClickConnectionPoint(this);
                }
            }
        }
    }
}