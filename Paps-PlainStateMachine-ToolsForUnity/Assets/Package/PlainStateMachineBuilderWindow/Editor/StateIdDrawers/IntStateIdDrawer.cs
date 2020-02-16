using UnityEngine;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class IntStateIdDrawer : StateIdDrawer
    {
        private const float FieldTopPadding = 17;

        public IntStateIdDrawer(IStateIdValidator stateIdValidator, object value) : base(value, stateIdValidator)
        {

        }

        public override void Draw(Rect rect)
        {
            DrawLabel(ref rect);
            DrawField(ref rect);
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType() == typeof(int);
        }

        private void DrawLabel(ref Rect rect)
        {
            GUI.Label(rect, GetLabel());
        }

        private string GetLabel()
        {
            if (StateId == null)
                return "State Id (No Value)";
            else
                return "State Id";
        }

        private void DrawField(ref Rect rect)
        {
            EditorGUI.BeginChangeCheck();

            string value = StateId != null ? StateId.ToString() : "";

            if (StateId == null)
                value = EditorGUI.TextField(GetFieldRect(ref rect), value);
            else
                value = EditorGUI.TextField(GetFieldRect(ref rect), value);

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

        private Rect GetFieldRect(ref Rect rect)
        {
            var position = new Vector2(rect.position.x, rect.position.y + FieldTopPadding);

            return new Rect(position, rect.size);
        }
    }
}