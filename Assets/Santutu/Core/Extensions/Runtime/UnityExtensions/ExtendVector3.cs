using Santutu.Core.Extensions.Runtime.CSharpExtensions;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendVector3
    {
        public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        public static bool Any(this Vector3 vec, float any)
        {
            if (vec.x == any || vec.y == any || vec.z == any)
            {
                return true;
            }

            return false;
        }


        public static Vector3 GetCenter(this Vector3 from, Vector3 to)
        {
            return to + ((from - to) / 2);
        }

        public static float GetDistance(this Vector3 from, Vector3 to)
        {
            return Vector3.Distance(from, to);
        }

        public static float GetSqrDistance(this Vector3 from, Vector3 to)
        {
            return (to - from).sqrMagnitude;
        }

        public static float Max(this Vector3 vector3)
        {
            return Mathf.Max(vector3.x, vector3.y, vector3.z);
        }

        public static float MaxXZ(this Vector3 vector3)
        {
            return Mathf.Max(vector3.x, vector3.z);
        }

        public static Vector3 Multiply(this Vector3 v3, Vector3 scale)
        {
            return Vector3.Scale(v3, scale);
        }

        public static Vector3 Divide(this Vector3 dividend, Vector3 divisor)
        {
            return new Vector3(dividend.x / divisor.x, dividend.y / divisor.y, dividend.z / divisor.z);
        }


        public static Vector3 Add(this Vector3 a, Vector3 b)
        {
            return a + b;
        }

        public static Vector3 AddY(this Vector3 vector3, float value)
        {
            return new Vector3(vector3.x, vector3.y + value, vector3.z);
        }

        public static Vector3 AddZ(this Vector3 vector3, float value)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z + value);
        }

        public static Vector3 AddX(this Vector3 vector3, float value)
        {
            return new Vector3(vector3.x + value, vector3.y, vector3.z);
        }

        public static Vector3 SubtractY(this Vector3 vector3, float value)
        {
            return new Vector3(vector3.x, vector3.y - value, vector3.z);
        }

        public static Vector3 SubtractZ(this Vector3 vector3, float value)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z - value);
        }

        public static Vector3 SubtractX(this Vector3 vector3, float value)
        {
            return new Vector3(vector3.x - value, vector3.y, vector3.z);
        }


        public static Quaternion DirectionToQuaternion(this Vector3 v)
        {
            return Quaternion.LookRotation(v);
        }

        public static Quaternion EulerAnglesToQuaternion(this Vector3 v)
        {
            return Quaternion.Euler(v);
        }

        public static Quaternion LookRotation(this Vector3 v)
        {
            return Quaternion.LookRotation(v);
        }


        public static Vector3 EulerAnglesToDirection(this Vector3 v)
        {
            return v.EulerAnglesToQuaternion().GetDirection();
        }

        public static Vector3 AddEulerAngles(this Vector3 v, Vector3 v2)
        {
            var v3 = v + v2;
            return new Vector3(v3.x % 360, v3.y % 360, v3.z % 360);
        }

        public static Vector3 ChangeYZ(this Vector3 vec)
        {
            return new Vector3(vec.x, vec.z, vec.y);
        }


        public static Vector3Int Floor(this Vector3 vec)
        {
            return new Vector3Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y), Mathf.FloorToInt(vec.z));
        }

        public static Vector3 GetFractional(this Vector3 vec)
        {
            return new Vector3(vec.x.GetFractional(), vec.y.GetFractional(), vec.z.GetFractional());
        }

        public static Vector3Int Ceil(this Vector3 vec)
        {
            return new Vector3Int(Mathf.CeilToInt(vec.x), Mathf.CeilToInt(vec.y), Mathf.CeilToInt(vec.z));
        }
    }
}