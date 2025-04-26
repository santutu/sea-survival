using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendSphereCollider
    {
        public static void SetDefault(this SphereCollider collider, float radius = 1f)
        {
            collider.center = Vector3.zero;
            collider.radius = radius;
        }

        public static Vector3 GetRelativeCenter(this SphereCollider collider)
        {
            return collider.center.Multiply(collider.transform.lossyScale);
        }

        public static Vector3 GetAbsoluteCenter(this SphereCollider collider)
        {
            return collider.transform.position + collider.transform.TransformDirection(collider.GetRelativeCenter());
        }

        public static float GetAbsoluteRadius(this SphereCollider collider)
        {
            return collider.radius * collider.transform.lossyScale.Max();
        }


        public static void SetAbsoluteRadius(this SphereCollider collider, float radius)
        {
            collider.radius = radius / collider.transform.lossyScale.Max();
        }


        public static Vector3 GetRelativePoint0(this SphereCollider collider)
        {
            var heightHalf = collider.radius * Vector3.up;
            var center = collider.GetRelativeCenter();

            return center + heightHalf;
        }

        public static Vector3 GetRelativePoint1(this SphereCollider collider)
        {
            var heightHalf = collider.radius * Vector3.down;
            var center = collider.GetRelativeCenter();

            return center + heightHalf;
        }
    }
}