using UnityEditor;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class TransitionConnectionPreview
    {
        private const float Width = 8f;

        public StateNode Source { get; private set; }

        private Vector3 StartPoint => Source.Center;

        public TransitionConnectionPreview(StateNode source)
        {
            Source = source;
        }

        public void Draw(Vector2 currentEndPoint)
        {
            var previousColor = Handles.color;
            Handles.color = Color.yellow;
            Handles.DrawAAPolyLine(Width, StartPoint, currentEndPoint);
            Handles.color = previousColor;
        }
    }
}