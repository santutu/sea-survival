using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendUnitObject
    {
        public static T EnsureNull<T>(this T obj) where T : Object
        {
            if (obj == null)
            {
                return null;
            }

            return obj;
        }
    }
}