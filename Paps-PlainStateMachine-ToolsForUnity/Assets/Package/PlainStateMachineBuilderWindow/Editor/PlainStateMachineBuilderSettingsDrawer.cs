using UnityEngine;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class PlainStateMachineBuilderSettingsDrawer
    {
        private const float Width = 300, Height = 400, RightPadding = 10, TopPadding = 10;
        private const float StateIdRepresentationFieldLeftPadding = 20, StateIdRepresentationFieldTopPadding = 40, StateIdRepresentationFieldSizeY = 17;
        private const float TitleLeftPadding = 20, TitleTopPadding = 20, TitleSizeY = 17;

        public StateIdRepresentation StateIdRepresentation { get; private set; }

        private static readonly Texture2D _backgroundTexture = Resources.Load<Texture2D>("Paps/PlainStateMachine-ToolsForUnity/Textures/node_normal");

        private GUIStyle _style;

        public PlainStateMachineBuilderSettingsDrawer()
        {
            _style = new GUIStyle();
            _style.normal.background = _backgroundTexture;
        }

        public void Draw(Rect windowRect)
        {
            var position = new Vector2(windowRect.size.x - RightPadding - Width, TopPadding);
            var size = new Vector2(Width, Height);

            var containerRect = new Rect(position, size);

            DrawBox(ref containerRect);
            DrawTitle(ref containerRect);
            DrawStateIdRepresentationField(ref containerRect);
        }

        private void DrawStateIdRepresentationField(ref Rect containerRect)
        {
            StateIdRepresentation = (StateIdRepresentation)EditorGUI.EnumPopup(GetStateIdRepresentationRect(ref containerRect, _style), StateIdRepresentation);
        }

        private void DrawBox(ref Rect containerRect)
        {
            GUI.Box(containerRect, string.Empty, _style);
        }

        private void DrawTitle(ref Rect containerRect)
        {
            GUI.Label(GetTitleRect(ref containerRect, _style), "Builder Settings");
        }

        private Rect GetStateIdRepresentationRect(ref Rect containerRect, GUIStyle containerStyle)
        {
            var position = new Vector2(containerRect.position.x + StateIdRepresentationFieldLeftPadding, containerRect.position.y + containerStyle.border.top + StateIdRepresentationFieldTopPadding);
            var size = new Vector2(containerRect.size.x - (StateIdRepresentationFieldLeftPadding * 2), StateIdRepresentationFieldSizeY);

            return new Rect(position, size);
        }

        private Rect GetTitleRect(ref Rect containerRect, GUIStyle containerStyle)
        {
            var position = new Vector2(containerRect.position.x + TitleLeftPadding, containerRect.position.y + containerStyle.border.top + TitleTopPadding);
            var size = new Vector2(containerRect.size.x - (TitleLeftPadding * 2), TitleSizeY);

            return new Rect(position, size);
        }
    }
}