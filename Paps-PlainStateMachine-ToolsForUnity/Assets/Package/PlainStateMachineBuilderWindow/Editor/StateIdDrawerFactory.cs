using System;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    public static class StateIdDrawerFactory
    {
        private static readonly Type _intType = typeof(int);
        private static readonly Type _floatType = typeof(float);
        private static readonly Type _stringType = typeof(string);

        public static StateIdDrawer CreateForType(Type type)
        {
            if(type != null)
            {
                if (type == _intType)
                {
                    return new IntStateIdDrawer();
                }
                else if (type == _floatType)
                {
                    return new FloatStateIdDrawer();
                }
                else if (type == _stringType)
                {
                    return new StringStateIdDrawer();
                }
                else if (type.IsEnum)
                {
                    return new EnumStateIdDrawer(type);
                }
            }

            return null;
        }
    }
}