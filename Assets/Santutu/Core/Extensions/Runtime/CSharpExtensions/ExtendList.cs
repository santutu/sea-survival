using System;
using System.Collections.Generic;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendList
    {
        public static ICollection<T> ReplaceAll<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            list.Clear();

            foreach (var item in items)
            {
                list.Add(item);
            }

            return list;
        }


        public static List<T> Splice<T>(this List<T> list, int startIdx, int deleteCount, params T[] items)
        {
            if (startIdx < 0)
                startIdx = list.Count + startIdx;

            startIdx = Math.Max(0, Math.Min(startIdx, list.Count));

            deleteCount = Math.Max(0, deleteCount);

            if (startIdx + deleteCount > list.Count)
                deleteCount = list.Count - startIdx;

            List<T> removedItems = list.GetRange(startIdx, deleteCount);

            list.RemoveRange(startIdx, deleteCount);

            if (items != null && items.Length > 0)
                list.InsertRange(startIdx, items);

            return removedItems;
        }

        public static bool TryFind<T>(this List<T> list, out T result, Predicate<T> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    result = item;
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}