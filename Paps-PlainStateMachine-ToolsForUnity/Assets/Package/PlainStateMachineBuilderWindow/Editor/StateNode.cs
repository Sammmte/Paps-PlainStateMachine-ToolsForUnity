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
        private const float SelectedExtraPixels = 5;
        private static readonly float SelectedHalfExtraPixels = SelectedExtraPixels / 2;

        private static readonly Texture2D _asNormal = CreateBackgroundTexture(Color.grey);
        private static readonly Texture2D _asInitial = CreateBackgroundTexture(new Color(235f / 255f, 149f / 255f, 50f / 255f)); //orange

        private static readonly Texture2D _selectedTexture = CreateBackgroundTexture(new Color(44f / 255f, 130f / 255f, 201f / 255f)); //blue

        public bool IsDragged { get; private set; }
        public bool IsSelected { get; private set; }

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

        public Action<StateNode> OnSelected, OnDeselected;

        private Rect _nodeRect;
        private GUIStyle _nodeStyle;
        private GUIStyle _selectedNodeStyle;
        private GUIStyle _controlsAreaStyle;

        private StateAssetField _stateAssetField;
        private StateIdDrawer _stateIdDrawer;
        private IStateIdValidator _stateIdValidator;

        public StateNode(Vector2 position, IStateIdValidator stateIdValidator, Type stateIdType = null, ScriptableObject stateAsset = null, object stateId = null)
        {
            

            _nodeRect = new Rect(position.x, position.y, Width, Height);
            _nodeStyle = new GUIStyle();

            _selectedNodeStyle = new GUIStyle();
            _selectedNodeStyle.normal.background = _selectedTexture;

            _controlsAreaStyle = new GUIStyle();
            _controlsAreaStyle.padding = new RectOffset(ControlPaddingLeft, ControlPaddingRight, ControlPaddingTop, ControlPaddingBottom);

            _stateAssetField = new StateAssetField(stateAsset);

            _stateIdValidator = stateIdValidator;

            if(stateIdType != null)
                _stateIdDrawer = StateIdDrawerFactory.Create(stateIdType, _stateIdValidator, stateId);

            AsNormal();
        }

        private static Texture2D CreateBackgroundTexture(Color color)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
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
            if(IsSelected)
                DrawAsSelected();

            GUILayout.BeginArea(_nodeRect, _nodeStyle);
            DrawControls();
            GUILayout.EndArea();
        }

        private void DrawAsSelected()
        {

            var selectedNodeRect = new Rect(
                new Vector2(_nodeRect.position.x - SelectedHalfExtraPixels, _nodeRect.position.y - SelectedHalfExtraPixels),
                new Vector2(_nodeRect.size.x + SelectedExtraPixels, _nodeRect.size.y + SelectedExtraPixels)
                );

            GUILayout.BeginArea(selectedNodeRect, _selectedNodeStyle);
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
                    
                    if(ClickedOver(ev))
                    {
                        IsDragged = true;
                        IsSelected = true;
                        OnSelected?.Invoke(this);
                        GUI.changed = true;
                    }
                    else
                    {
                        bool wasSelected = IsSelected;
                        IsSelected = false;

                        if (wasSelected)
                            OnDeselected?.Invoke(this);

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

        private bool ClickedOver(Event mouseClickEvent)
        {
            if (mouseClickEvent.button == 0 && _nodeRect.Contains(mouseClickEvent.mousePosition))
            {
                if (_nodeRect.Contains(mouseClickEvent.mousePosition))
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

        public Rect GetRect()
        {
            return _nodeRect;
        }
    }
}