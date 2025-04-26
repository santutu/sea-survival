using UnityEngine;

namespace Santutu.Core.Base.Runtime.Singletons
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T _ins;

        public static T Ins => Instance;

        public static T Instance
        {
            get
            {
                if (_ins == null)
                {
                    _ins = FindFirstObjectByType<T>(FindObjectsInactive.Include);
                }

                return _ins;
            }
        }

        protected virtual void Awake()
        {
            if (_ins == null)
            {
                _ins = (T)this;
            }
        }
    }
}