using System;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity
{
    [Serializable]
    public struct StateInfo
    {
        [SerializeField]
        public string StateId;

        [SerializeField]
        public ScriptableState StateObject;
    }
}