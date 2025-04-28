using System.Collections;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public class SoundWave : WeaponBase
    {
        [Header("음파 폭발 설정")] [SerializeField] private float baseDamage = 15f;
        [SerializeField] private float baseRadius = 3f; // 기본 폭발 반경
        [SerializeField] private GameObject waveEffectPrefab;
        [SerializeField] private float baseAttackDelay = 5f; // 기본 발사 간격 (초)

        [Header("레벨별 효과")] [SerializeField] private float slowEffectDuration = 1.5f; // 감속 효과 지속 시간 (레벨 2)
        [SerializeField] private float slowEffectAmount = 0.3f; // 감속 효과 비율 (30% 감소)
        [SerializeField] private float stunEffectDuration = 0.5f; // 기절 효과 지속 시간 (레벨 3)

        private void Awake()
        {
            weaponType = WeaponType.SoundWave;
            baseAttackInterval = baseAttackDelay;
        }

        protected override void PerformAttack()
        {
            // 플레이어 위치
            Vector2 position = transform.position;

            // 현재 레벨에 따른 반경 계산
            float radius = CalculateRadius();

            // 데미지 계산
            float damage = CalculateDamage(baseDamage);

            // 파동 이펙트 생성
            if (waveEffectPrefab != null)
            {
                GameObject waveEffect = Instantiate(waveEffectPrefab, position, Quaternion.identity);
                WaveEffectController controller = waveEffect.AddComponent<WaveEffectController>();

                // 이펙트 스케일 설정 (반경에 맞게)
                waveEffect.transform.localScale = new Vector3(radius / 5f, radius / 5f, 1f);

                // 이펙트 초기화
                controller.Initialize(radius);

                // 1초 후 이펙트 제거
                Destroy(waveEffect, 1f);
            }

            // 범위 내 적 감지 및 데미지 적용
            DetectAndDamageEnemies(position, radius, damage);
        }

        // 범위 내 적 감지 및 데미지 적용
        private void DetectAndDamageEnemies(Vector2 center, float radius, float damage)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    // 데미지 적용
                    IDamageable damageableTarget = hitCollider.GetComponent<IDamageable>();
                    if (damageableTarget != null)
                    {
                        damageableTarget.TakeDamage(damage);
                    }

                    // 레벨 2 이상이면 감속 효과 적용
                    if (currentLevel >= WeaponLevel.Level2)
                    {
                        ApplySlowEffect(hitCollider.gameObject);
                    }

                    // 레벨 3이면 기절 효과 추가 적용
                    if (currentLevel == WeaponLevel.Level3)
                    {
                        ApplyStunEffect(hitCollider.gameObject);
                    }
                }
            }
        }

        // 레벨에 따른 반경 계산
        private float CalculateRadius()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return baseRadius;
                case WeaponLevel.Level2:
                    return baseRadius * 1.2f; // 레벨 2는 범위 20% 증가
                case WeaponLevel.Level3:
                    return baseRadius * 1.4f; // 레벨 3는 범위 40% 증가
                default:
                    return baseRadius;
            }
        }

        // 레벨에 따른 발사 간격 계산
        protected override float GetCurrentAttackInterval()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return baseAttackDelay; // 5초
                case WeaponLevel.Level2:
                    return baseAttackDelay * 0.8f; // 4초
                case WeaponLevel.Level3:
                    return baseAttackDelay * 0.6f; // 3초
                default:
                    return baseAttackDelay;
            }
        }

        // 감속 효과 적용 (레벨 2 이상)
        private void ApplySlowEffect(GameObject enemy)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                StartCoroutine(SlowEnemy(enemyComponent));
            }
        }

        // 기절 효과 적용 (레벨 3)
        private void ApplyStunEffect(GameObject enemy)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                StartCoroutine(StunEnemy(enemyComponent));
            }
        }

        // 적 감속 코루틴
        private IEnumerator SlowEnemy(Enemy enemy)
        {
            // 원래 속도 저장
            float originalSpeed = enemy.moveSpeed;

            // 감속 적용
            enemy.moveSpeed *= (1 - slowEffectAmount);

            // 지정된 시간 동안 대기
            yield return new WaitForSeconds(slowEffectDuration);

            // 적이 아직 존재하면 원래 속도로 복원
            if (enemy != null)
            {
                enemy.moveSpeed = originalSpeed;
            }
        }

        // 적 기절 코루틴
        private IEnumerator StunEnemy(Enemy enemy)
        {
            // 원래 속도 저장
            float originalSpeed = enemy.moveSpeed;

            // 완전 정지 (기절)
            enemy.moveSpeed = 0;

            // 지정된 시간 동안 대기
            yield return new WaitForSeconds(stunEffectDuration);

            // 적이 아직 존재하면 원래 속도로 복원
            if (enemy != null)
            {
                enemy.moveSpeed = originalSpeed;
            }
        }

        // 레벨업 시 추가 효과 적용
        [Button]
        public override bool LevelUp()
        {
            bool result = base.LevelUp();

            if (result)
            {
                // 레벨 2, 3에서는 별도의 변수 설정 필요 없음
                // 이미 메서드 내에서 레벨에 따른 효과가 적용됨
            }

            return result;
        }

        // 현재 레벨 설명
        public override string GetCurrentLevelDescription()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return "5초마다 파동 방출, 중간 범위";
                case WeaponLevel.Level2:
                    return "4초마다 방출, 범위 20% 증가, 적 이동속도 감소 효과";
                case WeaponLevel.Level3:
                    return "3초마다 방출, 범위 40% 증가, 적 기절 효과 추가";
                default:
                    return "";
            }
        }

        // 다음 레벨 설명
        public override string GetNextLevelDescription()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return "4초마다 방출, 범위 20% 증가, 적 이동속도 감소 효과";
                case WeaponLevel.Level2:
                    return "3초마다 방출, 범위 40% 증가, 적 기절 효과 추가";
                case WeaponLevel.Level3:
                    return "최대 레벨입니다";
                default:
                    return "";
            }
        }

        // 음파 폭발 이펙트 컨트롤러
        private class WaveEffectController : MonoBehaviour
        {
            private float maxRadius;
            private float expansionSpeed = 5f;
            private float currentSize = 0f;

            public void Initialize(float radius)
            {
                maxRadius = radius;
            }

            private void Update()
            {
                // 점차 확장되는 이펙트 효과
                if (currentSize < 1f)
                {
                    currentSize += Time.deltaTime * expansionSpeed;
                    float size = Mathf.Lerp(0, 1, currentSize);
                    transform.localScale = new Vector3(size, size, 1f);

                    // 투명도 감소 (페이드 아웃)
                    SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        Color color = renderer.color;
                        color.a = Mathf.Lerp(1, 0, currentSize);
                        renderer.color = color;
                    }
                }
                else
                {
                    // 최대 크기에 도달하면 제거
                    Destroy(gameObject);
                }
            }
        }

        // 디버그용 기즈모 그리기
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, CalculateRadius());
        }
    }
}