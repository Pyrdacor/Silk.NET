using System;

namespace Silk.NET.UI
{
    internal static class Util
    {
        public static bool CheckGenericType(Type typeToCheck, Type baseType)
        {
            return typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == baseType;
        }
    }
}