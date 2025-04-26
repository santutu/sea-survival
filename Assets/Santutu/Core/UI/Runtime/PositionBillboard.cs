using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class PositionBillboard : MonoBehaviour
    {
        public Vector3 offset;

        public Transform target;

        public void Initialize(Transform newTarget, Vector3 offset)
        {
            target = newTarget;
            this.offset = offset;
        }


        private void LateUpdate()
        {
            transform.position = target.position + offset;
        }
    }
}