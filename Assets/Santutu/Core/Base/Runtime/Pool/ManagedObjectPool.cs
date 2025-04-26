using System;
using System.Collections.Generic;

namespace Santutu.Core.Base.Runtime.Pool
{
    public class ManagedObjectPool<T> : IDisposable where T : class
    {
        private Stack<T> _pool = new Stack<T>();
        private Stack<T> _all = new Stack<T>();

        public IReadOnlyCollection<T> Pool => _pool;
        public IReadOnlyCollection<T> All => _all;

        public int CountAll => _all.Count;
        public int CountActive => _all.Count - _pool.Count;
        public int CountInactive => _pool.Count;

        private Func<T> _create;

        private Action<T> _actionOnGet;
        private Action<T> _actionOnReturn;
        private Action<T> _actionOnDestroy;
        private bool _collectionCheck;


        public ManagedObjectPool(
            Func<T> create,
            Action<T> actionOnGet = null,
            Action<T> actionOnReturn = null,
            Action<T> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10
        )
        {
            _create = create;
            _actionOnGet = actionOnGet;
            _actionOnReturn = actionOnReturn;
            _actionOnDestroy = actionOnDestroy;
            _collectionCheck = collectionCheck;

            for (int i = 0; i < defaultCapacity; i++)
            {
                T item = create();
                _pool.Push(item);
                _all.Push(item);
            }
        }

        public T Get()
        {
            T item;
            if (_pool.Count > 0)
            {
                item = _pool.Pop();
            }
            else
            {
                item = _create();
                _all.Push(item);
            }

            _actionOnGet?.Invoke(item);
            return item;
        }

        public T[] GetMany(int count)
        {
            var arr = new T[count];

            for (var i = 0; i < count; i++)
            {
                arr[i] = Get();
            }

            return arr;
        }

        public void Return(T ele)
        {
            var contains = _pool.Contains(ele);
            if (_collectionCheck && contains)
            {
                throw new InvalidOperationException("This object is already in the pool.");
            }

            if (!_all.Contains(ele))
            {
                if (_collectionCheck)
                {
                    throw new InvalidOperationException("This object does not belong to the pool.");
                }

                _all.Push(ele);
            }

            _actionOnReturn?.Invoke(ele);
            if (!contains)
            {
                _pool.Push(ele);
            }
        }

        public void ReturnAll()
        {
            foreach (T item in _all)
            {
                if (!_pool.Contains(item))
                {
                    _actionOnReturn?.Invoke(item);
                    _pool.Push(item);
                }
            }
        }

        public bool TryReturn(Predicate<T> predicate)
        {
            foreach (T item in _all)
            {
                if (predicate(item) && !_pool.Contains(item))
                {
                    _actionOnReturn?.Invoke(item);
                    _pool.Push(item);
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            foreach (T item in _all)
            {
                _actionOnDestroy?.Invoke(item);
            }

            _pool.Clear();
            _all.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}