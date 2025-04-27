using System;
using Santutu.Core.Extensions.Runtime.UnityExtensions;
using UnityEngine;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.StageSystem;

namespace sea_survival.Scripts.StageSystem.Stages
{
    public class Lv5 : IStageLevel
    {
        public void SetupEnemies(BattleStage battleStage)
        {
            var boss = battleStage.bossPrefab.Instantiate().GetComponent<Boss>();
            boss.transform.position = Player.Ins.transform.position + new Vector3(-5, 0, 0);
            Debug.Log("스테이지 5: 보스 상어 등장 ");

            boss.onDeath.AddListener(() => { StageManager.Ins.BossDefeated(); }
            );
        }
    }
}