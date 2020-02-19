using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class PlainStateMachineBuilderSettingsDrawer
    {
        private const float Width = 300, Height = 400, RightPadding = 10, TopPadding = 10;

        public StateIdType StateIdRepresentation { get; private set; }

        public Type StateIdType { get; private set; }

        public event Action<Type> OnStateIdTypeChanged;

        private GUIStyle _boxStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _controlsAreaStyle;

        private string _enumTypeFullName = "";

        public PlainStateMachineBuilder PlainStateMachineBuilder { get; private set; }

        public PlainStateMachineBuilderSettingsDrawer(PlainStateMachineBuilder builder = null)
        {
            PlainStateMachineBuilder = builder;

            _boxStyle = new GUIStyle();

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.grey);
            texture.Apply();

            _boxStyle.normal.background = texture;

            _titleStyle = new GUIStyle();
            _titleStyle.padding = new RectOffset(20, 20, 20, 20);
            _titleStyle.alignment = TextAnchor.MiddleCenter;
            _titleStyle.fontSize = 20;

            _labelStyle = new GUIStyle();
            _labelStyle.wordWrap = true;

            _controlsAreaStyle = new GUIStyle();
            _controlsAreaStyle.padding = new RectOffset(20, 20, 20, 20);

            SetStateIdTypeByRepresentation();
        }

        public void Draw(Rect windowRect)
        {
            var position = new Vector2(windowRect.size.x - RightPadding - Width, TopPadding);
            var size = new Vector2(Width, Height);

            var boxRect = new Rect(position, size);

            GUILayout.BeginArea(boxRect, _boxStyle);

            DrawTitle();

            EditorGUI.BeginChangeCheck();
            DrawControls();

            if (EditorGUI.EndChangeCheck())
            {
                SetStateIdTypeByRepresentation();
                OnStateIdTypeChanged?.Invoke(StateIdType);
            }

            GUILayout.EndArea();
        }

        private void SetStateIdTypeByRepresentation()
        {
            if (StateIdRepresentation == Editor.StateIdType.Int)
                StateIdType = typeof(int);
            else if (StateIdRepresentation == Editor.StateIdType.Float)
                StateIdType = typeof(float);
            else if (StateIdRepresentation == Editor.StateIdType.String)
                StateIdType = typeof(string);
            else if (StateIdRepresentation == Editor.StateIdType.Enum)
            {
                if (string.IsNullOrEmpty(_enumTypeFullName) == false)
                {
                    StateIdType = GetTypeOf(_enumTypeFullName);
                }
                else
                {
                    StateIdType = null;
                }
            }
        }

        private Type GetTypeOf(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);

                if (type != null)
                    return type;
            }

            return null;
        }

        private void DrawBuilderField()
        {
            GUILayout.Label("Plain State Machine Builder", _labelStyle);
            PlainStateMachineBuilder = (PlainStateMachineBuilder)EditorGUILayout.ObjectField(PlainStateMachineBuilder, typeof(ScriptableObject), false);
        }

        private void DrawControls()
        {
            EditorGUILayout.BeginVertical(_controlsAreaStyle);

            DrawBuilderField();

            GUILayout.Space(20);

            if (PlainStateMachineBuilder != null)
            {
                DrawStateIdRepresentationField();
                if (StateIdRepresentation == Editor.StateIdType.Enum)
                {
                    DrawEnumTypeField();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStateIdRepresentationField()
        {
            GUILayout.Label("State Id Type", _labelStyle);
            StateIdRepresentation = (StateIdType)EditorGUILayout.EnumPopup(StateIdRepresentation);
        }

        private void DrawTitle()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Builder Settings", _titleStyle);
            GUILayout.EndVertical();
        }

        private void DrawEnumTypeField()
        {
            GUILayout.Label("Enum Type Full Name", _labelStyle);

            _enumTypeFullName = EditorGUILayout.TextField(_enumTypeFullName);

            EditorGUILayout.HelpBox("If the enum type is a nested type, the name would be like this:\nTheNamespace.TheClass+NestedEnum", MessageType.Info);
        }
    }
}