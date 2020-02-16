using UnityEngine;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class IntStateIdDrawer : StateIdDrawer
    {
        public IntStateIdDrawer(IStateIdValidator stateIdValidator, object value) : base(value, stateIdValidator)
        {

        }

        public override void Draw()
        {
            DrawLabel();
            DrawField();
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType() == typeof(int);
        }

        private void DrawLabel()
        {
            GUILayout.Label(GetLabel());
        }

        private string GetLabel()
        {
            if (StateId == null)
                return "State Id (No Value)";
            else
                return "State Id";
        }

        private void DrawField()
        {
            EditorGUI.BeginChangeCheck();

            string value = StateId != null ? StateId.ToString() : "";

            if (StateId == null)
                value = EditorGUILayout.TextField(value);
            else
                value = EditorGUILayout.TextField(value);

            if(EditorGUI.EndChangeCheck())
            {
                if (int.TryParse(value, out int result) && stateIdValidator.IsValid(result))
                    StateId = result;
                else
                {
                    Debug.LogWarning("Value " + value + " is not an Integer");
                    StateId = null;
                }
            }
        }
    }
}