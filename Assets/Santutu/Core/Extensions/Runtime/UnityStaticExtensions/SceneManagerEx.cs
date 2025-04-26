using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Santutu.Core.Extensions.Runtime.UnityExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santutu.Core.Extensions.Runtime.UnityStaticExtensions
{
    public static class SceneManagerEx
    {
        public static Scene ActiveScene => SceneManager.GetActiveScene();

        public static IEnumerable<Scene> GetAllScenes()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                yield return SceneManager.GetSceneAt(i);
            }
        }


        public static Scene GetRootScene()
        {
            foreach (var scene in GetAllScenes())
            {
                if (!scene.isSubScene)
                {
                    return scene;
                }
            }

            throw new Exception("Not Found root scene");
        }


        public static IEnumerable<int> GetAllSceneIndex()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                yield return i;
            }
        }


        public static IEnumerable<T> GetComponentsInActiveScene<T>()
        {
            return SceneManager.GetActiveScene().GetAllComponents<T>();
        }


        public static async UniTask<Scene> LoadAdditiveSceneAsync(
            string sceneName,
            IProgress<float> progress = null,
            Action<AsyncOperation> onComplete = null
        )
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (onComplete != null)
            {
                asyncOperation.completed += onComplete;
            }

            await asyncOperation.ToUniTask(progress: progress);
            if (onComplete != null)
            {
                asyncOperation.completed -= onComplete;
            }

            return SceneManager.GetSceneByName(sceneName);
        }

        public static async UniTask<Scene> LoadAdditiveSceneAsync(
            Scene scene,
            IProgress<float> progress = null,
            Action<AsyncOperation> onComplete = null
        )
        {
            return await LoadAdditiveSceneAsync(scene.name, progress, onComplete);
        }


        public static bool SetActiveScene(Scene scene)
        {
            return SceneManager.SetActiveScene(scene);
        }

        public static bool SetActiveScene(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            return SceneManager.SetActiveScene(scene);
        }

        public static Scene GetSceneByName(string name)
        {
            return SceneManager.GetSceneByName(name);
        }

        public static async UniTask UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }

        public static async UniTask UnloadSceneAsync(Scene scene)
        {
            await SceneManager.UnloadSceneAsync(scene.name).ToUniTask();
        }
    }
}