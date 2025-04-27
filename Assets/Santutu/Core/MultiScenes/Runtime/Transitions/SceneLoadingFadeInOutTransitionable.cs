using System;
using Cysharp.Threading.Tasks;
using Santutu.Core.Extensions.Runtime.UnityStaticExtensions;
using Santutu.Core.GameObjectTraveler.Runtime;
using UnityEngine;

namespace Santutu.Modules.MultiScenes.Runtime.Transitions
{
    public class SceneLoadingFadeInOutTransitionable : MonoBehaviour, ISceneLoadingTransitionable
    {
        [SerializeField] private float duration;


        public async UniTask<ISceneTransition> LoadSceneAndGetTransition()
        {
            var scene = await SceneManagerEx.LoadAdditiveSceneAsync("fadeInOut transition");
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                if (rootGameObject.TryGetComponentInSelfBelow<FadeInOutSceneTransition>(out var fadeInOut))
                {
                    fadeInOut.duration = duration;
                    return fadeInOut;
                }
            }

            Debug.LogError("not found fadein out");
            throw new Exception();
        }

        public async UniTask UnloadScene()
        {
            await SceneManagerEx.UnloadSceneAsync("fadeInOut transition");
        }
    }
}