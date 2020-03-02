using UnityEngine;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class IntStateIdDrawer : StateIdDrawer
    {
        public IntStateIdDrawer(object value) : base(value)
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
                if (int.TryParse(value, out int result))
                    StateId = result;
                else
                {
                    if(string.IsNullOrEmpty(value) == false)
                    {
                        Debug.LogWarning("Value " + value + " is not an Integer");
                    }
                    
                    StateId = null;
                }
            }
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType() == typeof(int);
        }
    }
}