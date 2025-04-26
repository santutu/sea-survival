using System;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendException
    {
        public static T ThrowIfNull<T>(this T obj, string message = "") where T : class
        {
            if (obj == null)
            {
                throw new NullReferenceException($"{typeof(T)} :{message}");
            }

            return obj;
        }

        public static float ThrowIfNotInRange(this float f, float minInclude, float maxInclude)
        {
            if (f < minInclude || f > maxInclude)
            {
                throw new Exception($"{minInclude} <= ${f} <= ${maxInclude}");
            }

            return f;
        }
    }
}