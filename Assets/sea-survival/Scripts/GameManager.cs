using System;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] private GameObject mainUI;
        [SerializeField] private GameObject inGameUI;

        [SerializeField] private bool initializeGame = false;

        [SerializeField] private Texture2D defaultCursor;

        private void Start()
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);

            if (initializeGame)
            {
                EnemySpawner.Ins.enabled = false;
                Player.Ins.enabled = false;
                StartPlayer.Ins.enabled = false;
                mainUI.SetActive(true);
                inGameUI.SetActive(false);
            }
        }

        public void StartGame()
        {
            mainUI.SetActive(false);
            inGameUI.SetActive(true);
            StartPlayer.Ins.enabled = true;
        }

        public void EndGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}