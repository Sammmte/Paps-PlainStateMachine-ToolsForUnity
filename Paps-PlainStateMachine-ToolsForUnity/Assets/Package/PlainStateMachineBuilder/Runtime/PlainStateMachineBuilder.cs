using UnityEngine;
using System;
using Paps.StateMachines;
using System.Reflection;
using System.Collections.Generic;

namespace Paps.PlainStateMachine_ToolsForUnity
{
    [CreateAssetMenu(menuName = "Paps/State Machine Builders/Plain State Machine Builder")]
    public class PlainStateMachineBuilder : ScriptableObject
    {
        internal event Action OnChanged;

        internal Type StateIdType
        {
            get
            {
                if (_stateIdType == null)
                    _stateIdType = GetTypeOf(_stateIdTypeFullName);

                return _stateIdType;
            }

            set
            {
                _stateIdType = value;
                _stateIdTypeFullName = _stateIdType.FullName;
                
                OnChanged?.Invoke();
            }
        }

        internal Type TriggerType
        {
            get
            {
                if (_triggerType == null)
                    _triggerType = GetTypeOf(_triggerTypeFullName);

                return _triggerType;
            }

            set
            {
                _triggerType = value;
                _triggerTypeFullName = _triggerType.FullName;
                
                OnChanged?.Invoke();
            }
        }
        
        [SerializeField]
        [HideInInspector]
        private List<StateInfo> _states;

        [SerializeField]
        [HideInInspector]
        private string _stateIdTypeFullName, _triggerTypeFullName;

        [SerializeField]
        [HideInInspector]
        private List<Metadata> _metadata;

        private Type _stateIdType;
        private Type _triggerType;

        internal void AddState(object stateId, ScriptableState stateObject)
        {
            if (_states == null)
                _states = new List<StateInfo>();

            _states.Add(new StateInfo() { StateId = PlainStateMachineGenericTypeSerializer.Serialize(stateId), StateObject = stateObject });
            OnChanged?.Invoke();
        }

        internal void RemoveState(object stateId)
        {
            if(_states != null)
            {
                string stringStateId = PlainStateMachineGenericTypeSerializer.Serialize(stateId);

                for(int i = 0; i < _states.Count; i++)
                {
                    if (_states[i].StateId == stringStateId)
                    {
                        _states.RemoveAt(i);
                        OnChanged?.Invoke();
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

        private PlainStateMachine<TState, TTrigger> Build<TState, TTrigger>()
        {
            var stateMachine = new PlainStateMachine<TState, TTrigger>();

            for(int i = 0; i < _states.Count; i++)
            {
                TState stateId = (TState) PlainStateMachineGenericTypeSerializer.Deserialize(_states[i].StateId, typeof(TState));
                IState stateObject = (IState)_states[i].StateObject;

                stateMachine.AddState(stateId, stateObject);
            }

            return stateMachine;
        }

        public object Build()
        {
            return GetType()
                .GetMethod("Build")
                .MakeGenericMethod(StateIdType, TriggerType)
                .Invoke(this, null);
        }

        internal void SetMetadata(string key, string value)
        {
            if (ContainsKey(key) == false)
                _metadata.Add(new Metadata() { Key = key, Value = value });
            else
            {
                for (int i = 0; i < _metadata.Count; i++)
                {
                    if (_metadata[i].Key == key)
                        _metadata[i].Value = value;
                }
            }
        }

        internal void RemoveMetadata(string key)
        {
            if(ContainsKey(key))
            {
                for (int i = 0; i < _metadata.Count; i++)
                {
                    if (_metadata[i].Key == key)
                        _metadata.RemoveAt(i);
                }
            }
        }

        internal string GetMetadata(string key)
        {
            for(int i = 0; i < _metadata.Count; i++)
            {
                if (_metadata[i].Key == key)
                    return _metadata[i].Value;
            }

            return string.Empty;
        }

        internal bool ContainsKey(string key)
        {
            for(int i = 0; i < _metadata.Count; i++)
            {
                if (_metadata[i].Key == key)
                    return true;
            }

            return false;
        }

        [Serializable]
        private struct StateInfo
        {
            [SerializeField]
            public string StateId;

            [SerializeField]
            public ScriptableState StateObject;
        }

        [Serializable]
        private class Metadata
        {
            [SerializeField]
            public string Key, Value;
        }
    }
}