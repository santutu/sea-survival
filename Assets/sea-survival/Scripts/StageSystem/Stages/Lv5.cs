using UnityEngine;

namespace sea_survival.Scripts.Stages.Stages
{
    public class Lv5 : IStageLevel
    {
        public void SetupEnemies(BattleStage battleStage)
        {
            // 스테이지 5: 보스 상어 (체력 25배, 전체 공격)
            // battleStage.EnableEnemyType(battleStage.BossSharkPrefab);
            Debug.Log("스테이지 5: 보스 상어 등장 (체력 25배, 전체 공격)");
        }
    }
}