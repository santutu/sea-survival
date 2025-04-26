using System;
using System.Collections.Generic;
using Santutu.Core.Base.Runtime.Enums;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendDictionary
    {
        public static (GetOrAdd, TValue) GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFactory)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return (Base.Runtime.Enums.GetOrAdd.Get, value);
            }

            var newValue = valueFactory();
            dict[key] = newValue;

            return (Base.Runtime.Enums.GetOrAdd.Add, newValue);
        }
    }
}