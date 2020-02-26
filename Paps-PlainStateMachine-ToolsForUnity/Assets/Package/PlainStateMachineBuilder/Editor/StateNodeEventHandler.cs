using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class StateNodeEventHandler
    {
        private PlainStateMachineBuilderEditorWindow _window;

        public StateNodeEventHandler(PlainStateMachineBuilderEditorWindow window)
        {
            _window = window;
        }

        public void HandleEventFor(StateNode node, Event nodeEvent)
        {
            switch (nodeEvent.type)
            {
                case EventType.MouseDown:

                    if (IsLeftMouseClick(nodeEvent.button))
                    {
                        if (IsPointerOverNode(nodeEvent.mousePosition, node))
                        {
                            _window.SelectNode(node);
                            nodeEvent.Use();
                        }
                        else
                            _window.DeselectNode(node);
                    }
                    else if (IsRightMouseClick(nodeEvent.button) && node.IsSelected)
                    {
                        DisplayNodeOptionsAtPosition(node);
                        nodeEvent.Use();
                    }

                    break;
                
                case EventType.MouseDrag:

                    if (IsLeftMouseClick(nodeEvent.button) && node.IsSelected)
                    {
                        node.Drag(nodeEvent.delta);
                        nodeEvent.Use();
                    }

                    break;
            }
        }

        private bool IsPointerOverNode(Vector2 pointerPosition, StateNode node)
        {
            return node.GetRect().Contains(pointerPosition);
        }
        
        private bool IsLeftMouseClick(int button)
        {
            return button == 0;
        }

        private bool IsRightMouseClick(int button)
        {
            return button == 1;
        }

        private void DisplayNodeOptionsAtPosition(StateNode node)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, () => _window.RemoveNode(node));
            genericMenu.ShowAsContext();
        }
    }
}


