using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendDestroy
    {
        public static void DestroyAll<T>(this T[] components) where T : Component
        {
            foreach (var component in components)
            {
                Object.Destroy(component.gameObject);
            }
        }

        public static void DestroySelf(this Object go)
        {
            Object.Destroy(go);
        }

        public static void DestroyImmediateSelf(this Object go)
        {
            Object.DestroyImmediate(go);
        }
        
        public static bool IsDestroyed(this Object go)
        {
            return go == null;
        }

    }
}