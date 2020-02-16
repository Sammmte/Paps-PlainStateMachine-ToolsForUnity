using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public static class StateIdDrawerFactory
    {
        private static readonly Type _intType = typeof(int);
        private static readonly Type _floatType = typeof(float);
        private static readonly Type _stringType = typeof(string);

        public static StateIdDrawer Create(Type stateIdType, IStateIdValidator stateIdValidator, object beginValue = null)
        {
            if(stateIdType != null)
            {
                if (stateIdType == _intType)
                {
                    return new IntStateIdDrawer(stateIdValidator, beginValue);
                }
                else if (stateIdType == _floatType)
                {
                    return new FloatStateIdDrawer(stateIdValidator, beginValue);
                }
                else if (stateIdType == _stringType)
                {
                    return new StringStateIdDrawer(stateIdValidator, beginValue);
                }
                else if (stateIdType.IsEnum)
                {
                    return new EnumStateIdDrawer(stateIdType, stateIdValidator, beginValue);
                }
            }

            return null;
        }
    }
}