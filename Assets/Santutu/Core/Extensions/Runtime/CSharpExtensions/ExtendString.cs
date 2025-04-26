using System.Collections.Generic;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendString
    {
        public static string JoinAsString(this IEnumerable<string> items, string separator)
        {
            return string.Join(separator, items);
        }
    }
}