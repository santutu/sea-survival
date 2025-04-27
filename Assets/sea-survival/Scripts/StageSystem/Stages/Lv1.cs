using sea_survival.Scripts.Enemies;
using UnityEngine;

namespace sea_survival.Scripts.StageSystem.Stages
{
    public class Lv1 : IStageLevel
    {
        public void SetupEnemies(BattleStage battleStage)
        {
            Debug.Log("스테이지 1: 기본 물고기 등장");
            EnemyAllSpawners.Ins.spawner[0].gameObject.SetActive(true);
        }
    }
}