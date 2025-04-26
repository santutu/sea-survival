using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendLinqVector
    {
        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            return vectors.Aggregate(Vector3.zero, (acc, v) => acc + v) / vectors.Count();
        }
    }
}