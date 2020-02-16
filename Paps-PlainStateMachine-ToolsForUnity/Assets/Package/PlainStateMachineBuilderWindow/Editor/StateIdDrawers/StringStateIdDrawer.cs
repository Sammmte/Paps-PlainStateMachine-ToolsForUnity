using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StringStateIdDrawer : StateIdDrawer
    {
        public StringStateIdDrawer(IStateIdValidator stateIdValidator, object value) : base(value, stateIdValidator)
        {

        }

        public override void Draw()
        {
            
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType() == typeof(string);
        }
    }
}