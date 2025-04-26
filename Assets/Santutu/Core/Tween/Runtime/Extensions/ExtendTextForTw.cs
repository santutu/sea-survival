using System.Threading;
using Cysharp.Threading.Tasks;
using Santutu.Core.Tween.Runtime.Enums;
using TMPro;
using UnityEngine;

namespace Santutu.Core.Tween.Runtime.Extensions
{
    public static class ExtendTextForTw
    {
        public static async UniTask TwColor(this TMP_Text text, Color color, float duration, TimeScale timeScale, CancellationToken ct = default)
        {
            Color startColor = text.color;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                ct.ThrowIfCancellationRequested();
                float t = elapsedTime / duration;
                text.color = Color.Lerp(startColor, color, t);

                switch (timeScale)
                {
                    case TimeScale.Scaled:
                        await UniTask.Yield();
                        elapsedTime += Time.deltaTime;
                        break;
                    case TimeScale.Unscaled:
                        await UniTask.Yield();
                        elapsedTime += Time.unscaledDeltaTime;
                        break;
                    case TimeScale.Realtime:
                        float startTime = Time.realtimeSinceStartup;
                        await UniTask.Yield();
                        elapsedTime += Time.realtimeSinceStartup - startTime;
                        break;
                }
            }

            if (ct.IsCancellationRequested)
            {
                return;
            }

            text.color = color;
        }
    }
}