using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendLinq
    {
        private static readonly Random random = new Random();

        public static T RandomOne<T>(this IEnumerable<T> source)
        {
            return source.Random(1).First();
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var list = source.ToList();
            int n = list.Count;

            if (count > n) count = n;

            for (int i = 0; i < count; i++)
            {
                int index = random.Next(n - i);
                yield return list[index];

                (list[index], list[n - i - 1]) = (list[n - i - 1], list[index]);
            }
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Select(g => g.First());
        }
    }
}