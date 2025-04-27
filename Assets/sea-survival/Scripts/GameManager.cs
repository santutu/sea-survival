using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Singletons;
using Santutu.Core.Extensions.Runtime.UnityExtensions;
using Santutu.Core.Extensions.Runtime.UnityStaticExtensions;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.StageSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace sea_survival.Scripts
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [FormerlySerializedAs("mainUI")] [SerializeField]
        private GameObject main;

        [FormerlySerializedAs("inGameUI")] [SerializeField]
        private GameObject inGamePrefab;

        [SerializeField] private GameObject inGame;

        [SerializeField] private bool initializeGame = false;

        [SerializeField] private Texture2D defaultCursor;


        [SerializeField] private List<GameObject> warmUpPrefabs = new();

        [SerializeField] private GameObject warmUpPoint;


        private void Start()
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);

            if (initializeGame)
            {
                ToMainMenu();
            }

            WarmUp().Forget();
        }

        public void ToMainMenu()
        {
            Time.timeScale = 1;
            foreach (var rootGameObject in SceneManagerEx.ActiveScene.GetRootGameObjects())
            {
                if (rootGameObject.name == "Core")
                {
                    continue;
                }

                rootGameObject.DestroySelf();
            }

            inGame = inGamePrefab.Instantiate();
            inGame.transform.position = Vector3.zero;

            StageManager.Ins.gameObject.SetActive(false);
            Player.Ins.enabled = false;
            main.SetActive(true);
            inGame.SetActive(false);


            //re create todo prefab
        }

        public void StartGame()
        {
            StageManager.Ins.gameObject.SetActive(true);
            main.SetActive(false);
            inGame.SetActive(true);

            FallingCinematicManager.Ins.StartCinematic();
        }

        public void EndGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private async UniTask WarmUp()
        {
            foreach (var prefab in warmUpPrefabs)
            {
                var newGo = prefab.Instantiate();
                newGo.transform.SetParent(warmUpPoint.transform);
                newGo.transform.position = warmUpPoint.transform.position;
            }

            await UniTask.DelayFrame(5);
            warmUpPoint.DestroySelf();
        }
    }
}