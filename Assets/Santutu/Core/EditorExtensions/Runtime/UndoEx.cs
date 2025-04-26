
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Santutu.Editors.EditorExtensions.Runtime
{
    public static class UndoEx
    {
        public static void RecordObject(Object objectToUndo, string name)
        {
#if UNITY_EDITOR

            Undo.RecordObject(objectToUndo, name);
#endif
        }
        
        public static void RecordObjects(Object[] objectsToUndo, string name)
        {
#if UNITY_EDITOR

            Undo.RecordObjects(objectsToUndo, name);
#endif
        }

        public static void RegisterCreatedObjectUndo(Object objectToUndo, string name)
        {
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(objectToUndo, name);
#endif
        }
    }
}