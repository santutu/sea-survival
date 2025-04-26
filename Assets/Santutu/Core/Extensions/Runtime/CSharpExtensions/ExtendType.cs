using System;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendType
    {
        public static bool IsAssignableTo(this Type from, Type to)
        {
            return to.IsAssignableFrom(from);
        }

        public static bool IsAssignableToAny(this Type from, params Type[] types)
        {
            foreach (var type in types)
            {
                if (type.IsAssignableFrom(from))
                {
                    return true;
                }
            }

            return false;
        }
    }
}