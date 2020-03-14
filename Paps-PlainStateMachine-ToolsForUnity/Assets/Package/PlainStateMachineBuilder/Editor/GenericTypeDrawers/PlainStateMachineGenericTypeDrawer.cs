using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal abstract class PlainStateMachineGenericTypeDrawer
    {
        public object Value { get; protected set; }

        public bool HasValue => Value != null;

        protected PlainStateMachineGenericTypeDrawer(object value)
        {
            if (value != null && IsValidType(value))
            {
                Value = value;
            }
        }

        public void Draw(string label = "Value")
        {
            DrawLabel(label);
            DrawValueControl();
        }

        private void DrawLabel(string label)
        {
            GUILayout.Label(GetCompleteLabel(label));
        }

        private string GetCompleteLabel(string baseLabel)
        {
            if (Value == null)
                return baseLabel + " (No Value)";
            else
                return baseLabel;
        }

        protected abstract void DrawValueControl();

        protected abstract bool IsValidType(object value);
    }
}