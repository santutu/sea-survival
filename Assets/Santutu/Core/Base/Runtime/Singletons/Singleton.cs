using System;

namespace Santutu.Core.Base.Runtime.Singletons
{
    public abstract class Singleton<T>
    {
        private static T _ins;

        public static T Instance
        {
            get
            {
                if (_ins == null)
                {
                    _ins = Activator.CreateInstance<T>();
                }

                return _ins;
            }
        }
    }
}