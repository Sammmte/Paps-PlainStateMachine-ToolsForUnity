using System;
using UnityEditor;
using UnityEngine;
using Paps.StateMachines;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StateAssetField
    {
        private const float LeftPadding = 20, FieldTopPadding = 27, FieldSizeY = 17, LabelTopPadding = 10, LabelSizeY = 17;

        private static readonly Type _scriptableObjectType = typeof(ScriptableObject);

        public ScriptableObject StateAsset { get; private set; }

        public StateAssetField(ScriptableObject stateAsset)
        {
            StateAsset = stateAsset;
        }

        public void Expose(ref Rect containerRect, GUIStyle containerStyle)
        {
            ShowLabel(ref containerRect, containerStyle);

            if (StateAsset is IState == false)
                StateAsset = null;

            EditorGUI.BeginChangeCheck();

            var preValidatedStateAsset = (ScriptableObject)EditorGUI.ObjectField(GetRect(ref containerRect, containerStyle), StateAsset, _scriptableObjectType, false);

            if (EditorGUI.EndChangeCheck())
            {
                if (preValidatedStateAsset is IState)
                {
                    StateAsset = preValidatedStateAsset;
                }
                else
                {
                    Debug.LogWarning("Scriptable object does not implements IState interface");
                }
            }
        }

        private Rect GetRect(ref Rect containerRect, GUIStyle containerStyle)
        {
            var position = new Vector2(containerRect.position.x + LeftPadding, containerRect.position.y + containerStyle.border.top + FieldTopPadding);
            var size = new Vector2(containerRect.size.x - (LeftPadding * 2), FieldSizeY);

            return new Rect(position, size);
        }

        private void ShowLabel(ref Rect containerRect, GUIStyle containerStyle)
        {
            var position = new Vector2(containerRect.position.x + LeftPadding, containerRect.position.y + containerStyle.border.top + LabelTopPadding);
            var size = new Vector2(containerRect.size.x - (LeftPadding * 2), LabelSizeY);

            var rect = new Rect(position, size);

            GUI.Label(rect, "State Asset");
        }
    }
}