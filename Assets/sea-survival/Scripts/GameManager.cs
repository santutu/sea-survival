using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Singletons;
using Santutu.Core.Extensions.Runtime.UnityExtensions;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] private GameObject mainUI;
        [SerializeField] private GameObject inGameUI;

        [SerializeField] private bool initializeGame = false;

        [SerializeField] private Texture2D defaultCursor;


        [SerializeField] private List<GameObject> warmUpPrefabs = new();

        [SerializeField] private GameObject warmUpPoint;

        private void Start()
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);

            if (initializeGame)
            {
                EnemyAllSpawners.Ins.enabled = false;
                Player.Ins.enabled = false;
                mainUI.SetActive(true);
                inGameUI.SetActive(false);
            }

            WarmUp().Forget();
        }

        public void StartGame()
        {
            mainUI.SetActive(false);
            inGameUI.SetActive(true);
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