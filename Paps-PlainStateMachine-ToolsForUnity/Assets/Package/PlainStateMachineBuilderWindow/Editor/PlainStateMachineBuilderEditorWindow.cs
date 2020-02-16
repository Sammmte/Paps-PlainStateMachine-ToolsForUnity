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

        private StateIdValidator _stateIdValidator;

        private Vector2 _drag;

        [MenuItem("Paps/Plain State Machine Builder")]
        private static void OpenWindow()
        {
            var window = GetWindow<PlainStateMachineBuilderEditorWindow>();
            window.Show();
        }

        void Awake()
        {
            titleContent = new GUIContent("Plain State Machine Builder Window");
            
            _nodes = new List<StateNode>();

            _gridDrawer = new BackgroundGridDrawer();
            _builderSettingsDrawer = new PlainStateMachineBuilderSettingsDrawer();

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
                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].Draw();
                }
            }
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:

                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }

                    break;

                case EventType.MouseDrag:

                    _drag = Vector2.zero;

                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }

                    break;
            }
        }

        private void OnDrag(Vector2 delta)
        {
            _drag = delta;

            if (_nodes != null)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].Drag(delta);
                }
            }

            GUI.changed = true;
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

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 mousePosition)
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
    }
}