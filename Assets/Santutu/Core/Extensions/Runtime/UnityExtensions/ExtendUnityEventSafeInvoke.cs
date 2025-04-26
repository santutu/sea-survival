using System;
using UnityEngine;
using UnityEngine.Events;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendUnityEventSafeInvoke
    {
        public static void SafeInvoke(this UnityEvent unityEvent)
        {
            try
            {
                unityEvent?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        
        public static void SafeInvoke<T>(this UnityEvent<T> unityEvent, T evt)
        {
            try
            {
                unityEvent?.Invoke(evt);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}