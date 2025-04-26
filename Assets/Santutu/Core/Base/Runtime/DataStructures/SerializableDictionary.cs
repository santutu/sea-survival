using System;
using System.Collections.Generic;
using UnityEngine;

namespace Santutu.Core.Base.Runtime.DataStructures
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] public List<SerializableKeyValuePair<TKey, TValue>> list = new();

        public void OnBeforeSerialize()
        {
            list.RemoveAll(el => el.key is not null && !ContainsKey(el.key));

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                var index = list.FindIndex(el => el.key.Equals(pair.Key));

                var hasKey = index >= 0;

                if (!hasKey)
                {
                    list.Add(new SerializableKeyValuePair<TKey, TValue>(pair.Key, pair.Value));
                }
                else if (hasKey && list[index].value != null && !list[index].value.Equals(pair.Value))
                {
                    list[index].value = pair.Value;
                }
            }
        }


        public void OnAfterDeserialize()
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            Clear();

            var uniqueKeys = new HashSet<TKey>();

            foreach (var keyValue in list)
            {
                if (keyValue.key != null && uniqueKeys.Add(keyValue.key))
                {
                    Add(keyValue.key, keyValue.value);
                }
            }
        }


        public SerializableDictionary() : base()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
            OnBeforeSerialize();
        }
    }

    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue>
    {
        [SerializeField] public TKey key;
        [SerializeField] public TValue value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
}