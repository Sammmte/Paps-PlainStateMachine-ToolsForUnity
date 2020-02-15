using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public abstract class StateIdDrawer
    {
        public abstract void Draw(Rect containerRect, GUIStyle containerStyle, object value);
    }
}