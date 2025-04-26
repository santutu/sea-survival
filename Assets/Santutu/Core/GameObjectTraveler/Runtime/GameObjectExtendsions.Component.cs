using UnityEngine;

namespace Santutu.Core.GameObjectTraveler.Runtime
{
    public static partial class GameObjectExtensions
    {
        public static bool TryGetComponentInSelfBelow<T>(this GameObject go, out T comp)
        {
            foreach (var child in go.SelfBelow())
            {
                if (child.TryGetComponent<T>(out comp))
                {
                    return true;
                }
            }

            comp = default;
            return false;
        }
    }
}