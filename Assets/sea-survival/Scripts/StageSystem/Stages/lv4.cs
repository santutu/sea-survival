using UnityEngine;

namespace sea_survival.Scripts.Stages.Stages
{
    public class Lv4 : IStageLevel
    {
        public void SetupEnemies(BattleStage battleStage)
        {
            // 스테이지 4: 상어 등장 (체력 5배)
            // battleStage.EnableEnemyType(battleStage.SharkPrefab);
            // battleStage.EnableEnemyType(battleStage.StrongFishPrefab, 0.3f); // 강화된 물고기도 소량 등장
            Debug.Log("스테이지 4: 상어 및 강화된 물고기 등장");
        }
    }
}