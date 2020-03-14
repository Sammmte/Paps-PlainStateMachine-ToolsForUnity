using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class PlainStateMachineBuilderSettingsDrawer
    {
        private const PlainStateMachineGenericType DefaultStateIdType = PlainStateMachineGenericType.Int;
        private const PlainStateMachineGenericType DefaultTriggerType = PlainStateMachineGenericType.Int;

        private const float Width = 300, Height = 400, RightPadding = 10, TopPadding = 10;

        private PlainStateMachineGenericType _stateIdRepresentation { get; set; }
        private PlainStateMachineGenericType _triggerRepresentation { get; set; }

        public Type StateIdType { get; private set; }
        public Type TriggerType { get; private set; }

        public event Action<Type> OnStateIdTypeChanged;
        public event Action<Type> OnTriggerTypeChanged;

        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _controlsAreaStyle;

        private string _stateIdEnumTypeFullName = "";
        private string _triggerEnumTypeFullName = "";

        private PlainStateMachineBuilder PlainStateMachineBuilder { get; set; }

        public PlainStateMachineBuilderSettingsDrawer(PlainStateMachineBuilder builder = null)
        {
            PlainStateMachineBuilder = builder;

            _titleStyle = new GUIStyle();
            _titleStyle.padding = new RectOffset(20, 20, 20, 20);
            _titleStyle.alignment = TextAnchor.MiddleCenter;
            _titleStyle.fontSize = 20;

            _labelStyle = new GUIStyle();
            _labelStyle.wordWrap = true;

            _controlsAreaStyle = new GUIStyle();
            _controlsAreaStyle.padding = new RectOffset(20, 20, 20, 20);

            LoadStateIdTypeDataFrom(builder.StateIdType);
            LoadTriggerTypeDataFrom(builder.TriggerType);

            SetStateIdTypeByRepresentation();
            SetTriggerTypeByRepresentation();
        }

        private void LoadStateIdTypeDataFrom(Type type)
        {
            if (type == null)
                _stateIdRepresentation = DefaultStateIdType;

            if (type == typeof(int))
                _stateIdRepresentation = PlainStateMachineGenericType.Int;
            else if (type == typeof(float))
                _stateIdRepresentation = PlainStateMachineGenericType.Float;
            else if (type == typeof(string))
                _stateIdRepresentation = PlainStateMachineGenericType.String;
            else if (type.IsEnum)
            {
                _stateIdRepresentation = PlainStateMachineGenericType.Enum;
                _stateIdEnumTypeFullName = type.FullName;
            }
            else
            {
                _stateIdRepresentation = DefaultStateIdType;
            }
        }

        private void LoadTriggerTypeDataFrom(Type type)
        {
            if (type == null)
                _triggerRepresentation = DefaultTriggerType;

            if (type == typeof(int))
                _triggerRepresentation = PlainStateMachineGenericType.Int;
            else if (type == typeof(float))
                _triggerRepresentation = PlainStateMachineGenericType.Float;
            else if (type == typeof(string))
                _triggerRepresentation = PlainStateMachineGenericType.String;
            else if (type.IsEnum)
            {
                _triggerRepresentation = PlainStateMachineGenericType.Enum;
                _triggerEnumTypeFullName = type.FullName;
            }
            else
            {
                _triggerRepresentation = DefaultTriggerType;
            }
        }

        public void Draw(Rect windowRect)
        {
            var position = new Vector2(windowRect.size.x - RightPadding - Width, TopPadding);
            var size = new Vector2(Width, Height);

            var boxRect = new Rect(position, size);

            var previousColor = GUI.color;
            GUI.color = Color.gray;
            GUILayout.BeginArea(boxRect, GUI.skin.window);
            GUI.color = previousColor;

            DrawTitle();

            EditorGUI.BeginChangeCheck();
            DrawControls();

            if (EditorGUI.EndChangeCheck())
            {
                SetStateIdTypeByRepresentation();
                SetTriggerTypeByRepresentation();
            }

            GUILayout.EndArea();
        }

        private void SetStateIdTypeByRepresentation()
        {
            var previousType = StateIdType;

            if (_stateIdRepresentation == Editor.PlainStateMachineGenericType.Int)
                StateIdType = typeof(int);
            else if (_stateIdRepresentation == Editor.PlainStateMachineGenericType.Float)
                StateIdType = typeof(float);
            else if (_stateIdRepresentation == Editor.PlainStateMachineGenericType.String)
                StateIdType = typeof(string);
            else if (_stateIdRepresentation == Editor.PlainStateMachineGenericType.Enum)
            {
                if (string.IsNullOrEmpty(_stateIdEnumTypeFullName) == false)
                {
                    var enumType = GetTypeOf(_stateIdEnumTypeFullName);

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

        private void SetTriggerTypeByRepresentation()
        {
            var previousType = TriggerType;

            if (_triggerRepresentation == Editor.PlainStateMachineGenericType.Int)
                TriggerType = typeof(int);
            else if (_triggerRepresentation == Editor.PlainStateMachineGenericType.Float)
                TriggerType = typeof(float);
            else if (_triggerRepresentation == Editor.PlainStateMachineGenericType.String)
                TriggerType = typeof(string);
            else if (_triggerRepresentation == Editor.PlainStateMachineGenericType.Enum)
            {
                if (string.IsNullOrEmpty(_triggerEnumTypeFullName) == false)
                {
                    var enumType = GetTypeOf(_triggerEnumTypeFullName);

                    if (enumType != null)
                    {
                        TriggerType = enumType;
                    }
                }
            }

            if (previousType != TriggerType)
            {
                OnTriggerTypeChanged?.Invoke(TriggerType);
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
                if (_stateIdRepresentation == PlainStateMachineGenericType.Enum)
                {
                    DrawEnumTypeFieldFor("State Id Enum Type Full Name", ref _stateIdEnumTypeFullName);
                }

                DrawTriggerRepresentationField();
                if(_triggerRepresentation == PlainStateMachineGenericType.Enum)
                {
                    DrawEnumTypeFieldFor("Trigger Enum Type Full Name", ref _triggerEnumTypeFullName);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStateIdRepresentationField()
        {
            GUILayout.Label("State Id Type", _labelStyle);
            _stateIdRepresentation = (PlainStateMachineGenericType)EditorGUILayout.EnumPopup(_stateIdRepresentation);
        }

        private void DrawTriggerRepresentationField()
        {
            GUILayout.Label("Trigger Type", _labelStyle);
            _triggerRepresentation = (PlainStateMachineGenericType)EditorGUILayout.EnumPopup(_triggerRepresentation);
        }

        private void DrawTitle()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Builder Settings", _titleStyle);
            GUILayout.EndVertical();
        }

        private void DrawEnumTypeFieldFor(string title, ref string fullNameVariable)
        {
            GUILayout.Label(title, _labelStyle);

            fullNameVariable = EditorGUILayout.TextField(fullNameVariable);

            EditorGUILayout.HelpBox("If the enum type is a nested type, the name would be like this:\nTheNamespace.TheClass+NestedEnum", MessageType.Info);
        }
    }
}