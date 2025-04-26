using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static partial class ExtendGameObject
    {
        public static GameObject Instantiate(this GameObject go)
        {
            return Object.Instantiate(go);
        }

        public static T Instantiate<T>(this GameObject go) where T : Component
        {
            var newGo = Object.Instantiate(go);
            return newGo.GetOrAddComponent<T>();
        }
    }
}