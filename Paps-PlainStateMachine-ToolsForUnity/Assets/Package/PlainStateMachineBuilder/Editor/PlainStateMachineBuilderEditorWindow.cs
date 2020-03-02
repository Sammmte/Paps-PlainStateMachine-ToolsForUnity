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

            LoadBuilder();

            _builderSettingsDrawer = new PlainStateMachineBuilderSettingsDrawer(_builder);
            _builderSettingsDrawer.OnStateIdTypeChanged += OnStateIdTypeChanged;
            _builderSettingsDrawer.OnTriggerTypeChanged += OnTriggerTypeChanged;

            Undo.undoRedoPerformed += Reload;
        }
        
        private void LoadBuilder()
        {
            if (TryLoadFromBuilderData() == false)
            {
                SetBuilderDefaults();
            }
        }

        private bool TryLoadFromBuilderData()
        {
            if(_builder.StateIdType != null)
            {
                var states = _builder.GetStates();

                if (states != null)
                {
                    _metadata = _builder.GetMetadata<PlainStateMachineBuilderMetadata>(MetadataKey);

                    for (int i = 0; i < states.Length; i++)
                    {
                        var current = states[i];

                        for(int j = 0; j < _metadata.StateNodesMetadata.Count; j++)
                        {
                            if(PlainStateMachineBuilderHelper.AreEquals(_metadata.StateNodesMetadata[j].StateId, current.StateId))
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

        private void SetBuilderDefaults()
        {
            _builder.StateIdType = DefaultStateIdType;
            _builder.TriggerType = DefaultTriggerType;

            SetBuilderDirty();
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
            var newNode = new StateNode(metadata.Position, _builder.StateIdType, stateInfo.StateObject, stateInfo.StateId);
            newNode.OnStateIdChanged += ReplaceStateId;
            newNode.OnStateObjectChanged += ReplaceStateObject;
            newNode.OnPositionChanged += UpdateNodePositionMetadata;
            _nodes.Add(newNode);
        }

        internal void AddNode(Vector2 mousePosition, ScriptableState stateObject = null)
        {
            var newNode = new StateNode(mousePosition, _builder.StateIdType, stateObject);
            newNode.OnStateIdChanged += ReplaceStateId;
            newNode.OnStateObjectChanged += ReplaceStateObject;
            newNode.OnPositionChanged += UpdateNodePositionMetadata;
            _nodes.Add(newNode);
        }

        internal void RemoveNode(StateNode node)
        {
            Undo.RecordObject(_builder, _builder.name);

            node.OnStateIdChanged -= ReplaceStateId;
            node.OnStateObjectChanged -= ReplaceStateObject;
            node.OnPositionChanged -= UpdateNodePositionMetadata;
            _nodes.Remove(node);

            if(node.StateId != null)
                _builder.RemoveState(node.StateId);

            RebuildMetadata();

            SetBuilderDirty();
        }

        private void ReplaceStateId(StateNode node, object previousId, object newId)
        {
            Undo.RecordObject(_builder, _builder.name);

            if (previousId != null && ThereIsOtherNodeWithId(node, previousId) == false)
                _builder.RemoveState(previousId);

            if (newId != null && _builder.ContainsState(newId) == false)
            {
                _builder.AddState(newId, node.StateObject);
            }

            RebuildMetadata();

            SetBuilderDirty();
        }

        private bool ThereIsOtherNodeWithId(StateNode comparisonNode, object stateId)
        {
            for(int i = 0; i < _nodes.Count; i++)
            {
                var current = _nodes[i];

                if (current != comparisonNode && object.Equals(current.StateId, stateId))
                    return true;
            }

            return false;
        }

        private void UpdateNodePositionMetadata(StateNode node, Vector2 position)
        {
            Undo.RecordObject(_builder, _builder.name);

            RebuildMetadata();

            SetBuilderDirty();
        }

        private void ReplaceStateObject(StateNode node, ScriptableState previousObj, ScriptableState newObj)
        {
            Undo.RecordObject(_builder, _builder.name);

            _builder.RemoveState(node.StateId);
            _builder.AddState(node.StateId, newObj);

            RebuildMetadata();

            SetBuilderDirty();
        }

        private void RebuildMetadata()
        {
            _metadata.StateNodesMetadata.Clear();

            _builder.RemoveMetadata(MetadataKey);

            var states = _builder.GetStates();

            if (states == null) return;

            for(int i = 0; i < states.Length; i++)
            {
                StateInfo currentState = states[i];
                StateNode currentStateNode = GetNodeOf(currentState);

                _metadata.StateNodesMetadata.Add(new StateNodeMetadata(currentState.StateId, currentStateNode.Position));
            }

            _builder.SaveMetadata(MetadataKey, _metadata);
        }

        private StateNode GetNodeOf(StateInfo stateInfo)
        {
            for(int i = 0; i < _nodes.Count; i++)
            {
                var current = _nodes[i];

                if (PlainStateMachineBuilderHelper.AreEquals(current.StateId, stateInfo.StateId))
                    return current;
            }

            return null;
        }

        private void OnStateIdTypeChanged(Type newType)
        {
            Undo.RecordObject(_builder, _builder.name);

            _builder.RemoveAllStates();

            _builder.StateIdType = newType;

            for(int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].SetNewStateIdType(newType);
            }

            RebuildMetadata();

            SetBuilderDirty();
        }

        private void OnTriggerTypeChanged(Type newType)
        {
            Undo.RecordObject(_builder, _builder.name);

            _builder.TriggerType = newType;

            RebuildMetadata();

            SetBuilderDirty();
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

        private void Reload()
        {
            Undo.undoRedoPerformed -= Reload;

            Initialize(_builder);

            SetBuilderDirty();
        }

        private void SetBuilderDirty()
        {
            EditorUtility.SetDirty(_builder);
        }
    }
}