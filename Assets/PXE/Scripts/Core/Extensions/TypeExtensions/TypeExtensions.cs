using System;

namespace PXE.Core.Extensions.TypeExtensions
{
    public static class TypeExtensions
    {
        public static bool IsBaseType(this Type type, Type baseType)
        {
            while (type != null)
            {
                if (type.BaseType == baseType)
                    return true;
                type = type.BaseType;
            }
            return false;
        }
    }
}