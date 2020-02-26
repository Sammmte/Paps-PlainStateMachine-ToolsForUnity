using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class WindowEventHandler
    {
        private PlainStateMachineBuilderEditorWindow _window;

        public WindowEventHandler(PlainStateMachineBuilderEditorWindow window)
        {
            _window = window;
        }

        public void HandleEvent(Event windowEvent)
        {
            switch (windowEvent.type)
            {
                case EventType.MouseDown:

                    if (windowEvent.button == 1)
                    {
                        DisplayGeneralOptionsMenuAtPosition(windowEvent.mousePosition);
                    }

                    break;
                
                case EventType.MouseDrag:

                    if (IsLeftMouseClick(windowEvent.button))
                    {
                        _window.Drag(windowEvent.delta);
                    }

                    break;
            }
        }

        private bool IsLeftMouseClick(int button)
        {
            return button == 0;
        }

        private void DisplayGeneralOptionsMenuAtPosition(Vector2 position)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => _window.AddNodeAtPosition(position));
            genericMenu.ShowAsContext();
        }
    }
}


