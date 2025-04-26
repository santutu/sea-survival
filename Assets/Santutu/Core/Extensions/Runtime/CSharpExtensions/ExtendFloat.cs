using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendFloat
    {
        public static float GetFractional(this float value)
        {
            return value - Mathf.Floor(value);
        }
    }
}