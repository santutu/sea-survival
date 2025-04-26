using System;

namespace Santutu.Core.Extensions.Runtime.UnityStaticExtensions
{
    public static class RandomEx
    {
        private static readonly System.Random R = new System.Random();

        public static int Range(int minInclude, int maxInclude)
        {
            return UnityEngine.Random.Range(minInclude, maxInclude + 1);
        }


        public static float Range(float minInclude, float maxInclude)
        {
            return UnityEngine.Random.Range(minInclude, maxInclude);
        }

        public static int Range(ValueTuple<int, int> minMax)
        {
            return UnityEngine.Random.Range(minMax.Item1, minMax.Item2 + 1);
        }


        public static T GetRandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(R.Next(v.Length));
        }
    }
}