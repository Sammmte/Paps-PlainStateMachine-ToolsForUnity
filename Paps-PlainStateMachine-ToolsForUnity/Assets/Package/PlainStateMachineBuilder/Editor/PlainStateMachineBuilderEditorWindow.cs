using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class PlainStateMachineBuilderEditorWindow : EditorWindow
    {
        private List<StateNode> _nodes;

        private BackgroundGridDrawer _gridDrawer;
        private PlainStateMachineBuilderSettingsDrawer _builderSettingsDrawer;
        private WindowEventHandler _windowEventHandler;
        private StateNodeEventHandler _nodeEventHandler;

        private PlainStateMachineBuilderMetadata _metadata;
        private const string MetadataKey = "PLAIN_STATE_MACHINE_BUILDER_METADATA";

        private static readonly Type DefaultStateIdType = typeof(int);
        private static readonly Type DefaultTriggerType = typeof(int);

        private PlainStateMachineBuilder _builder;

        public static void OpenWindow(PlainStateMachineBuilder builder)
        {
            var window = GetWindow<PlainStateMachineBuilderEditorWindow>();
            window.Initialize(builder);
            window.Show();
        }

        private void Initialize(PlainStateMachineBuilder builder)
        {
            _builder = builder;

            titleContent = new GUIContent("Plain State Machine Builder Window");

            _nodes = new List<StateNode>();

            _gridDrawer = new BackgroundGridDrawer();
            _windowEventHandler = new WindowEventHandler(this);
            _nodeEventHandler = new StateNodeEventHandler(this);
            _metadata = new PlainStateMachineBuilderMetadata();

            LoadBuilder(_builder);

            _builderSettingsDrawer = new PlainStateMachineBuilderSettingsDrawer(_builder);
            _builderSettingsDrawer.OnStateIdTypeChanged += OnStateIdTypeChanged;
            _builderSettingsDrawer.OnTriggerTypeChanged += OnTriggerTypeChanged;
        }


        
        private void LoadBuilder(PlainStateMachineBuilder builder)
        {
            if (TryLoadFromBuilderData(builder) == false)
            {
                SetBuilderDefaults(builder);
            }
        }

        private bool TryLoadFromBuilderData(PlainStateMachineBuilder builder)
        {
            if(builder.StateIdType != null)
            {
                var states = builder.GetStates();

                if (states != null)
                {
                    _metadata = builder.GetMetadata<PlainStateMachineBuilderMetadata>(MetadataKey);

                    for (int i = 0; i < states.Length; i++)
                    {
                        string serializedStateIdOfCurrent = builder.GetSerializedGenericTypeOf(states[i].SerializedStateId);

                        for (int j = 0; j < _metadata.StateNodesMetadata.Count; j++)
                        {
                            if (_metadata.StateNodesMetadata[j].SerializedStateId == serializedStateIdOfCurrent)
                            {
                                AddNodeWith(states[i], _metadata.StateNodesMetadata[j]);
                                break;
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        private void SetBuilderDefaults(PlainStateMachineBuilder builder)
        {
            builder.StateIdType = DefaultStateIdType;
            builder.TriggerType = DefaultTriggerType;
        }

        private void OnGUI()
        {
            DrawBackground();
            DrawNodes();
            DrawBuilderSettings();

            ProcessNodeEvents(Event.current);
            ProcessWindowEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawBackground()
        {
            _gridDrawer.Draw(position);
        }

        private void DrawBuilderSettings()
        {
            _builderSettingsDrawer.Draw(position);
        }

        private void DrawNodes()
        {
            if (_nodes.Count > 0)
            {
                for (int i = _nodes.Count - 1; i >= 0; i--)
                {
                    _nodes[i].Draw();
                }
            }
        }

        internal void Drag(Vector2 delta)
        {
            if (_nodes != null)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].Drag(delta);
                }
            }

            GUI.changed = true;
        }

        private void AddNodeWith(StateInfo stateInfo, StateNodeMetadata metadata)
        {
            var newNode = new StateNode(metadata.Position, stateInfo.SerializedStateId.GetType(), stateInfo.StateObject, stateInfo.SerializedStateId);
            newNode.OnStateIdChanged += ReplaceStateId;
            newNode.OnStateObjectChanged += ReplaceStateObject;
            newNode.OnPositionChanged += UpdatePositionMetadata;
            _nodes.Add(newNode);
        }

        internal void AddNode(Vector2 mousePosition, ScriptableState stateObject = null)
        {
            var newNode = new StateNode(mousePosition, _builderSettingsDrawer.StateIdType, stateObject);
            newNode.OnStateIdChanged += ReplaceStateId;
            newNode.OnStateObjectChanged += ReplaceStateObject;
            newNode.OnPositionChanged += UpdatePositionMetadata;
            _nodes.Add(newNode);
        }

        internal void RemoveNode(StateNode node)
        {
            node.OnStateIdChanged -= ReplaceStateId;
            node.OnStateObjectChanged -= ReplaceStateObject;
            node.OnPositionChanged -= UpdatePositionMetadata;
            _nodes.Remove(node);

            if(node.StateId != null)
                _builder.RemoveState(node.StateId);

            RebuildMetadata();
        }

        private void ReplaceStateId(StateNode node, object previousId, object newId)
        {
            if (previousId != null && ThereIsOtherNodeWithId(node, previousId) == false)
                _builder.RemoveState(previousId);

            if (newId != null && _builder.ContainsState(newId) == false)
            {
                _builder.AddState(newId, node.StateObject);
            }

            RebuildMetadata();
        }

        private bool ThereIsOtherNodeWithId(StateNode comparisonNode, object stateId)
        {
            for(int i = 0; i < _nodes.Count; i++)
            {
                var current = _nodes[i];

                Debug.Log("EQUALITY");
                Debug.Log(current.StateId);
                Debug.Log(stateId);

                if (current != comparisonNode && object.Equals(current.StateId, stateId))
                    return true;
            }

            return false;
        }

        private void UpdatePositionMetadata(StateNode node, Vector2 position)
        {
            RebuildMetadata();
        }

        private void ReplaceStateObject(StateNode node, ScriptableState previousObj, ScriptableState newObj)
        {
            _builder.RemoveState(node.StateId);
            _builder.AddState(node.StateId, newObj);

            RebuildMetadata();
        }

        private void RebuildMetadata()
        {
            _metadata.StateNodesMetadata.Clear();

            _builder.RemoveMetadata(MetadataKey);

            var states = _builder.GetStates();

            if (states == null) return;

            for(int i = 0; i < states.Length; i++)
            {
                StateNode currentStateNode = GetNodeOf(states[i]);

                _metadata.StateNodesMetadata.Add(new StateNodeMetadata() { SerializedStateId = states[i].SerializedStateId, Position = currentStateNode.GetRect().position });
            }

            _builder.SetMetadata(MetadataKey, _metadata);
        }

        private StateNode GetNodeOf(StateInfo stateInfo)
        {
            for(int i = 0; i < _nodes.Count; i++)
            {
                var current = _nodes[i];

                var serializedId = _builder.GetSerializedGenericTypeOf(current.StateId);

                if (serializedId == stateInfo.SerializedStateId)
                    return current;
            }

            return null;
        }

        private void OnStateIdTypeChanged(Type newType)
        {
            _builder.RemoveAllStates();

            _builder.StateIdType = newType;

            for(int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].SetNewStateIdType(newType);
            }

            RebuildMetadata();
        }

        private void OnTriggerTypeChanged(Type newType)
        {
            _builder.TriggerType = newType;

            RebuildMetadata();
        }

        private void ProcessWindowEvents(Event ev)
        {
            _windowEventHandler.HandleEvent(ev);
        }

        private void ProcessNodeEvents(Event e)
        {
            if (_nodes.Count > 0)
            {
                for (int i = _nodes.Count - 1; i >= 0; i--)
                {
                    var currentNode = _nodes[i];
                    
                    _nodeEventHandler.HandleEventFor(currentNode, e);
                }
            }
        }

        internal void SelectNode(StateNode node)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                var currentNode = _nodes[i];

                if (currentNode == node)
                    currentNode.Select();
                else
                    currentNode.Deselect();
            }
        }

        internal void DeselectNode(StateNode node)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                var currentNode = _nodes[i];

                if (currentNode == node)
                    currentNode.Deselect();
            }
        }
    }
}