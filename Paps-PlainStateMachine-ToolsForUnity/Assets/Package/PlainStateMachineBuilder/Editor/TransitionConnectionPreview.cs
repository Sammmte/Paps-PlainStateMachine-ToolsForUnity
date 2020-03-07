using UnityEditor;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class TransitionConnectionPreview
    {
        private const float Width = 8f;

        public StateNode Source { get; private set; }

        private Vector3 StartPoint => Source.Center;
        private Vector3 StartTangent => StartPoint + Vector3.left * 50f;


        public TransitionConnectionPreview(StateNode source)
        {
            Source = source;
        }

        public void Draw(Vector2 currentEndPoint)
        {
            Handles.DrawBezier(StartPoint, currentEndPoint, StartTangent, GetEndTangent(currentEndPoint), Color.yellow, null, Width);
        }

        private Vector3 GetEndTangent(Vector2 endPoint)
        {
            return endPoint - Vector2.left * 50f;
        }
    }
}