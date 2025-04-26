using UnityEngine;

namespace Santutu.Editors.EditorExtensions.Runtime
{
    public static class HandlesEx
    {
        public static void Label(Vector3 position, string text, Color color, int fontSize)
        {
#if UNITY_EDITOR
            GUIStyle style = new GUIStyle();
            style.fontSize = fontSize;
            style.normal.textColor = color;
            UnityEditor.Handles.Label(position, text, style);
#endif
        }

        public static void Label(Vector3 position, string text, Color color)
        {
#if UNITY_EDITOR
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            UnityEditor.Handles.Label(position, text, style);
#endif
        }


        public static void Label(Vector3 position, string text)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.Label(position, text);
#endif
        }

        public static void Label(string text, Vector3 position)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.Label(position, text);
#endif
        }
    }
}