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

        internal void AddState(object stateId, ScriptableState stateObject)
        {
            if (_states == null)
                _states = new List<StateInfo>();

            if (StateIdType != stateId.GetType())
                throw new InvalidOperationException("Cannot add state due to an invalid state id type");

            if (ContainsState(stateId))
                return;

            _states.Add(new StateInfo() { SerializedStateId = PlainStateMachineGenericTypeSerializer.Serialize(stateId), StateObject = stateObject });
            OnChanged?.Invoke();
        }

        internal string GetSerializedGenericTypeOf(object stateIdOrTrigger)
        {
            return PlainStateMachineGenericTypeSerializer.Serialize(stateIdOrTrigger);
        }

        internal object GetDeserializedGenericTypeOf(string serializedStateIdOrTrigger, Type type)
        {
            return PlainStateMachineGenericTypeSerializer.Deserialize(serializedStateIdOrTrigger, type);
        }

        internal void RemoveState(object stateId)
        {
            if(_states != null)
            {
                string stringStateId = PlainStateMachineGenericTypeSerializer.Serialize(stateId);

                for(int i = 0; i < _states.Count; i++)
                {
                    if (_states[i].SerializedStateId == stringStateId)
                    {
                        _states.RemoveAt(i);
                        OnChanged?.Invoke();
                        return;
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
                    object deserializedId = PlainStateMachineGenericTypeSerializer.Deserialize(_states[i].SerializedStateId, StateIdType);

                    if (object.Equals(deserializedId, stateId))
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
                TState stateId = (TState) PlainStateMachineGenericTypeSerializer.Deserialize(_states[i].SerializedStateId, typeof(TState));
                IState stateObject = (_states[i].StateObject as IState) ?? new EmptyState();

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

        internal void SetMetadata(string key, object value)
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