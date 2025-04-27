using UnityEngine;

namespace sea_survival.Scripts.Stages.Stages
{
    public interface IStageLevel
    {
        // 스테이지 적 설정
        void SetupEnemies(BattleStage battleStage);
    }
} 