using System;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendAction
    {
        public static void SafeInvoke<T>(this Action<T> action, T arg)
        {
            try
            {
                action?.Invoke(arg);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}