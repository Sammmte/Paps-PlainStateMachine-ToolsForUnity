using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class StateIdValidator : IStateIdValidator
    {
        public bool IsValid(object value)
        {
            return true;
        }
    }

}