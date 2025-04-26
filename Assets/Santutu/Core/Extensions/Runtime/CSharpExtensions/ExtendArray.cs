using System;
using System.Collections.Generic;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendArray
    {
        public static bool Exists<T>(this T[] array, Predicate<T> match)
        {
            return Array.Exists(array, match);
        }


        public static bool Exists<T>(this T[] array, T item)
        {
            return Array.Exists(array, (el) => EqualityComparer<T>.Default.Equals(el, item));
        }
    }
}