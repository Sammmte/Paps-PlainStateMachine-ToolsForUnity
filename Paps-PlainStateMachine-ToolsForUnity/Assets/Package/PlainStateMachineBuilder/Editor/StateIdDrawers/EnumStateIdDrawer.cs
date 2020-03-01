using UnityEngine;
using System;
using UnityEditor;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public class EnumStateIdDrawer : StateIdDrawer
    {
        private Type _enumType;

        private string[] _possibleValues;

        private int _currentSelected;

        public EnumStateIdDrawer(Type enumType, object value) : base(value)
        {
            _enumType = enumType;

            Debug.Log(_enumType);

            SavePossibleValues(enumType);
        }

        private void SavePossibleValues(Type enumType)
        {
            var values = Enum.GetValues(enumType);

            _possibleValues = new string[values.Length + 1];
            _possibleValues[0] = "No Value";

            int index = 1;

            foreach (var enumValue in values)
            {
                _possibleValues[index] = enumValue.ToString();
                index++;
            }
        }

        protected override void DrawValueControl()
        {
            EditorGUI.BeginChangeCheck();

            _currentSelected = EditorGUILayout.Popup(_currentSelected, _possibleValues);

            if (EditorGUI.EndChangeCheck())
            {
                if (_currentSelected != 0)
                    StateId = Enum.Parse(_enumType, _possibleValues[_currentSelected]);
                else
                {
                    StateId = null;
                }
            }
        }

        protected override bool IsValidType(object value)
        {
            return value.GetType().IsEnum;
        }
    }
}