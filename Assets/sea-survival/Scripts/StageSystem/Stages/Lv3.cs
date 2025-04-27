using sea_survival.Scripts.Enemies;
using UnityEngine;

namespace sea_survival.Scripts.Stages.Stages
{
    public class Lv3 : IStageLevel
    {
        public void SetupEnemies(BattleStage battleStage)
        {
            // 스테이지 3: 파도 등장 (죽일 수 없음, 피해야 함)
            EnemyAllSpawners.Ins.spawner[2].gameObject.SetActive(true);

            // 파도 스포너 설정 (필요시 파라미터 조정)
            var waveSpawner = WaveSpawner.Ins;

            if (waveSpawner != null)
            {
                waveSpawner.gameObject.SetActive(true);
                Debug.Log("파도 스포너 활성화");
            }

            Debug.Log("스테이지 3: 파도 및 물고기 등장");
        }
    }
}