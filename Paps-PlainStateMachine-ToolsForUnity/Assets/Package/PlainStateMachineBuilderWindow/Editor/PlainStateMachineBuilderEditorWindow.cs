using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Paps.StateMachines.Unity.Editor
{
    public class PlainStateMachineBuilderEditorWindow : EditorWindow
    {
        private List<StateNode> _nodes;

        private Vector2 _offset;
        private Vector2 _drag;

        [MenuItem("Paps/Plain State Machine Builder")]
        private static void OpenWindow()
        {
            var window = GetWindow<PlainStateMachineBuilderEditorWindow>();
            window.Show();
        }

        void Awake()
        {
            Debug.Log("Awake");

            titleContent = new GUIContent("Plain State Machine Builder Window");
            
            _nodes = new List<StateNode>();
        }

        private void OnGUI()
        {
            DrawBackground();

            DrawNodes();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawBackground()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
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
            }
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
            _nodes.Add(StateNodeFactory.Create(mousePosition));
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            _offset += _drag * 0.5f;
            Vector3 newOffset = new Vector3(_offset.x % gridSpacing, _offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
    }
}