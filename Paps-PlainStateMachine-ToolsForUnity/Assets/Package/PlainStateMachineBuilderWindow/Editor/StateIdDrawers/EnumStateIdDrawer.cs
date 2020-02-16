using UnityEngine;
using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class EnumStateIdDrawer : StateIdDrawer
    {
        private Type _enumType;

        public EnumStateIdDrawer(Type enumType, IStateIdValidator stateIdValidator, object value) : base(value, stateIdValidator)
        {
            _enumType = enumType;
        }

        public override void Draw(Rect rect)
        {
            
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType().IsEnum;
        }
    }
}