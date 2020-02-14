using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Paps.StateMachines.Unity.Editor
{
    public static class StateNodeFactory
    {
        private static GUIStyle _nodeStyle;

        private static bool _isInitialized;

        private const float NodeWidth = 200;
        private const float NodeHeight = 100;

        private static void Initialize()
        {
            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            _nodeStyle.border = new RectOffset(12, 12, 12, 12);

            _isInitialized = true;
        }

        public static StateNode Create(Vector2 position)
        {
            if (_isInitialized == false)
                Initialize();

            return new StateNode(position, NodeWidth, NodeHeight, _nodeStyle);
        }
    }
}