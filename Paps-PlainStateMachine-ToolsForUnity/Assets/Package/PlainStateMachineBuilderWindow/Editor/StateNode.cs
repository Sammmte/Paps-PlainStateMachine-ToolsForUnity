using UnityEngine;
using Paps.StateMachines;
using System;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StateNode
    {
        private const float Width = 200;
        private const float Height = 150;

        private const int ControlPaddingLeft = 20, ControlPaddingRight = 20, ControlPaddingTop = 20, ControlPaddingBottom = 20;

        private static readonly Texture2D _asNormal = CreateNormalStateTexture();
        private static readonly Texture2D _asInitial = CreateInitialStateTexture();

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
        private GUIStyle _nodeStyle;
        private GUIStyle _controlsAreaStyle;

        private StateAssetField _stateAssetField;
        private StateIdDrawer _stateIdDrawer;
        private IStateIdValidator _stateIdValidator;

        public StateNode(Vector2 position, IStateIdValidator stateIdValidator, Type stateIdType = null, ScriptableObject stateAsset = null, object stateId = null)
        {
            _nodeRect = new Rect(position.x, position.y, Width, Height);
            _nodeStyle = new GUIStyle();

            _controlsAreaStyle = new GUIStyle();
            _controlsAreaStyle.padding = new RectOffset(ControlPaddingLeft, ControlPaddingRight, ControlPaddingTop, ControlPaddingBottom);

            _stateAssetField = new StateAssetField(stateAsset);

            _stateIdValidator = stateIdValidator;

            if(stateIdType != null)
                _stateIdDrawer = StateIdDrawerFactory.Create(stateIdType, _stateIdValidator, stateId);

            AsNormal();
        }

        private static Texture2D CreateNormalStateTexture()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.grey);
            texture.Apply();

            return texture;
        }

        private static Texture2D CreateInitialStateTexture()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, new Color(235, 149, 50)); //orange
            texture.Apply();

            return texture;
        }

        public void SetNewStateIdType(Type stateIdType)
        {
            if (stateIdType == null)
                _stateIdDrawer = null;
            else
                _stateIdDrawer = StateIdDrawerFactory.Create(stateIdType, _stateIdValidator);
        }

        public void ShowNullTypeDrawer()
        {
            EditorGUILayout.HelpBox("The state id type was not provided", MessageType.Warning);
        }

        public void Drag(Vector2 delta)
        {
            _nodeRect.position += delta;
        }

        public void Draw()
        {
            GUILayout.BeginArea(_nodeRect, _nodeStyle);
            DrawControls();
            GUILayout.EndArea();
        }

        private void DrawControls()
        {
            EditorGUILayout.BeginVertical(_controlsAreaStyle);
            DrawStateIdDrawer();
            DrawStateAssetField();
            EditorGUILayout.EndVertical();
        }

        private void DrawStateAssetField()
        {
            _stateAssetField.Draw();
        }

        private void DrawStateIdDrawer()
        {
            if(_stateIdDrawer == null)
                ShowNullTypeDrawer();
            else
                _stateIdDrawer.Draw();
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
            _nodeStyle.normal.background = _asNormal;
        }

        public void AsInitial()
        {
            _nodeStyle.normal.background = _asInitial;
        }

        private Rect GetRect(ref Rect containerRect, GUIStyle containerStyle, float leftPadding, float topPadding, float sizeY)
        {
            var position = new Vector2(containerRect.position.x + leftPadding, containerRect.position.y + containerStyle.border.top + topPadding);
            var size = new Vector2(containerRect.size.x - (leftPadding * 2), sizeY);

            return new Rect(position, size);
        }
    }
}