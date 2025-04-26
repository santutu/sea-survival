using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Santutu.Core.Tween.Runtime.Extensions
{
    public static class ExtendImageForTw
    {
        public static async UniTask TwAlpha(this Image image, float alpha, float duration, CancellationToken cancellationToken = default)
        {
            float startAlpha = image.color.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                cancellationToken.ThrowIfCancellationRequested();

                elapsedTime += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

                Color color = image.color;
                color.a = Mathf.Lerp(startAlpha, alpha, normalizedTime);
                image.color = color;

                await UniTask.Yield(cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                Color finalColor = image.color;
                finalColor.a = alpha;
                image.color = finalColor;
            }
        }
    }
}