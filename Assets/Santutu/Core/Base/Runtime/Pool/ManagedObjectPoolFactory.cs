using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Santutu.Core.Base.Runtime.Pool
{
    public static class ManagedObjectPool
    {
        public static ManagedObjectPool<TComp> CreateGameObjectPool<TComp>(
            GameObject prefab,
            Func<GameObject, TComp> getComponent = null,
            Action<TComp> actionOnCreate = null,
            Action<TComp> actionOnGet = null,
            Action<TComp> actionOnReturn = null,
            Action<TComp> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10
        ) where TComp : Component
        {
            if (getComponent == null)
            {
                getComponent = (go) => {
                    if (!go.TryGetComponent<TComp>(out var comp))
                    {
                        comp = go.AddComponent<TComp>();
                    }

                    return comp;
                };
            }

            return new ManagedObjectPool<TComp>(
                () => {
                    var newGo = Object.Instantiate(prefab);
                    newGo.SetActive(false);

                    var comp = getComponent(newGo);

                    actionOnCreate?.Invoke(comp);

                    return comp;
                },
                actionOnGet: comp => {
                    comp.gameObject.SetActive(true);
                    actionOnGet?.Invoke(comp);
                },
                actionOnReturn: comp => {
                    comp.gameObject.SetActive(false);
                    actionOnReturn?.Invoke(comp);
                },
                actionOnDestroy: comp => {
                    Object.Destroy(comp.gameObject);
                    actionOnDestroy?.Invoke(comp);
                },
                collectionCheck,
                defaultCapacity
            );
        }
    }
}