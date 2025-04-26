using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Santutu.Core.Base.Runtime.Pool
{
    public class ManagedComponentPool<T> : ManagedObjectPool<T> where T : Component
    {
        public ManagedComponentPool(
            Transform parent,
            GameObject prefab,
            Func<T, T> create = null,
            Action<T> actionOnGet = null,
            Action<T> actionOnReturn = null,
            Action<T> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10
        ) : base(
            () => {
                var comp = Object.Instantiate(prefab).GetComponent<T>();
                comp.transform.SetParent(parent);
                comp.gameObject.SetActive(false);

                if (create != null)
                {
                    return create?.Invoke(comp);
                }

                return comp;
            },
            comp => {
                comp.gameObject.SetActive(true);
                actionOnGet?.Invoke(comp);
            },
            comp => {
                comp.gameObject.SetActive(false);
                actionOnReturn?.Invoke(comp);
            },
            actionOnDestroy,
            collectionCheck,
            defaultCapacity
        )
        {
        }
    }
}