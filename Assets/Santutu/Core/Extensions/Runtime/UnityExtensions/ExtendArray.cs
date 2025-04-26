using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendArray
    {
        public static T[] Fill<T>(this T[] source, T item)
        {
            for (int i = 0; i < source.Length; i++)
            {
                source[i] = item;
            }

            return source;
        }

        public static void Shuffle<T>(this T[] source)
        {
            int n = source.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + Random.Range(0, n - i);
                (source[r], source[i]) = (source[i], source[r]);
            }
        }

        public static IEnumerable<object> ToEnumerable(this Array arr)
        {
            foreach (var o in arr)
            {
                yield return o;
            }
        }
    }
}