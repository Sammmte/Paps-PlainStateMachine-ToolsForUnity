using System;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity
{
    [Serializable]
    internal class StateInfo
    {
        [SerializeField]
        private string _serializedStateId;

        [SerializeField]
        private ScriptableState _stateObject;

        [SerializeField]
        private string _stateIdTypeFullName;

        private Type _stateIdType;

        public Type StateIdType
        {
            get
            {
                if(_stateIdType == null)
                    _stateIdType = PlainStateMachineBuilderHelper.GetTypeOf(_stateIdTypeFullName);

                return _stateIdType;
            }
        }

        private object _stateId;

        public object StateId
        {
            get
            {
                if(_stateId == null)
                    _stateId = PlainStateMachineGenericTypeSerializer.Deserialize(_serializedStateId, StateIdType);

                return _stateId;
            }
        }

        public string SerializedStateId => _serializedStateId;

        public ScriptableState StateObject => _stateObject;

        public StateInfo(object stateId, ScriptableState stateObject)
        {
            _serializedStateId = PlainStateMachineGenericTypeSerializer.Serialize(stateId);

            _stateIdTypeFullName = stateId.GetType().FullName;

            _stateObject = stateObject;
        }

        private StateInfo()
        {

        }
    }
}