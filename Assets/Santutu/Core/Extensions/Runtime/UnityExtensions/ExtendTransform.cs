using System.Collections.Generic;
using System.Linq;
using Santutu.Core.GameObjectTraveler.Runtime;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendTransform
    {
        public static void SetLocalToDefault(this Transform tf)
        {
            tf.localPosition = Vector3.zero;
            tf.localEulerAngles = Vector3.zero;
            tf.localScale = Vector3.one;
        }

        public static IEnumerable<Transform> Children(this Transform tf)
        {
            return tf.gameObject.Children().Select(go => go.transform);
        }
    }
}