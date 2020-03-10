using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class PlainStateMachineBuilderEditorWindow : EditorWindow
    {
        private const string MetadataKey = "PLAIN_STATE_MACHINE_BUILDER_METADATA";

        private static readonly Type DefaultStateIdType = typeof(int);
        private static readonly Type DefaultTriggerType = typeof(int);

        private List<StateNode> _nodes;
        private List<TransitionConnection> _transitions;
        private BackgroundGridDrawer _gridDrawer;
        private PlainStateMachineBuilderSettingsDrawer _builderSettingsDrawer;
        private WindowEventHandler _windowEventHandler;
        private StateNodeEventHandler _nodeEventHandler;
        private TransitionConnectionEventHandler _transitionConnectionEventHandler;
        private PlainStateMachineBuilderMetadata _metadata;
        private PlainStateMachineBuilder _builder;
        private InspectorDrawer _inspectorDrawer;

        private StateNode _selectedNode;
        private StateNode _initialNode;

        private TransitionConnection _selectedTransition;

        private TransitionConnectionPreview _transitionPreview;

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
            _transitions = new List<TransitionConnection>();

            _gridDrawer = new BackgroundGridDrawer();
            _windowEventHandler = new WindowEventHandler(this);
            _nodeEventHandler = new StateNodeEventHandler(this);
            _transitionConnectionEventHandler = new TransitionConnectionEventHandler(this);
            _metadata = new PlainStateMachineBuilderMetadata();
            _inspectorDrawer = new InspectorDrawer();

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
            if (_builder.StateIdType != null)
            {
                LoadStates();

                return true;
            }

            return false;
        }

        private void LoadStates()
        {
            var states = _builder.GetStates();

            if (states != null)
            {
                _metadata = _builder.GetMetadata<PlainStateMachineBuilderMetadata>(MetadataKey);

                var initialStateId = _builder.GetInitialStateId();

                for (int i = 0; i < states.Length; i++)
                {
                    var current = states[i];

                    for (int j = 0; j < _metadata.StateNodesMetadata.Count; j++)
                    {
                        if (PlainStateMachineBuilderHelper.AreEquals(_metadata.StateNodesMetadata[j].StateId, current.StateId))
                        {
                            AddNodeFrom(states[i], _metadata.StateNodesMetadata[j]);
                            break;
                        }
                    }
                }

                var initialNode = StateNodeOf(initialStateId);

                if (initialNode != null)
                    SetInitialStateNode(initialNode);
            }
        }

        private void LoadTransitions()
        {
            var transitions = _builder.GetTransitions();

            if (transitions != null)
            {
                for (int i = 0; i < transitions.Length; i++)
                {
                    AddTransitionFrom(transitions[i]);
                }
            }
        }

        private void SetBuilderDefaults()
        {
            _builder.StateIdType = DefaultStateIdType;
            _builder.TriggerType = DefaultTriggerType;

            EditorUtility.SetDirty(_builder);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            DrawBackground();
            DrawTransitions();
            DrawTransitionPreview();
            DrawNodes();
            DrawBuilderSettings();

            if (HasSelectedNode())
                DrawInspector(_selectedNode.DrawControls);

            if (HasSelectedTransition())
                DrawInspector(_selectedTransition.DrawControls);

            ProcessNodeEvents(Event.current);
            ProcessTransitionEvents(Event.current);
            ProcessWindowEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawTransitionPreview()
        {
            if (_transitionPreview != null)
            {
                _transitionPreview.Draw(Event.current.mousePosition);
                GUI.changed = true;
            }
        }

        private void DrawInspector(Action drawSelectedElementControls)
        {
            _inspectorDrawer.Draw(position, drawSelectedElementControls);
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
                    var current = _nodes[i];

                    current.Draw(IsSelected(current), IsInitial(current));
                }
            }
        }

        private void DrawTransitions()
        {
            if (_transitions.Count > 0)
            {
                for (int i = 0; i < _transitions.Count; i++)
                {
                    var current = _transitions[i];

                    current.Draw(IsSelected(current));
                }
            }
        }

        public void Drag(Vector2 delta)
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

        private StateNode StateNodeOf(object stateId)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                var current = _nodes[i];

                if (PlainStateMachineBuilderHelper.AreEquals(current.StateId, stateId))
                    return current;
            }

            return null;
        }

        private void AddNodeFrom(StateInfo stateInfo, StateNodeMetadata metadata)
        {
            var newNode = new StateNode(metadata.Position, _builder.StateIdType, stateInfo.StateObject, stateInfo.StateId);
            InternalAddNode(newNode);
        }

        public void AddNode(Vector2 mousePosition, ScriptableState stateObject = null)
        {
            var newNode = new StateNode(mousePosition, _builder.StateIdType, stateObject);
            InternalAddNode(newNode);
        }

        private void InternalAddNode(StateNode node)
        {
            node.OnStateIdChanged += (changedNode, previousId, currentId) => Rebuild();
            node.OnStateObjectChanged += (changedNode, previousStateObj, currentStateObj) => Rebuild();
            node.OnPositionChanged += UpdateNodePositionMetadata;
            
            _nodes.Add(node);

            if (_nodes.Count == 1)
                SetInitialStateNode(node);
        }

        public void AddTransition(StateNode source, StateNode target)
        {
            if(ContainsTransitionWithSourceAndTarget(source, target))
                return;
            
            var newTransition = new TransitionConnection(source, target, _builder.TriggerType);

            newTransition.OnTriggerChanged += (connection, previousTrigger, currentTrigger) => Rebuild();
            newTransition.OnGuardConditionsChanged += (connection, currentGuardConditions) => Rebuild();

            _transitions.Add(newTransition);
        }

        public void AddTransitionFrom(TransitionInfo transitionInfo)
        {
            var source = StateNodeOf(transitionInfo.StateFrom);
            var target = StateNodeOf(transitionInfo.StateTo);
            
            var newTransition = new TransitionConnection(source, target, _builder.TriggerType, transitionInfo.Trigger, transitionInfo.GuardConditions);

            newTransition.OnTriggerChanged += (connection, previousTrigger, currentTrigger) => Rebuild();
            newTransition.OnGuardConditionsChanged += (connection, currentGuardConditions) => Rebuild();
            
            _transitions.Add(newTransition);
        }

        private bool ContainsTransitionWithSourceAndTarget(StateNode source, StateNode target)
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                if (_transitions[i].Source == source && _transitions[i].Target == target)
                    return true;
            }

            return false;
        }

        public void RemoveNode(StateNode node)
        {
            DoWithUndoAndDirtyFlag(() =>
            {
                _nodes.Remove(node);

                Rebuild();
            });
        }

        public void RemoveTransition(TransitionConnection transition)
        {
            DoWithUndoAndDirtyFlag(() =>
            {
                _transitions.Remove(transition);
                
                Rebuild();
            });
        }

        private void UpdateNodePositionMetadata(StateNode node, Vector2 position)
        {
            DoWithUndoAndDirtyFlag(() =>
            {
                RebuildMetadata();
            });
        }

        private void RebuildMetadata()
        {
            _metadata.StateNodesMetadata.Clear();

            _builder.RemoveMetadata(MetadataKey);

            var states = _builder.GetStates();

            if (states == null) return;

            for (int i = 0; i < states.Length; i++)
            {
                StateInfo currentState = states[i];
                StateNode currentStateNode = GetNodeOf(currentState);

                _metadata.StateNodesMetadata.Add(new StateNodeMetadata(currentState.StateId, currentStateNode.Position));
            }

            _builder.SaveMetadata(MetadataKey, _metadata);
        }

        private StateNode GetNodeOf(StateInfo stateInfo)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                var current = _nodes[i];

                if (PlainStateMachineBuilderHelper.AreEquals(current.StateId, stateInfo.StateId))
                    return current;
            }

            return null;
        }

        private void OnStateIdTypeChanged(Type newType)
        {
            DoWithUndoAndDirtyFlag(() =>
            {
                _builder.RemoveAllStates();

                _builder.StateIdType = newType;

                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].SetNewStateIdType(newType);
                }

                RebuildMetadata();
            });
        }

        private void OnTriggerTypeChanged(Type newType)
        {
            DoWithUndoAndDirtyFlag(() =>
            {
                _builder.TriggerType = newType;

                for(int i = 0; i < _transitions.Count; i++)
                {
                    _transitions[i].SetNewTriggerType(newType);
                }

                RebuildMetadata();
            });
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

        private void ProcessTransitionEvents(Event e)
        {
            if(_transitions.Count > 0)
            {
                for(int i = 0; i < _transitions.Count; i++)
                {
                    var current = _transitions[i];

                    _transitionConnectionEventHandler.HandleEventFor(current, e);
                }
            }
        }

        public void SelectNode(StateNode node)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                var currentNode = _nodes[i];

                if (currentNode == node)
                {
                    GUI.FocusControl(null);
                    _selectedNode = currentNode;
                }

            }

            Repaint();
        }

        public void DeselectAllNodes()
        {
            _selectedNode = null;

            Repaint();
        }

        public void SelectTransition(TransitionConnection transition)
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                var currentTransition = _transitions[i];

                if (currentTransition == transition)
                {
                    GUI.FocusControl(null);
                    _selectedTransition = currentTransition;
                }
            }

            Repaint();
        }

        public void DeselectAllTransitions()
        {
            _selectedTransition = null;

            Repaint();
        }

        public void SetInitialStateNode(StateNode node)
        {
            _initialNode = node;
            _builder.SetInitialState(_initialNode.StateId);
        }

        private void Reload()
        {
            Undo.undoRedoPerformed -= Reload;

            Initialize(_builder);

            EditorUtility.SetDirty(_builder);
            Repaint();
        }

        private void DoWithUndoAndDirtyFlag(Action action)
        {
            Undo.RecordObject(_builder, _builder.name);
            action();
            EditorUtility.SetDirty(_builder);
        }

        public bool IsSelected(StateNode node)
        {
            return _selectedNode == node;
        }

        public bool IsSelected(TransitionConnection transition)
        {
            return _selectedTransition == transition;
        }

        public bool IsInitial(StateNode node)
        {
            return _initialNode == node;
        }

        public bool HasSelectedNode()
        {
            return _selectedNode != null;
        }

        public bool HasSelectedTransition()
        {
            return _selectedTransition != null;
        }

        public void BeginTransitionPreviewFrom(StateNode source)
        {
            _transitionPreview = new TransitionConnectionPreview(source);
        }

        public void EndTransitionPreview()
        {
            _transitionPreview = null;
        }

        public bool HasTransitionPreview()
        {
            return _transitionPreview != null;
        }

        public StateNode GetSourceNodeFromTransitionPreview()
        {
            if (HasTransitionPreview())
                return _transitionPreview.Source;
            else 
                return null;
        }

        private void Rebuild()
        {
            ClearBuilder();
            RebuildStates();
            RebuildTransitions();
            RebuildMetadata();
        }
        
        private void ClearBuilder()
        {
            _builder.RemoveAllTransitions();
            _builder.RemoveAllStates();
        }

        private void RebuildStates()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _builder.AddState(_nodes[i].StateId, _nodes[i].StateObject);
                
                if(IsInitial(_nodes[i]))
                    SetInitialStateNode(_nodes[i]);
            }
        }

        private void RebuildTransitions()
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                _builder.AddTransition(_transitions[i].StateFrom, _transitions[i].Trigger, _transitions[i].StateTo,
                    _transitions[i].GuardConditions);
            }
        }
    }
}