using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Santutu.Core.Base.Runtime.WeightTables
{
    public class WeightTable<TItem>
    {
        private Func<TItem, float> _getWeight;
        private TItem[] _items;

        public WeightTable(IEnumerable<TItem> items, Func<TItem, float> getWeight)
        {
            _items = items.ToArray();
            
            if (!_items.Any()) throw new ArgumentNullException(nameof(items));

            _getWeight = getWeight;
        }

        public TItem GetOne()
        {
            float totalWeight = _items.Sum(item => Math.Max(0, _getWeight(item)));

            if (totalWeight <= 0)
            {
                return _items.FirstOrDefault();
            }

            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float currentWeight = 0;

            foreach (var item in _items)
            {
                float weight = Math.Max(0, _getWeight(item));
                if (weight <= 0) continue;

                currentWeight += weight;
                if (randomValue <= currentWeight)
                {
                    return item;
                }
            }

            return _items.LastOrDefault();
        }


        public IEnumerable<TItem> GetMany(int count)
        {
            if (count <= 0) yield break;

            for (int i = 0; i < count; i++)
            {
                yield return GetOne();
            }
        }


        public IEnumerable<TItem> GetManyUnique(int count)
        {
            if (count <= 0) yield break;

            var remainingItems = new List<TItem>(_items);
            var getWeightFunc = _getWeight;

            int itemsToReturn = Math.Min(count, remainingItems.Count);

            for (int i = 0; i < itemsToReturn; i++)
            {
                var tempTable = new WeightTable<TItem>(remainingItems.ToArray(), getWeightFunc);
                var selected = tempTable.GetOne();

                yield return selected;
                remainingItems.Remove(selected);

                if (!remainingItems.Any()) break;
            }
        }
    }
}