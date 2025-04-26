using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santutu.Core.Audio.Runtime.Extensions
{
    public static class ExtendAudioSourceForAudio
    {
        public static async UniTask PlayAsync(this AudioSource source, Audio audio, CancellationToken ct = default)
        {
            await audio.PlayAsync(source, ct);
        }
    }
}