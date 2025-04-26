using System.Collections.Generic;
using Santutu.Core.Base.Runtime.Contracts;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendLinqComponent
    {
        public static IEnumerable<T> OfComponent<T>(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                if (item.TryGetComponent(out T comp))
                {
                    yield return comp;
                }
            }
        }

        public static IEnumerable<T> OfComponent<T>(this IEnumerable<Component> source)
        {
            foreach (var item in source)
            {
                if (item.TryGetComponent(out T comp))
                {
                    yield return comp;
                }
            }
        }

        public static IEnumerable<T> OfComponent<T>(this IEnumerable<IMonoBehaviour> source)
        {
            foreach (var item in source)
            {
                if (item.TryGetComponent(out T comp))
                {
                    yield return comp;
                }
            }
        }
        
        
    }
}