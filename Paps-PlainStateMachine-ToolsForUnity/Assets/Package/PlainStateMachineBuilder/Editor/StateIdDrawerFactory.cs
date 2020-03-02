using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal static class StateIdDrawerFactory
    {
        private static readonly Type _intType = typeof(int);
        private static readonly Type _floatType = typeof(float);
        private static readonly Type _stringType = typeof(string);

        public static StateIdDrawer Create(Type stateIdType, object beginValue = null)
        {
            if(stateIdType != null)
            {
                if (stateIdType == _intType)
                {
                    return new IntStateIdDrawer(beginValue);
                }
                else if (stateIdType == _floatType)
                {
                    return new FloatStateIdDrawer(beginValue);
                }
                else if (stateIdType == _stringType)
                {
                    return new StringStateIdDrawer(beginValue);
                }
                else if (stateIdType.IsEnum)
                {
                    return new EnumStateIdDrawer(stateIdType, beginValue);
                }
            }

            return null;
        }
    }
}