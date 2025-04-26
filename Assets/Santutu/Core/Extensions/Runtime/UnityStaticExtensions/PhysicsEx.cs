using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityStaticExtensions
{
    public static class PhysicsEx
    {
        public static bool TryGetWorldMousePosition(out Vector3 mousePosition, float maxDistance, int layerMask = -1)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var found = Physics.Raycast(ray, out var hit, maxDistance, layerMask);

            mousePosition = hit.point;

            return found;
        }

        public static bool RaycastMousePosition<T>(out T comp, float maxDistance, int layerMask = -1) where T : class
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, maxDistance, layerMask);

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<T>(out comp))
                {
                    return true;
                }
            }

            comp = null;
            return false;
        }

        public static bool RaycastMousePosition(out RaycastHit hit, float maxDistance, int layerMask = -1)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, maxDistance, layerMask);
        }

        public static bool RaycastMousePosition<T>(Camera camera, out T comp, float maxDistance, int layerMask = -1)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            var result = Physics.Raycast(ray, out var hit, maxDistance, layerMask);

            comp = default;
            return result && hit.collider.gameObject.TryGetComponent<T>(out comp);
        }


        public static bool RaycastMousePosition(Camera camera, out RaycastHit hit, float maxDistance, int layerMask = -1)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, maxDistance, layerMask);
        }

        public static bool RaycastDebug(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
        {
            var hit = Physics.Raycast(origin, direction, out var hitInfo, maxDistance, layerMask);
#if UNITY_EDITOR
            if (hit)
            {
                Debug.DrawLine(origin, hitInfo.point, Color.red);
            }
            else
            {
                Debug.DrawLine(origin, origin + direction * maxDistance, Color.green);
            }

#endif

            return hit;
        }


        public static bool RaycastDebug(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
        {
            var hit = Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask);
#if UNITY_EDITOR
            if (hit)
            {
                Debug.DrawLine(origin, hitInfo.point, Color.red);
            }
            else
            {
                Debug.DrawLine(origin, origin + direction * maxDistance, Color.green);
            }
#endif

            return hit;
        }
    }
}