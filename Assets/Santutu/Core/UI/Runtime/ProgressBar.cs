using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Santutu.Core.Extensions.Runtime.CSharpExtensions;
using Santutu.Core.Extensions.Runtime.Helpers;
using Santutu.Core.Extensions.Runtime.UnityExtensions;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Santutu.Modules.UI.Runtime
{
    public class ProgressBar : MonoBehaviour
    {
        private enum DurationOrSpeed
        {
            Duration = 0,
            Speed = 1
        }

        [SerializeField] private Image fillImage;

        [SerializeField] private Gradient colorGradient;

        [SerializeField] private DurationOrSpeed durationOrSpeed = DurationOrSpeed.Duration;
        [SerializeField] public float durationOrSpeedValue = 0.3f;
        
        public float Progress => fillImage.fillAmount;

        private CancellationTokenSource _cts;

        public UnityEvent<float> onProgress = new();
        public UnityEvent<float> onCompleted = new();


        public void SetProgress(float value)
        {
            fillImage.fillAmount = value;
        }

        public void SetProgressSmoothly(float progress)
        {
            if (durationOrSpeed == DurationOrSpeed.Duration)
            {
                SetProgressSmoothlyByDuration(progress);
            }
            else if (durationOrSpeed == DurationOrSpeed.Speed)
            {
                SetProgressSmoothlyBySpeed(progress);
            }
        }

        private async void SetProgressSmoothlyByDuration(float progress)
        {
            try
            {
                TaskHelper.RefreshToken(ref _cts);
                var ct = _cts.Token;

                var duration = durationOrSpeedValue;

                float elapsed = 0f;

                float startValue = Progress;

                while (elapsed <= 1)
                {
                    elapsed += Time.deltaTime / duration;
                    var newProgress = Mathf.Lerp(startValue, progress, elapsed);

                    SetProgress(newProgress);
                    ApplyColorFromColorGradient();
                    onProgress?.SafeInvoke(fillImage.fillAmount);
                    await UniTask.NextFrame(ct);
                }

                SetProgress(progress);
                ApplyColorFromColorGradient();
                onProgress?.SafeInvoke(progress);
                onCompleted?.SafeInvoke(progress);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async void SetProgressSmoothlyBySpeed(float progress)
        {
            try
            {
                TaskHelper.RefreshToken(ref _cts);
                var ct = _cts.Token;

                var speed = durationOrSpeedValue;
                float time = 0;
                float initialProgress = fillImage.fillAmount;

                while (time < 1)
                {
                    fillImage.fillAmount = Mathf.Lerp(initialProgress, progress, time);
                    time += Time.deltaTime * speed;

                    ApplyColorFromColorGradient();
                    onProgress?.SafeInvoke(fillImage.fillAmount);
                    await UniTask.Yield(cancellationToken: ct);
                }

                fillImage.fillAmount = progress;

                ApplyColorFromColorGradient();
                onProgress?.SafeInvoke(progress);
                onCompleted?.SafeInvoke(progress);
            }
            catch (OperationCanceledException)
            {
            }
            catch (ExitGUIException e)
            {
                Debug.LogException(e);
            }
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        private void ApplyColorFromColorGradient()
        {
            fillImage.color = colorGradient.Evaluate(fillImage.fillAmount);
        }


        private void OnDestroy()
        {
            _cts?.CancelDispose();
        }
    }
}