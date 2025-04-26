using System.Threading;
using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Constants;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Santutu.Core.Audio.Runtime
{
    [Icon(Paths.EditorIconPath + "/sound icon.png")]
    [CreateAssetMenu(fileName = "audio", menuName = "Santutu/Audio", order = 0)]
    public class Audio : ScriptableObject
    {
        public AudioClip clip;

        public float volume = 1;
        public float startTime = 0;
        public float playDuration = 0;
        public float pitch = 1;
        public float fadeout = 0;


        public async UniTask PlayAsync(AudioSource source, CancellationToken ct = default)
        {
            if (clip == null)
                return;

            source.clip = clip;
            float originalPitch = source.pitch;
            source.pitch = pitch;

            float duration;

            source.time = startTime;
            source.volume = volume;
            source.Play();

            if (playDuration <= 0)
            {
                duration = clip.length - startTime;
            }
            else
            {
                source.SetScheduledEndTime(AudioSettings.dspTime + playDuration);
                duration = playDuration;
            }

            if (fadeout > 0 && fadeout < duration)
            {
                await UniTask.Delay((int)((duration - fadeout) * 1000), cancellationToken: ct);

                await ApplyFadeOutAsync(source, ct);
            }
            else
            {
                await UniTask.Delay((int)(duration * 1000), cancellationToken: ct);
            }

            if (playDuration > 0)
            {
                source.pitch = originalPitch;
            }
        }

        private async UniTask ApplyFadeOutAsync(AudioSource source, CancellationToken ct = default)
        {
            if (fadeout <= 0)
                return;

            float startVolume = source.volume;
            float startTime = Time.time;
            float endTime = startTime + fadeout;

            while (Time.time < endTime)
            {
                if (ct.IsCancellationRequested)
                    break;

                float t = 1 - ((endTime - Time.time) / fadeout);
                source.volume = Mathf.Lerp(startVolume, 0, t);

                await UniTask.Yield();
            }

            source.volume = 0;
        }

        public static implicit operator AudioClip(Audio audio)
        {
            return audio.clip;
        }
#if ODIN_INSPECTOR
        [Button]
#endif
        public async UniTask PlayForTest()
        {
            GameObject tempGO = new GameObject("TempAudioPlayer");
            AudioSource audioSource = tempGO.AddComponent<AudioSource>();

            await UniTask.DelayFrame(1);
            try
            {
                await PlayAsync(audioSource);
            }
            finally
            {
                Object.DestroyImmediate(tempGO);
            }
        }
    }
}