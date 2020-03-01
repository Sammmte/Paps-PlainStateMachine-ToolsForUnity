using System;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity
{
    [Serializable]
    public struct StateInfo
    {
        [SerializeField]
        public string SerializedStateId;

        [SerializeField]
        public ScriptableState StateObject;
    }
}