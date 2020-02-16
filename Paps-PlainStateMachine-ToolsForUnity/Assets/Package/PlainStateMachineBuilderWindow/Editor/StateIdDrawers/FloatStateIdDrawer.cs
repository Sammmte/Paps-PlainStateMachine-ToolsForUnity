using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class FloatStateIdDrawer : StateIdDrawer
    {
        public FloatStateIdDrawer(IStateIdValidator stateIdValidator, object value) : base(value, stateIdValidator)
        {

        }

        public override void Draw()
        {
            
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType() == typeof(float);
        }
    }
}