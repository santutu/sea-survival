﻿using System.Collections.Generic;

namespace Santutu.Core.DynamicArray.Runtime
{
    public interface IReadOnlyCovariantDynamicArray<out T> : IEnumerable<T>, IReadOnlyList<T>
    {
        public int Count { get; }
        public int Length { get; set; }
        public int Capacity { get; }
        public T this[int index] { get; }
    }
}