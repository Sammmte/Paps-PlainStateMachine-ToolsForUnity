using System;
using UnityEditor;
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

        public string Title { get; private set; }
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
            GUI.Box(_nodeRect, Title, _style);
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

    public class StateAssetField
    {
        private const float LeftPadding = 20, FieldTopPadding = 27, FieldSizeY = 17, LabelTopPadding = 10, LabelSizeY = 17;

        private static readonly Type _scriptableObjectType = typeof(ScriptableObject);

        public ScriptableObject StateAsset { get; private set; }

        public StateAssetField(ScriptableObject stateAsset)
        {
            StateAsset = stateAsset;
        }

        public void Expose(ref Rect containerRect, GUIStyle containerStyle)
        {
            ShowLabel(ref containerRect, containerStyle);

            EditorGUI.BeginChangeCheck();

            var preValidatedStateAsset = (ScriptableObject)EditorGUI.ObjectField(GetRect(ref containerRect, containerStyle), StateAsset, _scriptableObjectType, false);

            if (EditorGUI.EndChangeCheck())
            {
                if (preValidatedStateAsset is IState)
                {
                    StateAsset = preValidatedStateAsset;
                }
                else
                {
                    Debug.LogWarning("Scriptable object does not implements IState interface");
                }
            }
        }

        private Rect GetRect(ref Rect containerRect, GUIStyle containerStyle)
        {
            var position = new Vector2(containerRect.position.x + LeftPadding, containerRect.position.y + containerStyle.border.top + FieldTopPadding);
            var size = new Vector2(containerRect.size.x - (LeftPadding * 2), FieldSizeY);

            return new Rect(position, size);
        }

        private void ShowLabel(ref Rect containerRect, GUIStyle containerStyle)
        {
            var position = new Vector2(containerRect.position.x + LeftPadding, containerRect.position.y + containerStyle.border.top + LabelTopPadding);
            var size = new Vector2(containerRect.size.x - (LeftPadding * 2), LabelSizeY);

            var rect = new Rect(position, size);

            GUI.Label(rect, "State Asset");
        }
    }
}