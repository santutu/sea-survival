using System;

namespace Santutu.Core.Base.Runtime.DataStructures
{
    [Serializable]
    public struct RangeValue
    {
        public int min;
        public int max;

        public RangeValue(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public int Random()
        {
            return UnityEngine.Random.Range(min, max + 1);
        }
    }
}