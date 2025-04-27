using Santutu.Core.Extensions.Runtime.UnityStaticExtensions;
using Santutu.Core.Runtime.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santutu.Modules.MultiScenes.Runtime.Utils
{
    [DefaultExecutionOrder(ExecutionOrder.LoadFromRootScene)]
    public class LoadSceneFromRootScene : MonoBehaviour
    {
        [SerializeField] private SceneCluster targetScene;
        [SerializeField] public bool isEnable = true;

        private static bool _isLoaded = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _isLoaded = false;
        }

        protected void Awake()
        {
            if (!isEnable)
            {
                return;
            }

#if UNITY_EDITOR
            if (_isLoaded)
            {
                return;
            }

            _isLoaded = true;

            var activeScene = SceneManager.GetActiveScene();
            var rootScene = SceneManagerEx.GetRootScene();

            if (rootScene.name != MultiSceneManager.ROOT_SCENE_NAME)
            {
                MultiSceneManager.InitializeFromAnotherScene(targetScene);
                foreach (var go in activeScene.GetRootGameObjects())
                {
                    go.SetActive(false);
                }
            }

#endif
        }
    }
}