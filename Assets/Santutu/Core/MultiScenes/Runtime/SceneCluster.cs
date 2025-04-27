using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Constants;
using Santutu.Core.Base.Runtime.References;
using Santutu.Core.Extensions.Runtime.UnityStaticExtensions;
using Santutu.Modules.MultiScenes.Runtime.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Santutu.Modules.MultiScenes.Runtime
{
    [Icon(Paths.EditorIconPath + "/Black Unity Icon.png")]
    [CreateAssetMenu(fileName = "scene cluster", menuName = "Santutu/Scene Cluster", order = 0)]
    public class SceneCluster : ScriptableObject
    {
        public SceneReference[] subScenes;

        public SceneReference activeScene;

        [SerializeField] private UnityEvent onExit = new();
        [SerializeField] private UnityEvent onLoaded = new();

        public virtual void OnExit()
        {
            onExit?.Invoke();
        }

        public void OnLoaded()
        {
            onLoaded?.Invoke();
        }


        public (List<ObservableProgress<float>> progresses, UniTask task) CreateSubScenesLoadTask(
        )
        {
            var progresses = subScenes.Select(_ => new ObservableProgress<float>()).ToList();

            var tasks = subScenes.Select(
                (scene, i) => SceneManagerEx.LoadAdditiveSceneAsync(scene.name, progress: progresses[i])
            );

            return (progresses, UniTask.WhenAll(tasks));
        }

        public async UniTask UnloadSubScenes()
        {
            var tasks = subScenes.Reverse()
                .Select(scene => SceneManagerEx.UnloadSceneAsync(scene.name));
            await UniTask.WhenAll(tasks);
        }
    }
}