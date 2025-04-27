using System;
using Cysharp.Threading.Tasks;
using R3;
using Santutu.Modules.MultiScenes.Runtime.Transitions;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Santutu.Modules.MultiScenes.Runtime.Utils
{
    public class SwitchSceneButton : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Required]
#endif
        [SerializeField] private SceneCluster targetScene;

        [SerializeField] private Button button;

        protected void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void Start()
        {
            button.OnClickAsObservable()
                  .Subscribe(_ => {
                           if (gameObject.TryGetComponent<ISceneLoadingTransitionable>(out var sceneLoadingTransitionable))
                           {
                               MultiSceneManager.Instance.SwitchScene(targetScene, sceneLoadingTransitionable).Forget();
                           }
                           else
                           {
                               MultiSceneManager.Instance.SwitchScene(targetScene).Forget();
                           }
                       }
                   )
                  .AddTo(this);
        }
    }
}