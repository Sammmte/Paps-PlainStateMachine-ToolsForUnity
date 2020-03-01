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
            PlainStateMachine<string, int> stateMachine = (PlainStateMachine<string, int>)_stateMachineBuilder.Build();

            Debug.Log(stateMachine.ContainsState("hola"));
            Debug.Log(stateMachine.ContainsState("loro"));
        }
    }

}

