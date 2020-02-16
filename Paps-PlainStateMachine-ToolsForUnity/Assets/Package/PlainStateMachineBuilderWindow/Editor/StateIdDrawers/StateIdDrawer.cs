using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public abstract class StateIdDrawer
    {
        public object StateId { get; protected set; }
        protected IStateIdValidator stateIdValidator { get; private set; }

        public bool HasValue => StateId != null;

        protected StateIdDrawer(object value, IStateIdValidator stateIdValidator)
        {
            this.stateIdValidator = stateIdValidator;

            if (value != null && IsValidType(value))
            {
                StateId = value;
            }
        }

        public abstract void Draw();

        protected abstract bool IsValidType(object value);
    }
}