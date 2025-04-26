using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendAudioSource
    {
        public static async UniTask PlayAsync(
            this AudioSource source,
            AudioClip clip,
            CancellationToken cancellationToken = default
        )

        {
            source.clip = clip;

            source.Play();
            await UniTask.Delay(
                (int)(1000 * (clip.length - source.time)),
                cancellationToken: cancellationToken
            );
        }
    }
}