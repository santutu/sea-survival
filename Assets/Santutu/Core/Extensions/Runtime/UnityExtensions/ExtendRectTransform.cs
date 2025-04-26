using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Santutu.Core.GameObjectTraveler.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendRectTransform
    {
        public static LayoutGroup[] GetLayoutGroupsInIChildren(this Transform tf)
        {
            return tf.gameObject.SelfBelow()
                     .Where(go => go.TryGetComponent<LayoutGroup>(out var comp) && comp != null && comp.gameObject != null)
                     .Select(g => g.GetComponent<LayoutGroup>())
                     .ToArray();
        }

        public static void ForceRebuildLayoutImmediate(this Transform tf)
        {
            foreach (var rtf in tf.GetLayoutGroupsInIChildren())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rtf.transform);
            }
        }

        public static void ForceRebuildLayoutImmediate(this IEnumerable<LayoutGroup> layoutGroups)
        {
            foreach (var rtf in layoutGroups)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rtf.transform);
            }
        }

        public static void ForceRebuildLayoutImmediate(this LayoutGroup layoutGroup)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroup.transform);
        }

        public static bool PositionNear(this RectTransform rtf, RectTransform target)
        {
            var rect = rtf.GetWorldRect();
            var targetRect = target.GetWorldRect();

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            Vector2 position = new Vector2(
                targetRect.center.x,
                targetRect.yMax
            );

            //overlay is top
            var pivot = new Vector2(0.5f, 0f);
            rtf.pivot = pivot;
            rtf.position = position;

            rect = rtf.GetWorldRect();


            if (rect.xMax > screenWidth)
            {
                pivot.x = 1f;
                rtf.pivot = pivot;
                position.x = screenWidth;
            }
            else if (rect.xMin < 0)
            {
                pivot.x = 0;
                rtf.pivot = pivot;
                position.x = 0;
            }

            //overlay is bottom
            if (rect.yMax > screenHeight)
            {
                pivot.y = 1f;
                rtf.pivot = pivot;
                position.y = targetRect.yMin;
            }

            rtf.position = position;

            rect = rtf.GetWorldRect();

            //overlay is left
            if (rect.yMin < 0)
            {
                pivot = new(1, 1);
                pivot.y = 1f;
                rtf.pivot = pivot;
                position = new(targetRect.xMin, targetRect.yMax);
            }

            rtf.position = position;
            rect = rtf.GetWorldRect();

            if (rect.yMax > screenHeight)
            {
                pivot.y = 1f;
                rtf.pivot = pivot;
                position.y = screenHeight;
            }
            else if (rect.yMin < 0)
            {
                pivot.y = 0;
                rtf.pivot = pivot;
                position.y = 0;
            }


            rect = rtf.GetWorldRect();

            //overlay is right
            if (rect.xMin < 0)
            {
                pivot.x = 0f;
                rtf.pivot = pivot;
                position.x = targetRect.xMax;
            }

            rtf.position = position;

            rect = rtf.GetWorldRect();

            //overlay is center
            if (rect.xMax > screenWidth || rect.yMin < 0 || rect.xMin < 0 || rect.yMax > screenHeight)
            {
                rtf.pivot = new Vector2(0.5f, 0.5f);
                rtf.position = new Vector2(screenWidth / 2, screenHeight / 2);
            }


            if (rect.xMax > screenWidth || rect.xMin < 0)
            {
                return false;
            }

            if (rect.yMax > screenHeight || rect.yMin < 0)
            {
                return false;
            }

            return true;
        }


        public static Rect GetWorldRect(this RectTransform rtf)
        {
            var corners = ArrayPool<Vector3>.Shared.Rent(4);
            rtf.GetWorldCorners(corners);

            var size = new Vector2(corners[3].x - corners[0].x, corners[1].y - corners[0].y);
            Vector2 position = corners[0];
            var rect = new Rect(position, size);


            ArrayPool<Vector3>.Shared.Return(corners);

            return rect;
        }
    }
}