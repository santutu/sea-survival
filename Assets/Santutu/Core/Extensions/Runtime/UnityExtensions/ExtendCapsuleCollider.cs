using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendCapsuleCollider
    {
        public static void SetAbsoluteCenter(this CapsuleCollider collider, Vector3 center)
        {
            var relativeCenter = center - collider.transform.position;
            collider.center = relativeCenter.Divide(collider.transform.lossyScale);
        }


        public static void SetAbsoluteHeight(this CapsuleCollider collider, float height)
        {
            collider.height = height / collider.transform.lossyScale.y;
        }

        public static void SetAbsoluteRadius(this CapsuleCollider collider, float radius)
        {
            collider.radius = radius / collider.transform.lossyScale.MaxXZ();
        }


        public static Vector3 GetRelativePoint0(this CapsuleCollider collider)
        {
            var heightHalf = collider.GetAbsoluteHeight() * Vector3.up / 2;
            var center = collider.GetRelativeCenter();

            return center + heightHalf;
        }

        public static Vector3 GetRelativePoint1(this CapsuleCollider collider)
        {
            var heightHalf = collider.GetAbsoluteHeight() * Vector3.down / 2;
            var center = collider.GetRelativeCenter();

            return center + heightHalf;
        }


        public static Vector3 GetAbsolutePoint0(this CapsuleCollider collider)
        {
            return collider.transform.TransformDirection(collider.GetRelativePoint0()) + collider.transform.position;
        }

        public static Vector3 GetAbsolutePoint1(this CapsuleCollider collider)
        {
            return collider.transform.TransformDirection(collider.GetRelativePoint1()) + collider.transform.position;
        }


        public static float GetAbsoluteRadius(this CapsuleCollider collider)
        {
            return collider.radius * collider.transform.lossyScale.MaxXZ();
        }

        public static float GetAbsoluteHeight(this CapsuleCollider collider)
        {
            float height = Mathf.Max(collider.height, collider.radius * 2);
            return height * collider.transform.lossyScale.y;
        }

        public static Vector3 GetRelativeCenter(this CapsuleCollider collider)
        {
            return collider.center.Multiply(collider.transform.lossyScale);
        }

        public static Vector3 GetAbsoluteCenter(this CapsuleCollider collider)
        {
            return collider.transform.position + collider.transform.TransformDirection(collider.GetRelativeCenter());
        }
    }
}