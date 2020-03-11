using UnityEditor;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class WindowEventHandler
    {
        private PlainStateMachineBuilderEditorWindow _window;

        private bool _wantsToDrag;

        public WindowEventHandler(PlainStateMachineBuilderEditorWindow window)
        {
            _window = window;
        }

        public void HandleEvent(Event windowEvent)
        {
            switch (windowEvent.type)
            {
                case EventType.KeyDown:

                    if (IsDragKey(windowEvent.keyCode))
                        _wantsToDrag = true;

                    break;

                case EventType.KeyUp:

                    if (IsDragKey(windowEvent.keyCode))
                        _wantsToDrag = false;

                    break;

                case EventType.MouseDown:

                    if (IsRightMouseClick(windowEvent.button))
                    {
                        DisplayGeneralOptionsMenu(windowEvent.mousePosition);
                        windowEvent.Use();
                    }

                    break;
                
                case EventType.MouseDrag:

                    if (_wantsToDrag && IsLeftMouseClick(windowEvent.button))
                    {
                        _window.Drag(windowEvent.delta);
                        windowEvent.Use();
                    }

                    break;
            }
        }

        private bool IsLeftMouseClick(int button)
        {
            return button == 0;
        }

        private bool IsRightMouseClick(int button)
        {
            return button == 1;
        }

        private void DisplayGeneralOptionsMenu(Vector2 position)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => _window.AddNode(position));
            genericMenu.ShowAsContext();
        }

        private bool IsDragKey(KeyCode code)
        {
            return code == KeyCode.LeftAlt || code == KeyCode.LeftCommand;
        }
    }
}


