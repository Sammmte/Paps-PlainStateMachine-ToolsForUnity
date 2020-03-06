﻿using Paps.StateMachines;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity
{
    public abstract class ScriptableState : ScriptableObject, IState, IStateEventHandler
    {
        [SerializeField]
        private string _debugName;

        public string DebugName => _debugName;

        protected object stateMachine { get; private set; }

        public void Initialize(object stateMachine)
        {
            if (this.stateMachine == null)
                this.stateMachine = stateMachine;
        }
        
        public void Enter()
        {
            OnEnter();
        }

        protected virtual void OnEnter()
        {

        }

        public void Exit()
        {
            OnExit();
        }

        protected virtual void OnExit()
        {

        }

        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {

        }

        public virtual bool HandleEvent(IEvent ev)
        {
            return false;
        }
    }
}