using UnityEngine;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class FloatStateIdDrawer : StateIdDrawer
    {
        public FloatStateIdDrawer(object value) : base(value)
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
                if (float.TryParse(value, out float result))
                    StateId = result;
                else
                {
                    if(string.IsNullOrEmpty(value) == false)
                    {
                        Debug.LogWarning("Value " + value + " is not a Float");
                    }

                    StateId = null;
                }
            }
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType() == typeof(float);
        }
    }
}