using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    [Serializable]
    internal class PlainStateMachineBuilderMetadata
    {
        [SerializeField]
        public List<StateNodeMetadata> StateNodesMetadata;

        public PlainStateMachineBuilderMetadata()
        {
            StateNodesMetadata = new List<StateNodeMetadata>();
        }
    }
}