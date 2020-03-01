using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class PlainStateMachineBuilderSettingsDrawer
    {
        private const StateIdType DefaultStateIdType = Editor.StateIdType.Int;

        private const float Width = 300, Height = 400, RightPadding = 10, TopPadding = 10;

        private StateIdType _stateIdRepresentation { get; set; }

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

            LoadStateIdTypeDataFrom(builder.StateIdType);

            SetStateIdTypeByRepresentation();
        }

        private void LoadStateIdTypeDataFrom(Type type)
        {
            if (type == null)
                _stateIdRepresentation = DefaultStateIdType;

            if (type == typeof(int))
                _stateIdRepresentation = Editor.StateIdType.Int;
            else if (type == typeof(float))
                _stateIdRepresentation = Editor.StateIdType.Float;
            else if (type == typeof(string))
                _stateIdRepresentation = Editor.StateIdType.String;
            else if (type.IsEnum)
            {
                _stateIdRepresentation = Editor.StateIdType.Enum;
                _enumTypeFullName = type.FullName;
            }
            else
            {
                _stateIdRepresentation = DefaultStateIdType;
            }
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
            }

            GUILayout.EndArea();
        }

        private void SetStateIdTypeByRepresentation()
        {
            var previousType = StateIdType;

            if (_stateIdRepresentation == Editor.StateIdType.Int)
                StateIdType = typeof(int);
            else if (_stateIdRepresentation == Editor.StateIdType.Float)
                StateIdType = typeof(float);
            else if (_stateIdRepresentation == Editor.StateIdType.String)
                StateIdType = typeof(string);
            else if (_stateIdRepresentation == Editor.StateIdType.Enum)
            {
                if (string.IsNullOrEmpty(_enumTypeFullName) == false)
                {
                    var enumType = GetTypeOf(_enumTypeFullName);

                    if(enumType != null)
                    {
                        StateIdType = enumType;
                    }
                }
            }

            if(previousType != StateIdType)
            {
                OnStateIdTypeChanged?.Invoke(StateIdType);
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
            GUI.enabled = false;
            EditorGUILayout.ObjectField(PlainStateMachineBuilder, typeof(PlainStateMachineBuilder), false);
            GUI.enabled = true;
        }

        private void DrawControls()
        {
            EditorGUILayout.BeginVertical(_controlsAreaStyle);

            DrawBuilderField();

            GUILayout.Space(20);

            if (PlainStateMachineBuilder != null)
            {
                DrawStateIdRepresentationField();
                if (_stateIdRepresentation == Editor.StateIdType.Enum)
                {
                    DrawEnumTypeField();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStateIdRepresentationField()
        {
            GUILayout.Label("State Id Type", _labelStyle);
            _stateIdRepresentation = (StateIdType)EditorGUILayout.EnumPopup(_stateIdRepresentation);
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