using System;

namespace Santutu.Core.Base.Runtime.DataStructures
{
    [Serializable]
    public struct MinMaxf
    {
        public float min;
        public float max;

        public MinMaxf(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public bool Contain(float value)
        {
            return min <= value && value <= max;
        }

        public float Average()
        {
            return (min + max) / 2;
        }

        public float Random()
        {
            return UnityEngine.Random.Range(min, max);
        }

        public void Deconstruct(out float min, out float max)
        {
            min = this.min;
            max = this.max;
        }
    }
}