using Santutu.Core.Extensions.Runtime.UnityStaticExtensions;
using Santutu.Modules.MultiScenes.Runtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace sea_survival.Scripts
{
    public class GameStartButton : MonoBehaviour
    {
        [SerializeField] private SceneCluster inGameScene;


        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(LoadScene);
        }

        private async void LoadScene()
        {
            await MultiSceneManager.Ins.SwitchScene(inGameScene);
            GameManager.Ins.initializeGame = true;
        }
    }
}