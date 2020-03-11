using UnityEditor;
using UnityEngine;
using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class TransitionConnection
    {
        private const float Width = 8f, ClickableExtraRange = 8f;

        private const int ControlPaddingLeft = 20, ControlPaddingRight = 20, ControlPaddingTop = 20, ControlPaddingBottom = 20;

        private static readonly Color SelectedColor = new Color(44f / 255f, 130f / 255f, 201f / 255f);

        private readonly StateNode _source;
        private readonly StateNode _target;
        
        private bool _guardConditionsArrayOpened;
        private GUIStyle _controlsAreaStyle;
        private PlainStateMachineGenericTypeDrawer _triggerDrawer;

        public Vector3 StartPoint => _source.Center;
        public Vector3 EndPoint => _target.Center;
        public object Trigger => _triggerDrawer.Value;
        public ScriptableGuardCondition[] GuardConditions { get; private set; }
        public Action<TransitionConnection, object, object> OnTriggerChanged;
        public Action<TransitionConnection, ScriptableGuardCondition[]> OnGuardConditionsChanged;

        public StateNode Source => _source;
        public StateNode Target => _target;

        public object StateFrom => Source.StateId;
        public object StateTo => Target.StateId;

        public TransitionConnection(StateNode source, StateNode target, Type triggerType, object trigger = null, ScriptableGuardCondition[] guardConditions = null)
        {
            _source = source;
            _target = target;

            _triggerDrawer = PlainStateMachineGenericTypeDrawerFactory.Create(triggerType, trigger);
            GuardConditions = guardConditions;

            _controlsAreaStyle = new GUIStyle();
            _controlsAreaStyle.padding = new RectOffset(ControlPaddingLeft, ControlPaddingRight, ControlPaddingTop, ControlPaddingBottom);
        }

        public void SetNewTriggerType(Type triggerType)
        {
            if (triggerType == null)
                _triggerDrawer = null;
            else
                _triggerDrawer = PlainStateMachineGenericTypeDrawerFactory.Create(triggerType);
        }

        public void Draw(bool asSelected)
        {
            var previousColor = Handles.color;
            Handles.color = asSelected ? SelectedColor : Color.yellow;
            Handles.DrawAAPolyLine(Width,StartPoint, EndPoint);
            Handles.color = previousColor;
        }

        public void DrawControls()
        {
            EditorGUILayout.BeginVertical(_controlsAreaStyle);

            var previousTrigger = Trigger;
            DrawTriggerField();
            if (previousTrigger != Trigger) OnTriggerChanged?.Invoke(this, previousTrigger, Trigger);

            var previousGuardConditions = GuardConditions;
            DrawGuardConditionsField();
            if (previousGuardConditions != GuardConditions) OnGuardConditionsChanged?.Invoke(this, GuardConditions);

            EditorGUILayout.EndVertical();
        }

        private void DrawTriggerField()
        {
            _triggerDrawer.Draw("Trigger");
        }

        private void DrawGuardConditionsField()
        {
            GuardConditions = ArrayField("Guard Conditions", ref _guardConditionsArrayOpened, GuardConditions);
        }

        public bool IsPointOverConnection(Vector2 point)
        {
            return HandleUtility.DistancePointLine(point, StartPoint, EndPoint) <= ClickableExtraRange;
        }

        public ScriptableGuardCondition[] ArrayField(string label, ref bool open, ScriptableGuardCondition[] array)
        {
            if (array == null)
                array = new ScriptableGuardCondition[0];

            open = EditorGUILayout.Foldout(open, label);
            int newSize = array.Length;

            if (open)
            {
                newSize = EditorGUILayout.IntField("Size", newSize);
                newSize = newSize < 0 ? 0 : newSize;

                if (newSize != array.Length)
                {
                    array = ResizeArray(array, newSize);
                }

                for (var i = 0; i < newSize; i++)
                {
                    array[i] = (ScriptableGuardCondition)EditorGUILayout.ObjectField("Value " + i, array[i], typeof(ScriptableGuardCondition), false);
                }
            }
            return array;
        }

        private T[] ResizeArray<T>(T[] array, int size)
        {
            T[] newArray = new T[size];

            for (var i = 0; i < size; i++)
            {
                if (i < array.Length)
                {
                    newArray[i] = array[i];
                }
            }

            return newArray;
        }
    }
}

