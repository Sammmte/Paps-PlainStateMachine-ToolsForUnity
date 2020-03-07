using Paps.StateMachines;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity
{
    public abstract class ScriptableGuardCondition : ScriptableObject, IGuardCondition
    {
        public bool IsValid()
        {
            return Validate();
        }

        protected abstract bool Validate();
    }
}