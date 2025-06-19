using System.Collections.Generic;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts.Enemies
{
    public class EnemyAllSpawners : SingletonMonoBehaviour<EnemyAllSpawners>
    {
        [SerializeField] public List<EnemySpawner> spawner = new();
        
        // 모든 스포너 정지
        public void StopAllSpawners()
        {
            foreach (var enemySpawner in spawner)
            {
                if (enemySpawner != null)
                {
                    enemySpawner.enabled = false;
                }
            }
            Debug.Log("모든 적 스포너 정지");
        }
        
        // 모든 스포너 재시작
        public void StartAllSpawners()
        {
            foreach (var enemySpawner in spawner)
            {
                if (enemySpawner != null)
                {
                    enemySpawner.enabled = true;
                }
            }
            Debug.Log("모든 적 스포너 재시작");
        }
    }
}