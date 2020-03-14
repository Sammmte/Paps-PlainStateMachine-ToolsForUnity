using Paps.StateMachines;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity
{
    public abstract class ScriptableGuardCondition : ScriptableObject, IGuardCondition
    {
        [SerializeField]
        private string _debugName;

        public string DebugName => _debugName;

        public bool IsValid()
        {
            return Validate();
        }

        protected abstract bool Validate();
    }
}