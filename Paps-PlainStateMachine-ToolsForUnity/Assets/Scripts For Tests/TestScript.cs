using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Paps.StateMachines;
using Paps.PlainStateMachine_ToolsForUnity;
using System.Reflection;

namespace Tests
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField]
        private PlainStateMachineBuilder _stateMachineBuilder;

        [SerializeField]
        private TestStateAsset _state1, _state2;

        public void Start()
        {
            
        }
    }

}

