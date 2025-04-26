using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityStaticExtensions
{
    public static class ObjectEx
    {
        public static async UniTask<T> InstantiateAsync<T>(T source, CancellationToken ct = default) where T : Object
        {
            var results = await Object.InstantiateAsync(source).ToUniTask(cancellationToken: ct);
            return results[0];
        }
    }
}