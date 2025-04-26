using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendQuaternion
    {
        public static Vector3 GetDirection(this Quaternion quaternion)
        {
            return (quaternion * Vector3.forward);
        }
    }
}