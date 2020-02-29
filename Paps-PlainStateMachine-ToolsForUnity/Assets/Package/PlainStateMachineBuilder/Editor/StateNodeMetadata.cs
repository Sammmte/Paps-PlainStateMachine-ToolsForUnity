using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    [Serializable]
    public class StateNodeMetadata
    {
        [SerializeField]
        public Vector2 Position;

        [SerializeField]
        public string SerializedStateId;
    }

}