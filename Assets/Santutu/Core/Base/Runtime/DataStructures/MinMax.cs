using System;
using UnityEngine;
using UnityEngine.Serialization;
 
namespace Santutu.Core.Base.Runtime.DataStructures
{
    [Serializable]
    public struct MinMax
    {
        [SerializeField, FormerlySerializedAs("Min")]
        public int min;

        [SerializeField, FormerlySerializedAs("Max")]
        public int max;

        public MinMax(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public bool Contain(int value)
        {
            return min <= value && value <= max;
        }

        public int Average()
        {
            return (min + max) / 2;
        }

        public int Random()
        {
            return UnityEngine.Random.Range(min, max);
        }


        public void Deconstruct(out int min, out int max)
        {
            min = this.min;
            max = this.max;
        }
    }
}