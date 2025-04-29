using sea_survival.Scripts.Enemies;
using UnityEngine;

namespace sea_survival.Scripts.StageSystem.Stages
{
    public class Lv2 : IStageLevel
    {
        public void SetupEnemies(BattleStageState battleStage)
        {
            Debug.Log("스테이지 2: 강화된 물고기 등장");
            EnemyAllSpawners.Ins.spawner[1].gameObject.SetActive(true);
        }
    }
}