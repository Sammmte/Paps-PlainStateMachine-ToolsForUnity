using UnityEngine;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StringStateIdDrawer : StateIdDrawer
    {
        public StringStateIdDrawer(object value) : base(value)
        {

        }

        protected override void DrawValueControl()
        {
            EditorGUI.BeginChangeCheck();

            string value = StateId != null ? StateId.ToString() : "";

            if (StateId == null)
                value = EditorGUILayout.TextField(value);
            else
                value = EditorGUILayout.TextField(value);

            if (EditorGUI.EndChangeCheck())
            {
                if (string.IsNullOrEmpty(value) == false)
                    StateId = value;
                else
                {
                    Debug.LogWarning("String field cannot be empty");
                    StateId = null;
                }
            }
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType() == typeof(string);
        }
    }
}