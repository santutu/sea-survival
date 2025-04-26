using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendMonoBehaviour
    {
        public static CanvasGroup GetCanvasGroup(this MonoBehaviour mono)
        {
            return mono.GetOrAddComponent<CanvasGroup>();
        }
    }
}