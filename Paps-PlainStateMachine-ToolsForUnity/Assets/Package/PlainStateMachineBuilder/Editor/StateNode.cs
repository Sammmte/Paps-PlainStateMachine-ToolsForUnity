using System;
using UnityEditor;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class StateNode
    {
        private const float Width = 200;
        private const float Height = 150;

        private const int ControlPaddingLeft = 20, ControlPaddingRight = 20, ControlPaddingTop = 20, ControlPaddingBottom = 20;
        private const float SelectedExtraPixels = 5;
        private static readonly float SelectedHalfExtraPixels = SelectedExtraPixels / 2;

        private static readonly Texture2D _asNormal = CreateBackgroundTexture(Color.grey);
        private static readonly Texture2D _asInitial = CreateBackgroundTexture(new Color(235f / 255f, 149f / 255f, 50f / 255f)); //orange

        private static readonly Texture2D _selectedTexture = CreateBackgroundTexture(new Color(44f / 255f, 130f / 255f, 201f / 255f)); //blue

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

        public ScriptableState StateObject { get; private set; }

        public Action<StateNode, object, object> OnStateIdChanged;
        public Action<StateNode, ScriptableState, ScriptableState> OnStateObjectChanged;
        public Action<StateNode, Vector2> OnPositionChanged;

        private Rect _nodeRect;
        private GUIStyle _nodeStyle;
        private GUIStyle _selectedNodeStyle;
        private GUIStyle _controlsAreaStyle;
        private GUIStyle _identityTitleStyle;
        private GUIStyle _identityStateIdStyle;

        private StateIdDrawer _stateIdDrawer;

        public Vector2 Position => _nodeRect.position;
        public Vector2 Center => _nodeRect.center;

        public StateNode(Vector2 position, Type stateIdType = null, ScriptableState stateAsset = null, object stateId = null)
        {
            _nodeRect = new Rect(position.x, position.y, Width, Height);
            _nodeStyle = new GUIStyle();

            _selectedNodeStyle = new GUIStyle();
            _selectedNodeStyle.normal.background = _selectedTexture;

            _controlsAreaStyle = new GUIStyle();
            _controlsAreaStyle.padding = new RectOffset(ControlPaddingLeft, ControlPaddingRight, ControlPaddingTop, ControlPaddingBottom);

            _identityTitleStyle = new GUIStyle();
            _identityTitleStyle.padding = new RectOffset(20, 20, 20, 20);
            _identityTitleStyle.alignment = TextAnchor.MiddleCenter;
            _identityTitleStyle.fontSize = 20;
            _identityTitleStyle.wordWrap = true;

            _identityStateIdStyle = new GUIStyle();
            _identityStateIdStyle.padding = new RectOffset(20, 20, 20, 20);
            _identityStateIdStyle.alignment = TextAnchor.MiddleCenter;
            _identityStateIdStyle.fontSize = 16;
            _identityStateIdStyle.wordWrap = true;

            _stateIdDrawer = StateIdDrawerFactory.Create(stateIdType, stateId);
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
                _stateIdDrawer = StateIdDrawerFactory.Create(stateIdType);
        }

        public void Drag(Vector2 delta)
        {
            _nodeRect.position += delta;
            OnPositionChanged?.Invoke(this, _nodeRect.position);
        }

        public void Draw(bool asSelected, bool asInitial)
        {
            if(asSelected)
                DrawSelectedOutline();

            if (asInitial)
                AsInitial();
            else
                AsNormal();

            GUILayout.BeginArea(_nodeRect, _nodeStyle);
            DrawIdentity();
            GUILayout.EndArea();
        }

        private void DrawSelectedOutline()
        {
            var selectedNodeRect = new Rect(
                new Vector2(_nodeRect.position.x - SelectedHalfExtraPixels, _nodeRect.position.y - SelectedHalfExtraPixels),
                new Vector2(_nodeRect.size.x + SelectedExtraPixels, _nodeRect.size.y + SelectedExtraPixels)
                );

            GUILayout.BeginArea(selectedNodeRect, _selectedNodeStyle);
            GUILayout.EndArea();
        }

        private void DrawIdentity()
        {
            if (StateObject != null)
            {
                if(string.IsNullOrEmpty(StateObject.DebugName))
                    GUILayout.Label("Nameless Object", _identityTitleStyle);
                else
                    GUILayout.Label(StateObject.DebugName, _identityTitleStyle);
            }
            else
                GUILayout.Label("Empty State", _identityTitleStyle);

            GUILayout.Space(20);

            if (StateId != null)
                GUILayout.Label("State Id: " + StateId.ToString(), _identityStateIdStyle);
        }

        public void DrawControls()
        {
            EditorGUILayout.BeginVertical(_controlsAreaStyle);

            var previousStateId = StateId;
            DrawStateIdDrawer();
            if (previousStateId != StateId) OnStateIdChanged(this, previousStateId, StateId);

            EditorGUI.BeginChangeCheck();
            var previousStateObject = StateObject;
            DrawStateAssetField();
            if (EditorGUI.EndChangeCheck()) OnStateObjectChanged(this, previousStateObject, StateObject);

            EditorGUILayout.EndVertical();
        }

        private void DrawStateAssetField()
        {
            GUILayout.Label("State Asset");
            StateObject = (ScriptableState)EditorGUILayout.ObjectField(StateObject, typeof(ScriptableState), false);
        }

        private void DrawStateIdDrawer()
        {
            _stateIdDrawer.Draw();
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