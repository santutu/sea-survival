using System;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Assets.sea_survival.Scripts.Enemies;
using sea_survival.Scripts.StageSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace sea_survival.Scripts.UI
{
    public class GameManual : SingletonMonoBehaviour<GameManual>, IPointerDownHandler
    {
        private void Start()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GameStart();
        }

        private void Update()
        {
            if (Input.anyKey)
            {
                GameStart();
            }
        }


        public void GameStart()
        {
            this.gameObject.SetActive(false);

            FallingCinematicManager.Ins.StartCinematic();
            Time.timeScale = 1;
        }
    }
}