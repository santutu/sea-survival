using System;
using Santutu.Core.Extensions.Runtime.UnityExtensions;
using UnityEngine;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.StageSystem;
using sea_survival.Assets.sea_survival.Scripts.Enemies;

namespace sea_survival.Scripts.StageSystem.Stages
{
    public class Lv5 : IStageLevel
    {
        public void SetupEnemies(BattleStageState battleStage)
        {
            var boss = battleStage.bossPrefab.Instantiate().GetComponent<Boss>();
            GameManager.Ins.ClearAllPortal();
            boss.transform.position = Player.Ins.transform.position + new Vector3(10, 0, 0);
            Debug.Log("스테이지 5: 보스 상어 등장 ");
        }
    }
}