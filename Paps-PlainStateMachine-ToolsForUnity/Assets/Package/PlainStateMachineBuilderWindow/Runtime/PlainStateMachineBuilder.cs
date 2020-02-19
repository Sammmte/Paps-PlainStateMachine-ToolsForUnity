using UnityEngine;
using System;
using Paps.StateMachines;
using UnityObject = UnityEngine.Object;
using System.Reflection;
using System.Collections.Generic;

namespace Paps.PlainStateMachine_ToolsForUnity
{
    [CreateAssetMenu(menuName = "Paps/State Machine Builders/Plain State Machine Builder")]
    public class PlainStateMachineBuilder : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private List<StateInfo> _states;

        [SerializeField]
        [HideInInspector]
        private ParserMethodInfo _stateIdParserMethodInfo, _triggerParserMethodInfo;

        [SerializeField]
        [HideInInspector]
        private string _stateIdTypeFullName, _triggerTypeFullName;

        private Type _stateIdType;
        private Type _triggerType;

        public Type StateIdType
        {
            get
            {
                if(_stateIdType == null || _stateIdType.FullName != _stateIdTypeFullName)
                {
                    _stateIdType = GetTypeOf(_stateIdTypeFullName);
                }

                return _stateIdType;
            }
        }

        public Type TriggerType
        {
            get
            {
                if(_triggerType == null || _triggerType.FullName != _triggerTypeFullName)
                {
                    _triggerType = GetTypeOf(_triggerTypeFullName);
                }

                return _triggerType;
            }
        }

        public event Action OnChanged;

        public void SetStateIdTypeWithParser(Type type, MethodInfo staticParserMethod)
        {
            if (staticParserMethod.IsStatic == false)
                throw new ArgumentException("Parser method must be static");

            _stateIdTypeFullName = type.FullName;

            _stateIdParserMethodInfo = new ParserMethodInfo(staticParserMethod.DeclaringType.FullName, staticParserMethod.Name);
        }

        public void SetTriggerTypeWithParser(Type type, MethodInfo staticParserMethod)
        {
            if (staticParserMethod.IsStatic == false)
                throw new ArgumentException("Parser method must be static");

            _triggerTypeFullName = type.FullName;

            _triggerParserMethodInfo = new ParserMethodInfo(staticParserMethod.DeclaringType.FullName, staticParserMethod.Name);
        }

        public void AddState<T>(object stateId, T stateObject) where T : UnityObject, IState
        {
            if (_states == null)
                _states = new List<StateInfo>();

            _states.Add(new StateInfo() { StateId = stateId.ToString(), StateObject = stateObject });
        }

        public void RemoveState(object stateId)
        {
            if(_states != null)
            {
                string stringStateId = stateId.ToString();

                for(int i = 0; i < _states.Count; i++)
                {
                    if (_states[i].StateId == stringStateId)
                    {
                        _states.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        private Type GetTypeOf(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);

                if (type != null)
                    return type;
            }

            return null;
        }

        public PlainStateMachine<TState, TTrigger> Build<TState, TTrigger>()
        {
            var stateMachine = new PlainStateMachine<TState, TTrigger>();

            var stateIdParser = GetTypeOf(_stateIdParserMethodInfo.TypeFullName).GetMethod(_stateIdParserMethodInfo.MethodName, new Type[] { typeof(string) });

            for(int i = 0; i < _states.Count; i++)
            {
                TState stateId = (TState)stateIdParser.Invoke(null, new object[] { _states[i].StateId });
                IState stateObject = (IState)_states[i].StateObject;

                stateMachine.AddState(stateId, stateObject);
            }

            return stateMachine;
        }

        [Serializable]
        private struct StateInfo
        {
            [SerializeField]
            public string StateId;

            [SerializeField]
            public UnityObject StateObject;
        }

        [Serializable]
        private struct ParserMethodInfo
        {
            [SerializeField]
            public string TypeFullName;

            [SerializeField]
            public string MethodName;

            public ParserMethodInfo(string typeFullName, string methodName)
            {
                TypeFullName = typeFullName;
                MethodName = methodName;
            }
        }
    }
}