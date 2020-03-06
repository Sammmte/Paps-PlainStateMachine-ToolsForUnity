using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class TransitionConnection
    {
        private readonly StateNode _source;
        private readonly StateNode _target;

        public Vector3 StartPoint => _source.Center;
        public Vector3 EndPoint => _target.Center;
        public Vector3 StartTangent => StartPoint + Vector3.left * 50f;
        public Vector3 EndTangent => EndPoint - Vector3.left * 50f;
        
        public TransitionConnection(StateNode source, StateNode target)
        {
            _source = source;
            _target = target;
        }

        public void Draw()
        {
            Handles.DrawBezier(StartPoint, EndPoint, StartTangent, EndTangent, Color.yellow, null, 8f);
        }

        public bool IsPointOverConnection(Vector3 point, float range)
        {
            return HandleUtility.DistancePointBezier(point, StartPoint, EndPoint, StartTangent, EndTangent) <= range;
        }
    }
}

