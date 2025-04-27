using Cysharp.Threading.Tasks;
using Santutu.Core.Extensions.Runtime.UnityExtensions;
using Santutu.Core.Tween.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Santutu.Modules.MultiScenes.Runtime.Transitions
{
    public class FadeInOutSceneTransition : MonoBehaviour, ISceneTransition
    {
        [SerializeField] private Image image;

        [SerializeField] public float duration;

        private void Awake()
        {
            if (image == null)
            {
                image = this.GetComponent<Image>();
            }
        }


        public async UniTask StartIn()
        {
            image.SetAlpha(0);
            await image.TwAlpha(1f, duration);
        }

        public async UniTask StartOut()
        {
            image.SetAlpha(1);
            await image.TwAlpha(0f, duration);
        }

        public async UniTask In()
        {
            image.SetAlpha(0);
            await image.TwAlpha(1f, duration);
        }

        public async UniTask Out()
        {
            image.SetAlpha(1);
            await image.TwAlpha(0f, duration);
        }
    }
}