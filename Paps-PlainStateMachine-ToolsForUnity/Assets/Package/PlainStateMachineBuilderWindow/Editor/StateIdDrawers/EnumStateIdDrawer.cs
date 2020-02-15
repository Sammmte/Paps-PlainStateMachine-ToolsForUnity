using UnityEngine;
using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class EnumStateIdDrawer : StateIdDrawer
    {
        private Type _enumType;

        public EnumStateIdDrawer(Type enumType)
        {
            _enumType = enumType;
        }

        public override void Draw(Rect containerRect, GUIStyle containerStyle, object value)
        {
            
        }
    }
}