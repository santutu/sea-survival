using System.Collections.Generic;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendIList
    {
        public static T GetRandom<T>(this IReadOnlyList<T> list)
        {
            var index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    }
}