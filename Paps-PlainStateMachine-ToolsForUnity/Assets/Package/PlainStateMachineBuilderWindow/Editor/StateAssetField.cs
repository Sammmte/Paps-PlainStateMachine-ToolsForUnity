using System;
using UnityEditor;
using UnityEngine;
using Paps.StateMachines;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StateAssetField
    {
        private const float FieldTopPadding = 17;

        private static readonly Type _scriptableObjectType = typeof(ScriptableObject);

        public ScriptableObject StateAsset { get; private set; }

        public StateAssetField(ScriptableObject stateAsset)
        {
            StateAsset = stateAsset;
        }

        public void Draw(Rect rect)
        {
            ShowLabel(ref rect);

            if (StateAsset is IState == false)
                StateAsset = null;

            EditorGUI.BeginChangeCheck();

            var preValidatedStateAsset = (ScriptableObject)EditorGUI.ObjectField(GetFieldRect(ref rect), StateAsset, _scriptableObjectType, false);

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

        private void ShowLabel(ref Rect rect)
        {
            GUI.Label(rect, "State Asset");
        }

        private Rect GetFieldRect(ref Rect labelRect)
        {
            var position = new Vector2(labelRect.position.x, labelRect.position.y + FieldTopPadding);

            return new Rect(position, labelRect.size);
        }
    }
}