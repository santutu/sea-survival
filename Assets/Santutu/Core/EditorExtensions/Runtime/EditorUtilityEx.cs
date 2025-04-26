using UnityEngine;

namespace Santutu.Editors.EditorExtensions.Runtime
{
    public static class EditorUtilityEx
    {
        public static void SetDirty(Object target)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
#endif
        }
    }
}