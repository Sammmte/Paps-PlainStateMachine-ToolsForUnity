using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.StateMachines.Unity.Editor
{
    public class StateNode
    {
        private Rect _rect;
        private GUIStyle _style;

        public string Title { get; private set; }
        public bool IsDragged { get; private set; }

        public StateNode(Vector2 position, float width, float height, GUIStyle nodeStyle)
        {
            _rect = new Rect(position.x, position.y, width, height);
            _style = nodeStyle;
        }

        public void Drag(Vector2 delta)
        {
            _rect.position += delta;
        }

        public void Draw()
        {
            GUI.Box(_rect, Title, _style);
        }

        public bool ProcessEvents(Event ev)
        {
            switch (ev.type)
            {
                case EventType.MouseDown:
                    
                    if(WantsToDrag(ev))
                    {
                        IsDragged = true;
                        GUI.changed = true;
                    }
                    else
                    {
                        GUI.changed = true;
                    }

                    break;

                case EventType.MouseUp:

                    IsDragged = false;

                    break;

                case EventType.MouseDrag:

                    if (IsDragging(ev))
                    {
                        Drag(ev.delta);
                        ev.Use();
                        return true;
                    }

                    break;
            }

            return false;
        }

        private bool WantsToDrag(Event ev)
        {
            if (ev.button == 0)
            {
                if (_rect.Contains(ev.mousePosition))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsDragging(Event ev)
        {
            if (ev.button == 0 && IsDragged)
            {
                return true;
            }

            return false;
        }
    }
}