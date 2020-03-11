using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class InspectorDrawer
    {
        private const float Width = 300, Height = 400, LeftPadding = 10, TopPadding = 10;

        private GUIStyle _boxStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _controlsAreaStyle;

        public InspectorDrawer()
        {
            _boxStyle = new GUIStyle();

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.grey);
            texture.Apply();

            _boxStyle.normal.background = texture;

            _titleStyle = new GUIStyle();
            _titleStyle.padding = new RectOffset(20, 20, 20, 20);
            _titleStyle.alignment = TextAnchor.MiddleCenter;
            _titleStyle.fontSize = 20;
            _titleStyle.wordWrap = true;

            _labelStyle = new GUIStyle();
            _labelStyle.wordWrap = true;

            _controlsAreaStyle = new GUIStyle();
            _controlsAreaStyle.padding = new RectOffset(20, 20, 20, 20);
        }

        public void Draw(Rect windowRect, IInspectable inspectable)
        {
            var position = new Vector2(LeftPadding, TopPadding);
            var size = new Vector2(Width, Height);

            var boxRect = new Rect(position, size);

            GUILayout.BeginArea(boxRect, _boxStyle);
            DrawTitle();
            inspectable?.DrawControls();
            GUILayout.EndArea();
        }

        private void DrawTitle()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Element Inspector", _titleStyle);
            GUILayout.EndVertical();
        }
    }
}