using System;
using System.Collections.Generic;
using Santutu.Core.Base.Runtime.Contracts;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendHasComponent
    {
        public static bool HasComponent<T>(this GameObject gameObject)
        {
            return gameObject.TryGetComponent<T>(out _);
        }


        public static bool HasComponent<T>(this Component comp)
        {
            return comp.TryGetComponent<T>(out _);
        }


        public static IEnumerable<TSource> HasComponent<TSource>(this IEnumerable<TSource> source, Type type) where TSource : Component
        {
            foreach (var item in source)
            {
                if (item.TryGetComponent(type, out _))
                {
                    yield return item;
                }
            }
        }

        
    }

    public static class ExtendIMonoBehaviourHasComponent
    {
        public static IEnumerable<TSource> HasComponent<TSource>(this IEnumerable<TSource> source, Type type) where TSource : IMonoBehaviour
        {
            foreach (var item in source)
            {
                if (item.TryGetComponent(type, out _))
                {
                    yield return item;
                }
            }
        }
    }
}