using Paps.PlainStateMachine_ToolsForUnity;
using Paps.StateMachines;
using UnityEngine;

namespace Tests
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField]
        private PlainStateMachineBuilder _stateMachineBuilder;

        public void Start()
        {
            PlainStateMachine<int, int> stateMachine = (PlainStateMachine<int, int>)_stateMachineBuilder.Build();

            var states = stateMachine.GetStates();

            Debug.Log("State count: " + stateMachine.StateCount);

            foreach(var state in states)
            {
                Debug.Log(state);
            }
        }
    }

}

