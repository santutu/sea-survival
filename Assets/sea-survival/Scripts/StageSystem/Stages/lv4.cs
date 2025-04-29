using sea_survival.Assets.sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Enemies;
using UnityEngine;

namespace sea_survival.Scripts.StageSystem.Stages
{
    public class Lv4 : IStageLevel
    {
        public void SetupEnemies(BattleStageState battleStage)
        {
            WaterCurrentManager.Ins.gameObject.SetActive(true);
            EnemyAllSpawners.Ins.spawner[2].gameObject.SetActive(true);
            Debug.Log("스테이지 4: 상어 등장");
        }
    }
}