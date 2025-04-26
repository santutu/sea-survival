using System.IO;
using System.Reflection;
using Santutu.Core.Base.Runtime.Constants;
using Santutu.Core.Base.Runtime.Helpers;
using UnityEngine;

namespace Santutu.Core.Base.Runtime.Singletons
{
    public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T _ins;

        public static T Instance
        {
            get
            {
                if (_ins == null)
                {
                    string loadPath = $"Santutu/Config/{typeof(T).Name}_auto_naming";
                    string savePath = Path.Join(Paths.ResourcesDirPath, loadPath) + ".asset";

                    var path = typeof(T).GetCustomAttribute<SingletonResourcePathAttribute>();

                    if (path != null)
                    {
                        loadPath = path.LoadPath;
                        savePath = path.SavePath;
                    }


                    _ins = Resources.Load<T>(loadPath);
                    if (_ins == null)
                    {
#if UNITY_EDITOR
                        _ins = AssetHelper.LoadOrCreateAsset<T>(savePath);
#else
                        Debug.LogError($"Not found {typeof(T)} as singleton scriptable object in {loadPath}");
#endif
                    }
                }


                return _ins;
            }
        }
    }
}