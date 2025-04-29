using System.Collections.Generic;
using Santutu.Core.Base.Runtime.Singletons;
using Santutu.Core.GameObjectTraveler.Runtime;
using sea_survival.Scripts.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace sea_survival.Scripts.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<Transform> spawnPoints = new();
        [SerializeField] private GameObject spawnPoint;

        [Header("적 생성 설정")][SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private float spawnRate = 1f;
        [SerializeField] private int maxEnemies = 50;

        private Player Player => Player.Ins;
        private float _nextSpawnTime = 0f;
        private List<GameObject> _activeEnemies = new List<GameObject>();

        private void Awake()
        {
            foreach (var item in spawnPoint.Children())
            {
                spawnPoints.Add(item.transform);
            }
        }

        private void Update()
        {
            if (Player == null) return;

            CleanInactiveEnemies();

            if (_activeEnemies.Count < maxEnemies && Time.time >= _nextSpawnTime)
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + 1f / spawnRate;
            }
        }

        private void CleanInactiveEnemies()
        {
            _activeEnemies.RemoveAll(enemy => enemy == null);
        }

        private void SpawnEnemy()
        {
            if (enemyPrefabs.Length == 0 || Player == null || spawnPoints.Count == 0) return;

            // 랜덤한 스폰 포인트 선택
            Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Vector3 spawnPosition = selectedSpawnPoint.position;

            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            _activeEnemies.Add(enemy);
        }
    }
}