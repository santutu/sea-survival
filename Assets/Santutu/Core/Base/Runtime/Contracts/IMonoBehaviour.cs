using System;
using System.Threading;
using UnityEngine;

namespace Santutu.Core.Base.Runtime.Contracts
{
    public interface IMonoBehaviour
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public CancellationToken destroyCancellationToken { get; }

        public bool enabled { get; }

        public bool TryGetComponent(Type type, out Component component);


        public bool TryGetComponent<T>(out T component);

        public Component GetComponent(Type type);
        public T GetComponent<T>();

        public T[] GetComponents<T>();

        public T[] GetComponentsInChildren<T>();

        public bool HasComponent<T>()
        {
            return TryGetComponent(typeof(T), out _);
        }
    }
}