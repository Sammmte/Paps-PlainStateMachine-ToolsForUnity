using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal static class PlainStateMachineGenericTypeDrawerFactory
    {
        private static readonly Type _intType = typeof(int);
        private static readonly Type _floatType = typeof(float);
        private static readonly Type _stringType = typeof(string);

        public static PlainStateMachineGenericTypeDrawer Create(Type stateIdType, object beginValue = null)
        {
            if(stateIdType != null)
            {
                if (stateIdType == _intType)
                {
                    return new IntDrawer(beginValue);
                }
                else if (stateIdType == _floatType)
                {
                    return new FloatDrawer(beginValue);
                }
                else if (stateIdType == _stringType)
                {
                    return new StringDrawer(beginValue);
                }
                else if (stateIdType.IsEnum)
                {
                    return new EnumDrawer(stateIdType, beginValue);
                }
            }

            return null;
        }
    }
}