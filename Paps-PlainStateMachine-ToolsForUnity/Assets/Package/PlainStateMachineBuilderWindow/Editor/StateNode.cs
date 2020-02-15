using UnityEngine;
using Paps.StateMachines;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StateNode
    {
        private const float Width = 200;
        private const float Height = 100;

        private Rect _nodeRect;
        private GUIStyle _style;

        private StateAssetField _stateAssetField;

        public bool IsDragged { get; private set; }

        public IState StateObject => _stateAssetField.StateAsset as IState;

        private static readonly Texture2D _asNormal = Resources.Load<Texture2D>("Paps/PlainStateMachine-ToolsForUnity/Textures/node_normal");
        private static readonly Texture2D _asInitial = Resources.Load<Texture2D>("Paps/PlainStateMachine-ToolsForUnity/Textures/node_initial");

        public StateNode(Vector2 position, ScriptableObject stateAsset)
        {
            _nodeRect = new Rect(position.x, position.y, Width, Height);

            _style = new GUIStyle();

            _stateAssetField = new StateAssetField(stateAsset);

            AsNormal();
        }

        public StateNode(Vector2 position) : this(position, null)
        {

        }

        public void Drag(Vector2 delta)
        {
            _nodeRect.position += delta;
        }

        public void Draw()
        {
            GUI.Box(_nodeRect, string.Empty, _style);
            _stateAssetField.Expose(ref _nodeRect, _style);
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
                if (_nodeRect.Contains(ev.mousePosition))
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

        public void AsNormal()
        {
            _style.normal.background = _asNormal;
        }

        public void AsInitial()
        {
            _style.normal.background = _asInitial;
        }
    }
}