using Santutu.Core.GameObjectTraveler.Runtime;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static partial class ExtendGameObject
    {
        public static bool TryFindGameObjectWithTagInChildren(this GameObject go, string tag, out GameObject found)
        {
            var isValid = !string.IsNullOrEmpty(tag.Trim());

            if (!isValid)
            {
                found = null;
                return false;
            }


            foreach (var child in go.SelfBelow())
            {
                if (child.CompareTag(tag))
                {
                    found = child;
                    return true;
                }
            }

            found = null;
            return false;
        }
    }
}