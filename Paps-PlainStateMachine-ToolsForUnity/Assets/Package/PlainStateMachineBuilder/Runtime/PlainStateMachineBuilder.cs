using UnityEngine;
using System;
using Paps.StateMachines;
using Paps.StateMachines.Extensions;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Paps.PlainStateMachine_ToolsForUnity.Editor")]
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
                    _stateIdType = PlainStateMachineBuilderHelper.GetTypeOf(_stateIdTypeFullName);

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
                    _triggerType = PlainStateMachineBuilderHelper.GetTypeOf(_triggerTypeFullName);

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
        //[HideInInspector]
        private List<StateInfo> _states;

        [SerializeField]
        //[HideInInspector]
        private string _stateIdTypeFullName, _triggerTypeFullName;

        [SerializeField]
        //[HideInInspector]
        private List<Metadata> _metadata;

        private Type _stateIdType;
        private Type _triggerType;

        private void Awake()
        {
            if(_states == null)
                _states = new List<StateInfo>();

            if (_metadata == null)
                _metadata = new List<Metadata>();
        }

        internal void AddState(object stateId, ScriptableState stateObject)
        {
            if (StateIdType != stateId.GetType())
                throw new InvalidOperationException("Cannot add state due to an invalid state id type");

            if (ContainsState(stateId))
                return;

            _states.Add(new StateInfo(stateId, stateObject));
            OnChanged?.Invoke();
        }

        internal StateInfo StateInfoOf(object stateId)
        {
            for(int i = 0; i < _states.Count; i++)
            {
                var current = _states[i];

                if (PlainStateMachineBuilderHelper.AreEquals(stateId, current.StateId))
                    return current;
            }

            return null;
        }

        internal void RemoveState(object stateId)
        {
            if(ContainsState(stateId))
            {
                for(int i = 0; i < _states.Count; i++)
                {
                    var current = _states[i];

                    if(PlainStateMachineBuilderHelper.AreEquals(stateId, current.StateId))
                    {
                        _states.RemoveAt(i);
                        OnChanged?.Invoke();
                    }
                }
            }
        }

        internal bool ContainsState(object stateId)
        {
            if(stateId != null)
            {
                for (int i = 0; i < _states.Count; i++)
                {
                    var current = _states[i];

                    if (PlainStateMachineBuilderHelper.AreEquals(stateId, current.StateId))
                        return true;
                }
            }

            return false;
        }

        internal StateInfo[] GetStates()
        {
            if (_states.Count > 0)
                return _states.ToArray();
            else
                return null;
        }

        internal void RemoveAllStates()
        {
            _states.Clear();

            OnChanged?.Invoke();
        }

        private PlainStateMachine<TState, TTrigger> Build<TState, TTrigger>()
        {
            var stateMachine = new PlainStateMachine<TState, TTrigger>();

            for(int i = 0; i < _states.Count; i++)
            {
                var current = _states[i];

                TState stateId = (TState)current.StateId;
                IState stateObject = (current.StateObject as IState) ?? new EmptyState();

                stateMachine.AddState(stateId, stateObject);
            }

            return stateMachine;
        }

        public object Build()
        {
            return GetType()
                .GetMethod("Build", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(StateIdType, TriggerType)
                .Invoke(this, null);
        }

        internal void SaveMetadata(string key, object value)
        {
            if (ContainsMetadataKey(key) == false)
                _metadata.Add(new Metadata() { Key = key, Value = JsonUtility.ToJson(value) });
            else
            {
                for (int i = 0; i < _metadata.Count; i++)
                {
                    if (_metadata[i].Key == key)
                        _metadata[i].Value = JsonUtility.ToJson(value);
                }
            }
        }

        internal void RemoveMetadata(string key)
        {
            if(ContainsMetadataKey(key))
            {
                for (int i = 0; i < _metadata.Count; i++)
                {
                    if (_metadata[i].Key == key)
                        _metadata.RemoveAt(i);
                }
            }
        }

        internal T GetMetadata<T>(string key)
        {
            for(int i = 0; i < _metadata.Count; i++)
            {
                if (_metadata[i].Key == key)
                    return JsonUtility.FromJson<T>(_metadata[i].Value);
            }

            return default;
        }

        internal bool ContainsMetadataKey(string key)
        {
            for(int i = 0; i < _metadata.Count; i++)
            {
                if (_metadata[i].Key == key)
                    return true;
            }

            return false;
        }

        [Serializable]
        private class Metadata
        {
            [SerializeField]
            public string Key, Value;
        }
    }
}