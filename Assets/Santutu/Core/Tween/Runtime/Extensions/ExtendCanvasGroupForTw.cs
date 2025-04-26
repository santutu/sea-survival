using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santutu.Core.Tween.Runtime.Extensions
{
    public static class ExtendCanvasGroupForTw
    {
        public static async UniTask TwAlpha(this CanvasGroup canvasGroup, float alpha, float duration, CancellationToken cancellationToken = default)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                cancellationToken.ThrowIfCancellationRequested();
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, alpha, t);
                await UniTask.Yield(cancellationToken);
            }

            canvasGroup.alpha = alpha;
        }

    }
}