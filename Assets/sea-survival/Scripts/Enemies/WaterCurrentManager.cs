using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using sea_survival.Scripts.Players;
using Santutu.Core.Base.Runtime.Singletons;

namespace sea_survival.Assets.sea_survival.Scripts.Enemies
{
    public class WaterCurrentManager : SingletonMonoBehaviour<WaterCurrentManager>
    {
        [Header("물살 이펙트 설정")]
        [SerializeField] private GameObject waterCurrentEffectPrefab;
        [SerializeField] private float spawnInterval = 0.5f;
        [SerializeField] private float minY = -5f;
        [SerializeField] private float maxY = 5f;

        [Header("플레이어 이속 감소")]
        [SerializeField] private float speedMultiplier = 0.7f;

        private float timer;
        private bool isActive = true;

        private void OnEnable()
        {
            if (Player.Ins != null)
            {
                Player.Ins.speedmultiplyModifier *= speedMultiplier;
            }
            isActive = true;
        }

        private void OnDisable()
        {
            if (Player.Ins != null)
            {
                Player.Ins.speedmultiplyModifier /= speedMultiplier;
            }
            isActive = false;
        }

        private void Update()
        {
            if (!isActive) return;

            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnWaterCurrentEffect();
                timer = 0f;
            }
        }

        private void SpawnWaterCurrentEffect()
        {
            float randomY = UnityEngine.Random.Range(minY, maxY);
            Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0f);

            if (waterCurrentEffectPrefab != null)
            {
                Instantiate(waterCurrentEffectPrefab, spawnPosition, Quaternion.identity);
            }
        }

        private void OnDrawGizmos()
        {
            // 생성 범위를 나타내는 세로 선
            Gizmos.color = Color.cyan;
            Vector3 lineStart = new Vector3(transform.position.x, minY, 0f);
            Vector3 lineEnd = new Vector3(transform.position.x, maxY, 0f);
            Gizmos.DrawLine(lineStart, lineEnd);

            // 위아래 경계를 나타내는 가로 선
            float lineLength = 1f; // 가로 선의 길이
            Vector3 topLineStart = new Vector3(transform.position.x - lineLength / 2f, maxY, 0f);
            Vector3 topLineEnd = new Vector3(transform.position.x + lineLength / 2f, maxY, 0f);
            Vector3 bottomLineStart = new Vector3(transform.position.x - lineLength / 2f, minY, 0f);
            Vector3 bottomLineEnd = new Vector3(transform.position.x + lineLength / 2f, minY, 0f);

            Gizmos.DrawLine(topLineStart, topLineEnd);
            Gizmos.DrawLine(bottomLineStart, bottomLineEnd);

            // 현재 위치 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}