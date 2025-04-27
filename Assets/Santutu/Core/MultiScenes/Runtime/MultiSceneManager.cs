using System;
using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Singletons;
using Santutu.Core.Extensions.Runtime.UnityStaticExtensions;
using Santutu.Modules.MultiScenes.Runtime.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santutu.Modules.MultiScenes.Runtime
{
    public class MultiSceneManager : SingletonMonoBehaviour<MultiSceneManager>
    {
        public const string ROOT_SCENE_NAME = "Root Scene";


        public SceneCluster startScene;
        public SceneCluster loadingScene;


        protected override void Awake()
        {
            base.Awake();
            Instance.StartState().Forget();
        }


        public static bool UseForceStartScene = false;
        public static SceneCluster ForceStartScene;


        public SceneCluster CurrentScene { get; set; }
        public static bool Loaded => Instance != null;
        public static readonly ObservableProgress<float> OnSceneLoadProgressChanged = new();

        public async UniTask SwitchScene(SceneCluster targetScene)
        {
            if (!Loaded)
            {
                InitializeFromAnotherScene(targetScene);
                return;
            }

            await SwitchState(targetScene);
        }

        public async UniTask SwitchScene(SceneCluster targetScene, ISceneLoadingTransitionable sceneLoadingTransitionable)
        {
            if (!Loaded)
            {
                InitializeFromAnotherScene(targetScene);
                return;
            }

            await SwitchState(targetScene, sceneLoadingTransitionable);
        }

        public static void InitializeFromAnotherScene(SceneCluster startScene)
        {
            if (Loaded)
            {
                throw new Exception("already loaded root scene.");
            }

            UseForceStartScene = true;
            ForceStartScene = startScene;
            SceneManager.LoadScene(ROOT_SCENE_NAME);
        }


        private async UniTask StartState()
        {
            var startScene = UseForceStartScene ? ForceStartScene : this.startScene;
            // unload current scene.
            if (CurrentScene != null)
            {
                CurrentScene.OnExit();
                await CurrentScene.UnloadSubScenes();
            }

            //load next scene
            var (progresses, loadScenesTask) = startScene.CreateSubScenesLoadTask();

            OnSceneLoadProgressChanged.Report(0);
            var disposable = OnSceneLoadProgressChanged.ListenFrom(progresses);
            await loadScenesTask;
            disposable.Dispose();
            OnSceneLoadProgressChanged.Report(1);

            CurrentScene = startScene;
            SceneManagerEx.SetActiveScene(CurrentScene.activeScene.name);
            CurrentScene.OnLoaded();
        }

        private async UniTask SwitchState(
            SceneCluster nextScene,
            ISceneLoadingTransitionable fadeInOutSceneLoadingTransitionable
        )
        {
            //transition
            var transition = await fadeInOutSceneLoadingTransitionable.LoadSceneAndGetTransition();
            await transition.StartIn();

            //loading scene
            var loadSceneTask = loadingScene.CreateSubScenesLoadTask();
            await loadSceneTask.task;
            SceneManagerEx.SetActiveScene(loadingScene.activeScene.name);

            // unload current scene.
            if (CurrentScene != null)
            {
                CurrentScene.OnExit();
                await CurrentScene.UnloadSubScenes();
            }


            //transition
            await transition.StartOut();

            //load next scene
            var (progresses, loadScenesTask) = nextScene.CreateSubScenesLoadTask();

            OnSceneLoadProgressChanged.Report(0);
            var disposable = OnSceneLoadProgressChanged.ListenFrom(progresses);
            await loadScenesTask;
            disposable.Dispose();
            OnSceneLoadProgressChanged.Report(1);
            CurrentScene = nextScene;
            SceneManagerEx.SetActiveScene(nextScene.activeScene.name);

            CurrentScene.OnLoaded();

            //unload loading scene
            UnloadScene(transition, loadingScene, fadeInOutSceneLoadingTransitionable).Forget();
        }

        private async UniTask UnloadScene(ISceneTransition transition, SceneCluster scene, ISceneLoadingTransitionable fadeInOutSceneLoadingTransitionable)
        {
            await transition.In();
            await scene.UnloadSubScenes();
            await transition.Out();
            await fadeInOutSceneLoadingTransitionable.UnloadScene();
        }


        private async UniTask SwitchState(SceneCluster nextScene)
        {
            //load loading scene
            var loadingSceneTask = loadingScene.CreateSubScenesLoadTask();
            await loadingSceneTask.task;
            SceneManagerEx.SetActiveScene(loadingScene.activeScene.name);

            // unload current scene.
            if (CurrentScene != null)
            {
                CurrentScene.OnExit();
                await CurrentScene.UnloadSubScenes();
            }

            //load next scene
            var (progresses, loadScenesTask) = nextScene.CreateSubScenesLoadTask();

            OnSceneLoadProgressChanged.Report(0);
            var disposable = OnSceneLoadProgressChanged.ListenFrom(progresses);
            await loadScenesTask;
            disposable.Dispose();
            OnSceneLoadProgressChanged.Report(1);
            CurrentScene = nextScene;
            SceneManagerEx.SetActiveScene(nextScene.activeScene.name);
            CurrentScene.OnLoaded();

            //unload loading scene
            loadingScene.UnloadSubScenes().Forget();
        }
    }
}