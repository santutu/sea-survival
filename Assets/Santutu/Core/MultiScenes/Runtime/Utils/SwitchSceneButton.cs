using System;
using R3;
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
                  .Subscribe(_ => { MultiSceneManager.Instance.SwitchScene(gameObject, targetScene); }
                   )
                  .AddTo(this);
        }
    }
}