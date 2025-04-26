using UnityEngine.Events;

namespace Santutu.Editors.EditorExtensions.Runtime
{
    public static class UnityEventToolsEx
    {
        public static void AddPersistentListener(UnityEvent unityEvent, UnityAction call)
        {
#if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, call);

#endif            
        }
        public static void AddPersistentListener<T0>(UnityEvent<T0> unityEvent, UnityAction<T0> call)
        {
#if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, call);

#endif
        }
    }
}