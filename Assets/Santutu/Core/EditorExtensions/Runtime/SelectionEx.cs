using UnityEngine;

namespace Santutu.Editors.EditorExtensions.Runtime
{
    public static class SelectionEx
    {
        public static Object activeObject
        {
            get
            {
#if UNITY_EDITOR


                return UnityEditor.Selection.activeObject;
#else
                return null;
#endif
            }
            set
            {
#if UNITY_EDITOR
                UnityEditor.Selection.activeObject = value;
#endif
            }
        }
    }
}