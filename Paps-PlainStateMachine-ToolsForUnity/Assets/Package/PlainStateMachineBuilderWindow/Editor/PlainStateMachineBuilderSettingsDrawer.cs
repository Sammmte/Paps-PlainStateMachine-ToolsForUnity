using UnityEngine;
using UnityEditor;
using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class PlainStateMachineBuilderSettingsDrawer
    {
        private const float Width = 300, Height = 400, RightPadding = 10, TopPadding = 10;
        private const float StateIdRepresentationFieldLeftPadding = 20, StateIdRepresentationFieldTopPadding = 40, StateIdRepresentationFieldSizeY = 17;
        private const float TitleLeftPadding = 20, TitleTopPadding = 20, TitleSizeY = 17;
        private const float EnumTypeFieldLeftPadding = 20, EnumTypeFieldTopPadding = 80, EnumTypeFieldSizeY = 17;
        private const float EnumTypeFieldTitleLeftPadding = 20, EnumTypeFieldTitleTopPadding = 60, EnumTypeFieldTitleSizeY = 17;

        public StateIdRepresentation StateIdRepresentation { get; private set; }

        public Type StateIdType { get; private set; }

        public event Action<Type> OnStateIdTypeChanged;

        private static readonly Texture2D _backgroundTexture = Resources.Load<Texture2D>("Paps/PlainStateMachine-ToolsForUnity/Textures/node_normal");

        private GUIStyle _style;

        private string _enumTypeFullName = "";

        public PlainStateMachineBuilderSettingsDrawer()
        {
            _style = new GUIStyle();
            _style.normal.background = _backgroundTexture;

            SetStateIdTypeByRepresentation();
        }

        public void Draw(Rect windowRect)
        {
            var position = new Vector2(windowRect.size.x - RightPadding - Width, TopPadding);
            var size = new Vector2(Width, Height);

            var containerRect = new Rect(position, size);

            DrawBox(ref containerRect);
            DrawTitle(ref containerRect);

            EditorGUI.BeginChangeCheck();
            DrawStateIdRepresentationField(ref containerRect);
            
            if(StateIdRepresentation == StateIdRepresentation.Enum)
            {
                DrawEnumTypeField(ref containerRect);
            }

            if(EditorGUI.EndChangeCheck())
            {
                SetStateIdTypeByRepresentation();
                OnStateIdTypeChanged?.Invoke(StateIdType);
            }
        }

        private void SetStateIdTypeByRepresentation()
        {
            if (StateIdRepresentation == StateIdRepresentation.Int)
                StateIdType = typeof(int);
            else if (StateIdRepresentation == StateIdRepresentation.Float)
                StateIdType = typeof(float);
            else if (StateIdRepresentation == StateIdRepresentation.String)
                StateIdType = typeof(string);
            else if (StateIdRepresentation == StateIdRepresentation.Enum)
            {
                if (string.IsNullOrEmpty(_enumTypeFullName) == false)
                {
                    StateIdType = Type.GetType(_enumTypeFullName);
                }
                else
                {
                    StateIdType = null;
                }
            }
        }

        private void DrawStateIdRepresentationField(ref Rect containerRect)
        {
            StateIdRepresentation = (StateIdRepresentation)EditorGUI.EnumPopup(
                GetRect(ref containerRect, _style, StateIdRepresentationFieldLeftPadding, 
                StateIdRepresentationFieldTopPadding, StateIdRepresentationFieldSizeY), StateIdRepresentation);
        }

        private void DrawBox(ref Rect containerRect)
        {
            GUI.Box(containerRect, string.Empty, _style);
        }

        private void DrawTitle(ref Rect containerRect)
        {
            GUI.Label(GetRect(ref containerRect, _style, TitleLeftPadding, TitleTopPadding, TitleSizeY), "Builder Settings");
        }

        private void DrawEnumTypeField(ref Rect containerRect)
        {
            GUI.Label(GetRect(ref containerRect, _style, EnumTypeFieldTitleLeftPadding, EnumTypeFieldTitleTopPadding, EnumTypeFieldTitleSizeY), "Enum Type Full Name");
            _enumTypeFullName = EditorGUI.TextField(GetRect(ref containerRect, _style, EnumTypeFieldLeftPadding, EnumTypeFieldTopPadding, EnumTypeFieldSizeY), _enumTypeFullName);
        }

        private Rect GetRect(ref Rect containerRect, GUIStyle containerStyle, float leftPadding, float topPadding, float sizeY)
        {
            var position = new Vector2(containerRect.position.x + leftPadding, containerRect.position.y + containerStyle.border.top + topPadding);
            var size = new Vector2(containerRect.size.x - (leftPadding * 2), sizeY);

            return new Rect(position, size);
        }
    }
}