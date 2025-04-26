using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendBoxCollider
    {
        public static void SetSize(this BoxCollider2D collider, RectTransform tf)
        {
            var rect = tf.rect;
            collider.size = new Vector2(rect.width, rect.height);
        }

        public static Vector3 GetAbsoluteSize(this BoxCollider collider)
        {
            return ExtendVector3.Multiply(collider.size, collider.transform.lossyScale);
        }


        public static void SetAbsoluteSize(this BoxCollider collider, Vector3 size)
        {
            Vector3 newSize = size.Divide(collider.transform.lossyScale);

            collider.size = newSize;
        }

        public static Vector3 GetAbsoluteCenter(this BoxCollider collider)
        {
            return collider.transform.position + collider.transform.TransformDirection(collider.GetRelativeCenter());
        }

        public static Vector3 GetRelativeCenter(this BoxCollider collider)
        {
            return ExtendVector3.Multiply(collider.center, collider.transform.lossyScale);
        }

        public static Vector3 GetRelativePoint0(this BoxCollider collider)
        {
            var heightHalf = collider.size.y * Vector3.up * 0.5f;
            var center = collider.GetRelativeCenter();

            return center + heightHalf;
        }

        public static Vector3 GetRelativePoint1(this BoxCollider collider)
        {
            var heightHalf = collider.size.y * Vector3.down * 0.5f;
            var center = collider.GetRelativeCenter();

            return center + heightHalf;
        }
    }
}