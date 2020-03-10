using Paps.PlainStateMachine_ToolsForUnity;
using Paps.StateMachines;
using UnityEngine;
using SomeNamespace;
using System;

namespace Tests
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField]
        private PlainStateMachineBuilder _stateMachineBuilder;

        public void Start()
        {
            var stateMachine = (PlainStateMachine<int, int>)_stateMachineBuilder.Build();

            var states = stateMachine.GetStates();

            Debug.Log("State count: " + stateMachine.StateCount);

            Debug.Log("Initial State: " + stateMachine.InitialState);

            foreach (var state in states)
            {
                Debug.Log(state);

                var eventHandlers = stateMachine.GetEventHandlersOf(state);
                
                if(eventHandlers != null)
                    Debug.Log("State " + state + " contains " + eventHandlers.Length + " event handlers");
            }

            var transitions = stateMachine.GetTransitions();

            foreach (var transition in transitions)
            {
                Debug.Log("Transition: " + transition.StateFrom + " -> " + transition.Trigger + " -> " + transition.StateTo);

                var guardConditions = stateMachine.GetGuardConditionsOf(transition);

                Debug.Log("Guard conditions count: " + guardConditions.Length);
            }
        }
    }

}

