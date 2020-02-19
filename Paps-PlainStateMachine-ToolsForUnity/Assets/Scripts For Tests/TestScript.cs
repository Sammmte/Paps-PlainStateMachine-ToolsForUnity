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
            Func<string, int> parser = int.Parse;

            _stateMachineBuilder = ScriptableObject.CreateInstance<PlainStateMachineBuilder>();

            _stateMachineBuilder.SetStateIdTypeWithParser(typeof(int), parser.Method);
            _stateMachineBuilder.SetTriggerTypeWithParser(typeof(int), parser.Method);

            _stateMachineBuilder.AddState(1, _state1);
            _stateMachineBuilder.AddState(2, _state2);

            _stateMachineBuilder.RemoveState(2);

            var stateMachine = _stateMachineBuilder.Build<int, int>();

            var states = stateMachine.GetStates();

            for (int i = 0; i < states.Length; i++)
            {
                Debug.Log("State Id: " + states[i]);
            }
        }
    }

}

