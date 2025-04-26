using System;
using System.Collections.Generic;
using Santutu.Core.GameObjectTraveler.Runtime;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendGetComponent
    {
        public static T GetComponentOrAssert<T>(this GameObject gameObject)
        {
            var comp = gameObject.GetComponent<T>();
            Debug.Assert(comp != null, $"{gameObject.name} is null", gameObject);
            return comp;
        }

        public static T GetComponentOrAssert<T>(this Component component)
        {
            return component.gameObject.GetComponentOrAssert<T>();
        }

        public static T GetComponentOrNull<T>(this GameObject gameObject)
        {
            return gameObject.GetComponent<T>();
        }

        public static T GetComponentOrNull<T>(this Component component)
        {
            return component.GetComponent<T>();
        }

        public static T AddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddComponent<T>();
        }


        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (!component.TryGetComponent<T>(out var result))
            {
                return component.gameObject.AddComponent<T>();
            }

            return result;
        }


        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent<T>(out var result))
            {
                return go.AddComponent<T>();
            }

            return result;
        }

        public static IEnumerable<T> GetComponentsInOneDepthChildren<T>(this Transform parent)

        {
            foreach (var child in parent.gameObject.Children())
            {
                foreach (var component in child.GetComponents<T>())
                {
                    yield return component;
                }
            }
        }


        public static Collider GetShapeColliderOrNullInSelfBelow(this Transform transform)
        {
            foreach (var gameObject in transform.gameObject.SelfBelow())
            {
                if (gameObject.TryGetComponent<CapsuleCollider>(out var capsuleCollider))
                {
                    return capsuleCollider;
                }

                if (gameObject.TryGetComponent<SphereCollider>(out var sphereCollider))
                {
                    return sphereCollider;
                }

                if (gameObject.TryGetComponent<BoxCollider>(out var boxCollider))
                {
                    return boxCollider;
                }
            }

            return null;
        }

        public static Collider GetShapeColliderInSelfBelow(this Transform transform)
        {
            var collider = transform.GetShapeColliderOrNullInSelfBelow();

            if (collider == null)
            {
                Debug.LogError("not found collider", transform.gameObject);
                throw new Exception($"not found collider");
            }

            return collider;
        }
    }
}