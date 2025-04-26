using System.Threading;
using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Pool;
using UnityEngine;

namespace Santutu.Core.Audio.Runtime
{
    public class AudioSourcePool : MonoBehaviour
    {
        private ManagedComponentPool<AudioSource> _pool;
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private int defaultCapacity = 10;

        protected void Awake()
        {
            if (audioSourcePrefab == null)
            {
                var audioSourcePrefabGo = new GameObject("AudioSource");
                audioSourcePrefab = audioSourcePrefabGo.AddComponent<AudioSource>();
            }

            _pool = new ManagedComponentPool<AudioSource>(
                parent: transform,
                prefab: audioSourcePrefab.gameObject,
                create: null,
                actionOnGet: null,
                actionOnReturn: null,
                actionOnDestroy: null,
                collectionCheck: true,
                defaultCapacity: defaultCapacity
            );
        }

        public async UniTask PlayAsync(Audio audio, Transform parent, CancellationToken ct = default)
        {
            if (audio == null || audio.clip == null)
            {
                return;
            }

            AudioSource audioSource = _pool.Get();
            audioSource.transform.SetParent(parent);
            audioSource.transform.localPosition = Vector3.zero;

            try
            {
                await audio.PlayAsync(audioSource, ct);
            }
            finally
            {
                if (this != null)
                {
                    audioSource.transform.SetParent(transform);
                    _pool.Return(audioSource);
                }
            }
        }


        private void OnDestroy()
        {
            _pool?.Clear();
        }
    }
}