using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using R3;
using Santutu.Core.Audio.Runtime;
using Santutu.Core.Audio.Runtime.Extensions;
using Santutu.Core.Extensions.Runtime.UnityExtensions;

namespace Santutu.Modules.UI.Runtime
{
    public class ButtonSound : MonoBehaviour
    {
        [SerializeField] public Audio clickSound;
        [SerializeField, HideInInspector] private AudioSource audioSource;

        [SerializeField] private Button btn;

        private void Awake()
        {
            audioSource = this.GetOrAddComponent<AudioSource>();
        }

        void Start()
        {
            if (btn == null)
            {
                btn = GetComponent<Button>();
            }

            btn.onClick.AsObservable()
               .Subscribe(_ => { audioSource.PlayAsync(clickSound, destroyCancellationToken).Forget(); })
               .AddTo(this);
        }
    }
}