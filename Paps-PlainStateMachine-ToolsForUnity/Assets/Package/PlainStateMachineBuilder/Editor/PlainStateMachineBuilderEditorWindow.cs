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

        private StateIdValidator _stateIdValidator;

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
            _builderSettingsDrawer = new PlainStateMachineBuilderSettingsDrawer(builder);
            _windowEventHandler = new WindowEventHandler(this);
            _stateIdValidator = new StateIdValidator();

            _builderSettingsDrawer.OnStateIdTypeChanged += OnStateIdTypeChanged;
        }

        private void OnGUI()
        {
            DrawBackground();
            DrawNodes();
            DrawBuilderSettings();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

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

        internal void AddNodeAtPosition(Vector2 mousePosition)
        {
            _nodes.Add(new StateNode(mousePosition, _stateIdValidator, _builderSettingsDrawer.StateIdType));
        }

        private void OnStateIdTypeChanged(Type newType)
        {
            for(int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].SetNewStateIdType(newType);
            }
        }

        private void ProcessEvents(Event ev)
        {
            _windowEventHandler.HandleEvent(ev);
        }

        private void ProcessNodeEvents(Event e)
        {
            if (_nodes.Count > 0)
            {
                for (int i = _nodes.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = _nodes[i].ProcessEvents(e);

                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }
    }
}