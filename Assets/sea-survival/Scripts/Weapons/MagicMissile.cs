using System.Collections;
using System.Collections.Generic;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Enums;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public class MagicMissile : WeaponBase
    {
        [Header("마법 미사일 설정")]
        [SerializeField] private float baseDamage = 15f;
        [SerializeField] private float missileSpeed = 5f;
        [SerializeField] private float missileLifetime = 3f;
        [SerializeField] private GameObject missilePrefab;
        [SerializeField] private float explosionRadius = 1.5f; // 레벨 3에서 폭발 효과 추가
        [SerializeField] private float explosionDamagePercent = 0.5f; // 폭발 데미지는 기본 데미지의 50%
        
        private int missileCount = 1; // 레벨에 따라 증가
        private float guidanceStrength = 0.5f; // 유도 강도, 레벨에 따라 증가
        
        private void Awake()
        {
            weaponType = WeaponType.MagicMissile;
        }
        
        protected override void PerformAttack()
        {
            // 가장 가까운 적 타겟 찾기
            List<Enemy> enemies = FindNearestEnemies(missileCount);
            
            if (enemies.Count > 0)
            {
                // 찾은 적 수만큼 미사일 발사 (최대 missileCount)
                for (int i = 0; i < Mathf.Min(enemies.Count, missileCount); i++)
                {
                    LaunchMissile(enemies[i].transform);
                }
            }
            else
            {
                // 적이 없는 경우 플레이어 앞으로 발사
                for (int i = 0; i < missileCount; i++)
                {
                    StartCoroutine(DelayedLaunch(i * 0.2f, null));
                }
            }
        }
        
        private IEnumerator DelayedLaunch(float delay, Transform target)
        {
            yield return new WaitForSeconds(delay);
            LaunchMissile(target);
        }
        
        private void LaunchMissile(Transform target)
        {
            if (missilePrefab == null) return;
            
            // 미사일 생성
            GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity);
            MissileController controller = missile.AddComponent<MissileController>();
            
            // 미사일 설정
            float damage = CalculateDamage(baseDamage);
            controller.Initialize(damage, target, missileSpeed, missileLifetime, guidanceStrength, 
                currentLevel == WeaponLevel.Level3, explosionRadius, damage * explosionDamagePercent);
            
            // 일정 시간 후 미사일 제거
            Destroy(missile, missileLifetime);
        }
        
        // 가장 가까운 적 찾기 (여러 개)
        private List<Enemy> FindNearestEnemies(int count)
        {
            Enemy[] allEnemies = FindObjectsOfType<Enemy>();
            List<Enemy> nearestEnemies = new List<Enemy>();
            
            if (allEnemies.Length == 0) return nearestEnemies;
            
            // 모든 적들을 거리 순으로 정렬
            System.Array.Sort(allEnemies, (a, b) => {
                float distA = Vector2.Distance(transform.position, a.transform.position);
                float distB = Vector2.Distance(transform.position, b.transform.position);
                return distA.CompareTo(distB);
            });
            
            // 가장 가까운 적들 count만큼 선택
            for (int i = 0; i < Mathf.Min(allEnemies.Length, count); i++)
            {
                nearestEnemies.Add(allEnemies[i]);
            }
            
            return nearestEnemies;
        }
        
        // 레벨업 시 추가 효과 적용
        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            
            if (result)
            {
                if (currentLevel == WeaponLevel.Level2)
                {
                    // 레벨 2에서 미사일 개수 증가, 유도성 향상
                    missileCount = 2;
                    guidanceStrength = 1.0f;
                }
                else if (currentLevel == WeaponLevel.Level3)
                {
                    // 레벨 3에서 미사일 개수 다시 증가, 폭발 효과 추가
                    missileCount = 3;
                    guidanceStrength = 1.5f;
                }
            }
            
            return result;
        }
        
        // 현재 레벨 설명
        public override string GetCurrentLevelDescription()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return "단일 미사일 발사, 중간 공격력";
                case WeaponLevel.Level2:
                    return "2개 미사일 동시 발사, 유도성 향상";
                case WeaponLevel.Level3:
                    return "3개 미사일 동시 발사, 폭발 효과 추가 (주변 적에게 추가 데미지)";
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
                    return "2개 미사일 동시 발사, 유도성 향상";
                case WeaponLevel.Level2:
                    return "3개 미사일 동시 발사, 폭발 효과 추가 (주변 적에게 추가 데미지)";
                case WeaponLevel.Level3:
                    return "최대 레벨입니다";
                default:
                    return "";
            }
        }
        
        // 미사일 컨트롤러 클래스
        private class MissileController : MonoBehaviour
        {
            private float damage;
            private Transform target;
            private float speed;
            private float lifetime;
            private float guidanceStrength;
            private bool hasExplosion;
            private float explosionRadius;
            private float explosionDamage;
            
            public void Initialize(float damage, Transform target, float speed, float lifetime, 
                float guidanceStrength, bool hasExplosion, float explosionRadius, float explosionDamage)
            {
                this.damage = damage;
                this.target = target;
                this.speed = speed;
                this.lifetime = lifetime;
                this.guidanceStrength = guidanceStrength;
                this.hasExplosion = hasExplosion;
                this.explosionRadius = explosionRadius;
                this.explosionDamage = explosionDamage;
                
                StartCoroutine(MissileMovement());
            }
            
            private IEnumerator MissileMovement()
            {
                float elapsed = 0f;
                
                while (elapsed < lifetime)
                {
                    // 타겟이 유효한지 확인
                    if (target == null)
                    {
                        // 타겟이 없으면 전방으로 직진
                        transform.Translate(Vector3.right * speed * Time.deltaTime);
                    }
                    else
                    {
                        // 타겟 방향으로 이동
                        Vector2 direction = (target.position - transform.position).normalized;
                        float step = speed * Time.deltaTime;
                        
                        // 회전 처리
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, guidanceStrength * Time.deltaTime * 5);
                        
                        // 현재 방향으로 이동
                        transform.Translate(Vector3.right * step, Space.Self);
                    }
                    
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                
                // 수명 종료 시 제거
                Destroy(gameObject);
            }
            
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision.CompareTag("Enemy"))
                {
                    // 적중한 적에게 데미지 적용
                    IDamageable damageableTarget = collision.GetComponent<IDamageable>();
                    if (damageableTarget != null)
                    {
                        damageableTarget.TakeDamage(damage);
                    }
                    
                    // 폭발 효과가 있으면 주변 적에게도 데미지
                    if (hasExplosion)
                    {
                        Explode();
                    }
                    
                    // 미사일 제거
                    Destroy(gameObject);
                }
            }
            
            private void Explode()
            {
                // 폭발 범위 내의 모든 적을 찾음
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                
                foreach (Collider2D hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Enemy"))
                    {
                        // 원래 맞은 적은 제외
                        if (hitCollider.gameObject != target?.gameObject)
                        {
                            IDamageable damageableTarget = hitCollider.GetComponent<IDamageable>();
                            if (damageableTarget != null)
                            {
                                damageableTarget.TakeDamage(explosionDamage);
                            }
                        }
                    }
                }
                
                // TODO: 폭발 이펙트 생성
            }
            
            private void OnDrawGizmos()
            {
                if (hasExplosion)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(transform.position, explosionRadius);
                }
            }
        }
    }
} 