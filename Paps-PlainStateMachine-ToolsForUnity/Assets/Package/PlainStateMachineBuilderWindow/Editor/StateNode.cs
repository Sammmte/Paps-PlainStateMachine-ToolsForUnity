using UnityEngine;
using Paps.StateMachines;
using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StateNode
    {
        private const float Width = 200;
        private const float Height = 150;

        private static readonly float StateIdDrawerLeftPadding = 20, StateIdDrawerTopPadding = 20, StateIdDrawerSizeY = 17;
        private static readonly float StateAssetFieldLeftPadding = 20, StateAssetFieldTopPadding = StateIdDrawerTopPadding + 50, StateAssetFieldSizeY = 17;

        private static readonly Texture2D _asNormal = Resources.Load<Texture2D>("Paps/PlainStateMachine-ToolsForUnity/Textures/node_normal");
        private static readonly Texture2D _asInitial = Resources.Load<Texture2D>("Paps/PlainStateMachine-ToolsForUnity/Textures/node_initial");

        public bool IsDragged { get; private set; }

        public object StateId
        {
            get
            {
                if (_stateIdDrawer == null)
                    return null;
                else
                    return _stateIdDrawer.StateId;
            }
        }

        public IState StateObject => _stateAssetField.StateAsset as IState;

        private Rect _nodeRect;
        private GUIStyle _style;

        private StateAssetField _stateAssetField;
        private StateIdDrawer _stateIdDrawer;
        private IStateIdValidator _stateIdValidator;

        public StateNode(Vector2 position, IStateIdValidator stateIdValidator, Type stateIdType = null, ScriptableObject stateAsset = null, object stateId = null)
        {
            _nodeRect = new Rect(position.x, position.y, Width, Height);
            _style = new GUIStyle();
            _stateAssetField = new StateAssetField(stateAsset);

            _stateIdValidator = stateIdValidator;

            SetNewStateIdType(stateIdType);

            AsNormal();
        }

        public void SetNewStateIdType(Type stateIdType)
        {
            if (stateIdType == null)
                _stateIdDrawer = null;
            else
                _stateIdDrawer = StateIdDrawerFactory.Create(stateIdType, _stateIdValidator, StateId);
        }

        public void ShowNullTypeDrawer()
        {

        }

        public void Drag(Vector2 delta)
        {
            _nodeRect.position += delta;
        }

        public void Draw()
        {
            DrawBox();
            DrawStateIdDrawer();
            DrawStateAssetField();
        }

        private void DrawBox()
        {
            GUI.Box(_nodeRect, string.Empty, _style);
        }

        private void DrawStateAssetField()
        {
            _stateAssetField.Draw(GetRect(ref _nodeRect, _style, StateAssetFieldLeftPadding, StateAssetFieldTopPadding, StateAssetFieldSizeY));
        }

        private void DrawStateIdDrawer()
        {
            if(_stateIdDrawer == null)
                ShowNullTypeDrawer();
            else
                _stateIdDrawer.Draw(GetRect(ref _nodeRect, _style, StateIdDrawerLeftPadding, StateIdDrawerTopPadding, StateIdDrawerSizeY));
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

        private Rect GetRect(ref Rect containerRect, GUIStyle containerStyle, float leftPadding, float topPadding, float sizeY)
        {
            var position = new Vector2(containerRect.position.x + leftPadding, containerRect.position.y + containerStyle.border.top + topPadding);
            var size = new Vector2(containerRect.size.x - (leftPadding * 2), sizeY);

            return new Rect(position, size);
        }
    }
}