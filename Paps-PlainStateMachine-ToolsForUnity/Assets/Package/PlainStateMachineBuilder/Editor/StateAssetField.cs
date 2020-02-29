using System;
using UnityEditor;
using UnityEngine;
using Paps.StateMachines;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StateAssetField
    {
        private static readonly Type _stateObjectType = typeof(ScriptableState);

        public ScriptableState StateAsset { get; private set; }

        public StateAssetField(ScriptableState stateAsset)
        {
            StateAsset = stateAsset;
        }

        public void Draw()
        {
            if (StateAsset is IState == false)
                StateAsset = null;

            DrawLabel();
            DrawStateAssetField();
        }

        private void DrawStateAssetField()
        {
            EditorGUI.BeginChangeCheck();

            var preValidatedStateAsset = (ScriptableState)EditorGUILayout.ObjectField(StateAsset, _stateObjectType, false);

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

        private void DrawLabel()
        {
            GUILayout.Label("State Asset");
        }
    }
}