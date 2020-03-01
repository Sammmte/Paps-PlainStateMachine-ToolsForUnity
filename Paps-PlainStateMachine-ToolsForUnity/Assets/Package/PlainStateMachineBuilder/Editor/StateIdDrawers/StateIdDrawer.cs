using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public abstract class StateIdDrawer
    {
        public object StateId { get; protected set; }

        public bool HasValue => StateId != null;

        protected StateIdDrawer(object value)
        {
            if (value != null && IsValidType(value))
            {
                StateId = value;
            }
        }

        public void Draw()
        {
            DrawLabel();
            DrawValueControl();
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

        protected abstract void DrawValueControl();

        protected abstract bool IsValidType(object value);
    }
}