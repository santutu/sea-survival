using System.Reflection;
using Santutu.Core.Base.Editor.Helpers;
using UnityEngine;

namespace Santutu.Core.Base.Editor
{
    public class EditorSingletonScriptableObject<T> : ScriptableObject where T : EditorSingletonScriptableObject<T>
    {
        private static T _ins;

        public static T Instance
        {
            get
            {
                if (_ins == null)
                {
                    var path = typeof(T).GetCustomAttribute<SingletonAssetPathAttribute>();
                    _ins = AssetHelper.LoadOrCreateAsset<T>(path);
                }


                return _ins;
            }
        }
    }
}