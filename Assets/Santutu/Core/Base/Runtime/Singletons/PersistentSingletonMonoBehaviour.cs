using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santutu.Core.Base.Runtime.Singletons
{
    public class PersistentSingletonMonoBehaviour<T> : MonoBehaviour where T : PersistentSingletonMonoBehaviour<T>
    {
        private static GameObject _container;

        private static T _ins;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_ins == null)
                {
                    foreach (var rootGo in Enumerable.Range(0, SceneManager.sceneCount)
                                                     .Select(SceneManager.GetSceneAt)
                                                     .SelectMany(scene => scene.GetRootGameObjects()))
                    {
                        var found = rootGo.GetComponentInChildren<T>();
                        if (found != null)
                        {
                            _ins = found;
                            break;
                        }
                    }
                }
#endif

                if (_ins == null)
                {
                    new GameObject(typeof(T).Name).AddComponent<T>();
                }

                return _ins;
            }
        }

        protected virtual void Awake()
        {
            _ins = (T)this;
            DontDestroyOnLoad(gameObject);
        }
    }
}