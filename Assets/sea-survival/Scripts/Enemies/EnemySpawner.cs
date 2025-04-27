using System.Collections.Generic;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("적 생성 설정")] [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private float spawnRate = 1f;
        [SerializeField] private float minSpawnDistance = 10f;
        [SerializeField] private float maxSpawnDistance = 15f;
        [SerializeField] private int maxEnemies = 50;

        private Player Player => Player.Ins;
        private float _nextSpawnTime = 0f;
        private List<GameObject> _activeEnemies = new List<GameObject>();


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
            if (enemyPrefabs.Length == 0 || Player == null) return;

            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            float spawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 spawnPosition = Player.transform.position + new Vector3(spawnDirection.x, spawnDirection.y, 0) * spawnDistance;

            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);


            _activeEnemies.Add(enemy);
        }
    }
}