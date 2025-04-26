using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityStaticExtensions
{
    public static class InputEx
    {
        public static async UniTask WaitForMouseButtonUp(int button, CancellationToken ct = default)
        {
            while (true)
            {
                if (Input.GetMouseButtonUp(button))
                {
                    return;
                }

                await UniTask.NextFrame(cancellationToken: ct);
            }
        }

        public static bool TryGetWorldPosition(out Vector3 position, float maxDistance, int layer = -1)
        {
            return PhysicsEx.TryGetWorldMousePosition(out position, maxDistance, layer);
        }
    }
}