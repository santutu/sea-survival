using System;
using Santutu.Editors.EditorExtensions.Runtime;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class SaveOffsetMinMax : MonoBehaviour
    {
        [SerializeField] private bool restoreOnAwake = true;
        [SerializeField] private Offset offset;
        [SerializeField] private Offset tempOffset;

        [Serializable]
        internal struct Offset
        {
            [SerializeField] public float left;
            [SerializeField] public float top;
            [SerializeField] public float right;
            [SerializeField] public float bottom;
        }


        protected void Awake()
        {
            if (restoreOnAwake)
            {
                Restore();
            }
        }

        [ContextMenu("Restore")]
        public void Restore()
        {
            UndoEx.RecordObject(transform, "restore offset min max");

            var rtf = (RectTransform)transform;
            rtf.offsetMin = new Vector2(offset.left, offset.bottom);
            rtf.offsetMax = new Vector2(offset.right, offset.top);
        }

        [ContextMenu("RestoreTempOffset")]
        public void RestoreTempOffset()
        {
            UndoEx.RecordObject(transform, "restore offset min max");

            var rtf = (RectTransform)transform;
            rtf.offsetMin = new Vector2(tempOffset.left, tempOffset.bottom);
            rtf.offsetMax = new Vector2(tempOffset.right, tempOffset.top);
        }

        [ContextMenu("Save")]
        public void Save()
        {
            UndoEx.RecordObject(this, "save offset min max");

            var rtf = (RectTransform)transform;
            offset.left = rtf.offsetMin.x;
            offset.bottom = rtf.offsetMin.y;
            offset.right = rtf.offsetMax.x;
            offset.top = rtf.offsetMax.y;
        }

        [ContextMenu("SaveTempOffset")]
        private void SaveTempOffset()
        {
            UndoEx.RecordObject(this, "save temp offset min max");

            var rtf = (RectTransform)transform;
            tempOffset.left = rtf.offsetMin.x;
            tempOffset.bottom = rtf.offsetMin.y;
            tempOffset.right = rtf.offsetMax.x;
            tempOffset.top = rtf.offsetMax.y;
        }
    }
}