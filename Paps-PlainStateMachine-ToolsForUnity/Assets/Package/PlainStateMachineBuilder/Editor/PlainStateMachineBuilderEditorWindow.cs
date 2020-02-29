using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class PlainStateMachineBuilderEditorWindow : EditorWindow
    {
        private List<StateNode> _nodes;

        private BackgroundGridDrawer _gridDrawer;
        private PlainStateMachineBuilderSettingsDrawer _builderSettingsDrawer;
        private WindowEventHandler _windowEventHandler;
        private StateNodeEventHandler _nodeEventHandler;

        private StateIdValidator _stateIdValidator;

        private PlainStateMachineBuilderMetadata _metadata;
        private const string MetadataKey = "PLAIN_STATE_MACHINE_BUILDER_METADATA";

        private static readonly Type DefaultStateIdType = typeof(int);
        private static readonly Type DefaultTriggerType = typeof(int);

        [MenuItem("Paps/Plain State Machine Builder")]
        private static void OpenWindow()
        {
            OpenWindow(null);
        }

        public static void OpenWindow(PlainStateMachineBuilder builder)
        {
            var window = GetWindow<PlainStateMachineBuilderEditorWindow>();
            window.Initialize(builder);
            window.Show();
        }

        private void Initialize(PlainStateMachineBuilder builder = null)
        {
            titleContent = new GUIContent("Plain State Machine Builder Window");
            
            _nodes = new List<StateNode>();

            _gridDrawer = new BackgroundGridDrawer();
            
            _windowEventHandler = new WindowEventHandler(this);
            _nodeEventHandler = new StateNodeEventHandler(this);
            _stateIdValidator = new StateIdValidator();
            _metadata = new PlainStateMachineBuilderMetadata();

            if (builder != null)
            {
                LoadBuilder(builder);
            }

            _builderSettingsDrawer = new PlainStateMachineBuilderSettingsDrawer(builder);
            _builderSettingsDrawer.OnStateIdTypeChanged += OnStateIdTypeChanged;
            _builderSettingsDrawer.OnBuilderChanged += OnBuilderChanged;
        }

        private void LoadBuilder(PlainStateMachineBuilder builder)
        {
            if (TryLoadFromBuilderData(builder) == false)
                LoadAsNew(builder);
        }

        private void Unload()
        {
            _nodes.Clear();
        }

        private bool TryLoadFromBuilderData(PlainStateMachineBuilder builder)
        {
            var states = builder.GetStates();

            if(states != null)
            {
                _metadata = builder.GetMetadata<PlainStateMachineBuilderMetadata>(MetadataKey);

                for(int i = 0; i < states.Length; i++)
                {
                    string serializedStateIdOfCurrent = builder.GetSerializedGenericTypeOf(states[i].StateId);

                    for (int j = 0; j < _metadata.StateNodesMetadata.Count; j++)
                    {
                        if (_metadata.StateNodesMetadata[j].SerializedStateId == serializedStateIdOfCurrent)
                        {
                            AddNodeWith(states[i], _metadata.StateNodesMetadata[j]);
                            break;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        private void LoadAsNew(PlainStateMachineBuilder builder)
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
            _nodes.Add(new StateNode(metadata.Position, _stateIdValidator, stateInfo.StateId.GetType(), stateInfo.StateObject, stateInfo.StateId));
        }

        internal void AddNode(Vector2 mousePosition)
        {
            _nodes.Add(new StateNode(mousePosition, _stateIdValidator, _builderSettingsDrawer.StateIdType));
        }

        internal void RemoveNode(StateNode node)
        {
            _nodes.Remove(node);

            if (_builderSettingsDrawer.PlainStateMachineBuilder != null)
                _builderSettingsDrawer.PlainStateMachineBuilder.RemoveState(node.StateId);
        }

        private bool SaveState(StateNode node)
        {
            if (_stateIdValidator.IsValid(node.StateId))
            {
                _builderSettingsDrawer.PlainStateMachineBuilder.AddState(node.StateId, node.StateObject);
                string serializedStateId = _builderSettingsDrawer.PlainStateMachineBuilder.GetSerializedGenericTypeOf(node.StateId);
                var stateNodeMetadata = new StateNodeMetadata() { SerializedStateId = serializedStateId, Position = node.GetRect().position };

                AddOrReplaceStateNodeMetadata(stateNodeMetadata);

                _builderSettingsDrawer.PlainStateMachineBuilder.SetMetadata(MetadataKey, stateNodeMetadata);
                return true;
            }

            return false;
        }

        private void AddOrReplaceStateNodeMetadata(StateNodeMetadata metadata)
        {
            var possiblyAddedMetadata = _metadata.StateNodesMetadata.FirstOrDefault(m => m.SerializedStateId == metadata.SerializedStateId);

            if (possiblyAddedMetadata == null)
                _metadata.StateNodesMetadata.Add(metadata);
            else
                _metadata.StateNodesMetadata[_metadata.StateNodesMetadata.IndexOf(possiblyAddedMetadata)] = metadata;
        }

        private void OnStateIdTypeChanged(Type newType)
        {
            for(int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].SetNewStateIdType(newType);
            }
        }

        private void OnBuilderChanged(PlainStateMachineBuilder builder)
        {
            if (builder != null)
                LoadBuilder(builder);
            else
                Unload();
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