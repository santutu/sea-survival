using sea_survival.Scripts.Enemies;
using UnityEngine;

namespace sea_survival.Scripts.StageSystem.Stages
{
    public class Lv3 : IStageLevel
    {
        public void SetupEnemies(BattleStage battleStage)
        {
            var waveSpawner = WaveSpawner.Ins;
            waveSpawner.gameObject.SetActive(true);
            Debug.Log("파도 스포너 활성화");
            Debug.Log("스테이지 3: 파도  등장");
        }
    }
}