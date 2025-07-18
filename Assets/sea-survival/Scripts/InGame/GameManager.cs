﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Singletons;
using Santutu.Core.Extensions.Runtime.UnityExtensions;
using Santutu.Core.Extensions.Runtime.UnityStaticExtensions;
using sea_survival.Assets.sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.StageSystem;
using sea_survival.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace sea_survival.Scripts
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] public bool initializeGame = false;

        [SerializeField] private Texture2D defaultCursor;


        [SerializeField] private List<GameObject> warmUpPrefabs = new();

        [SerializeField] private GameObject warmUpPoint;


        private void Start()
        {
            Time.timeScale = 1;
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            WarmUp().Forget();

            if (initializeGame)
            {
                StartGame();
            }
        }


        private void StartGame()
        {
            StageManager.Ins.gameObject.SetActive(false);
            WaterCurrentManager.Ins.gameObject.SetActive(false);
            GameManual.Ins.gameObject.SetActive(true);
            Time.timeScale = 0;
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

        public void ClearAllPortal()
        {
            foreach (var portal in SceneManagerEx.ActiveScene.GetAllComponents<Portal>())
            {
                portal.gameObject.DestroySelf();
            }
        }

        public void ClearAllEnemiesAndExp()
        {
            ClearAllEnemies();
            foreach (var exp in SceneManagerEx.ActiveScene.GetAllComponents<Exp>())
            {
                exp.gameObject.DestroySelf();
            }
        }

        public void ClearAllEnemies()
        {
            foreach (var enemy in SceneManagerEx.ActiveScene.GetAllComponents<Enemy>())
            {
                enemy.gameObject.DestroySelf();
            }
        }
    }
}