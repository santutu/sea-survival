using System;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendObject
    {
        public static object ConvertTo(this string value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            if (targetType == typeof(string))
            {
                return value;
            }
            else if (targetType == typeof(int))
            {
                return int.Parse(value);
            }
            else if (targetType == typeof(float))
            {
                return float.Parse(value);
            }
            else if (targetType == typeof(bool))
            {
                return bool.Parse(value);
            }
            else if (targetType.IsEnum)
            {
                return System.Enum.Parse(targetType, value, true);
            }

            throw new System.ArgumentException($"Unsupported type conversion: {targetType}");
        }
    }
}