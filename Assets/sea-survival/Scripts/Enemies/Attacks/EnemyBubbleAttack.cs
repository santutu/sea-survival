using UnityEngine;
using sea_survival.Scripts.Contracts;
using System.Collections;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.Enemies.Attacks
{
    public class EnemyBubbleAttack : MonoBehaviour, IEnemyAttack
    {
        [Header("거품 공격 설정")]
        [SerializeField] private float attackRange = 6f;
        [SerializeField] private float attackCooldown = 3f;
        [SerializeField] private float damage = 15f;
        
        [Header("발사체 설정")]
        [SerializeField] private GameObject bubblePrefab; // 거품 발사체 프리팹
        [SerializeField] private float projectileSpeed = 8f;
        [SerializeField] private float projectileLifetime = 3f;
        [SerializeField] private int projectileCount = 1; // 한 번에 발사할 거품 수
        [SerializeField] private float spreadAngle = 15f; // 다중 발사 시 퍼짐 각도
        
        [Header("발사 설정")]
        [SerializeField] private Transform firePoint; // 발사 지점
        [SerializeField] private float windupTime = 0.8f; // 공격 전 준비 시간
        
        [Header("이펙트")]
        [SerializeField] private GameObject chargingEffect; // 충전 이펙트
        [SerializeField] private GameObject fireEffect; // 발사 이펙트
        
        [SerializeField, ReadOnly] private float _attackTimer = 0f;
        [SerializeField, ReadOnly] private bool _isAttacking = false;
        
        public bool CanAttack => _attackTimer <= 0f && !_isAttacking;
        public float AttackCooldown => attackCooldown;
        public float AttackRange => attackRange;
        
        private void Update()
        {
            if (_attackTimer > 0f)
            {
                _attackTimer -= Time.deltaTime;
            }
        }
        
        public void Attack(Transform target)
        {
            if (!CanAttack || target == null)
                return;
            
            _isAttacking = true;
            StartCoroutine(BubbleAttackSequence(target));
        }
        
        public void ResetAttackCooldown()
        {
            _attackTimer = attackCooldown;
        }
        
        private IEnumerator BubbleAttackSequence(Transform target)
        {
            // 1단계: 충전
            GameObject chargingObj = null;
            if (chargingEffect != null)
            {
                Vector3 effectPos = firePoint != null ? firePoint.position : transform.position;
                chargingObj = Instantiate(chargingEffect, effectPos, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(windupTime);
            
            // 충전 이펙트 제거
            if (chargingObj != null)
            {
                Destroy(chargingObj);
            }
            
            // 2단계: 발사
            FireBubbles(target);
            
            // 발사 이펙트
            if (fireEffect != null)
            {
                Vector3 effectPos = firePoint != null ? firePoint.position : transform.position;
                Instantiate(fireEffect, effectPos, Quaternion.identity);
            }
            
            // 공격 완료
            _isAttacking = false;
            ResetAttackCooldown();
        }
        
        private void FireBubbles(Transform target)
        {
            if (bubblePrefab == null)
            {
                Debug.LogWarning("Bubble prefab이 설정되지 않았습니다!");
                return;
            }
            
            Vector3 shootPosition = firePoint != null ? firePoint.position : transform.position;
            Vector3 baseDirection = (target.position - shootPosition).normalized;
            
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 shootDirection = baseDirection;
                
                // 다중 발사 시 각도 조정
                if (projectileCount > 1)
                {
                    float angleOffset = (i - (projectileCount - 1) * 0.5f) * spreadAngle;
                    shootDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
                }
                
                // 거품 발사체 생성
                GameObject bubble = Instantiate(bubblePrefab, shootPosition, Quaternion.identity);
                
                // 발사체 초기화
                InitializeBubbleProjectile(bubble, shootDirection);
            }
        }
        
        private void InitializeBubbleProjectile(GameObject bubble, Vector3 direction)
        {
            // Rigidbody2D로 이동
            Rigidbody2D bubbleRb = bubble.GetComponent<Rigidbody2D>();
            if (bubbleRb != null)
            {
                bubbleRb.linearVelocity = direction * projectileSpeed;
            }
            
            // BubbleProjectile 컴포넌트 설정
            BubbleProjectile projectile = bubble.GetComponent<BubbleProjectile>();
            if (projectile == null)
            {
                projectile = bubble.AddComponent<BubbleProjectile>();
            }
            
            projectile.Initialize(damage, projectileLifetime);
            
            // 회전 설정 (방향에 맞게)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bubble.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        [Button("테스트 공격")]
        public void TestAttack()
        {
            Transform playerTransform = FindObjectOfType<Players.Player>()?.transform;
            if (playerTransform != null)
            {
                Attack(playerTransform);
            }
        }
    }
    
    // 거품 발사체 클래스
    public class BubbleProjectile : MonoBehaviour
    {
        private float _damage;
        private float _lifetime;
        private bool _hasHit = false;
        
        public void Initialize(float damage, float lifetime)
        {
            _damage = damage;
            _lifetime = lifetime;
            
            // 수명 타이머 시작
            Destroy(gameObject, lifetime);
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hasHit) return;
            
            // 발사한 적 자신과의 충돌 무시
            if (collision.GetComponent<Enemy>() != null)
                return;
            
            // 플레이어 또는 다른 IDamageable과 충돌
            if (collision.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(_damage);
                _hasHit = true;
                
                // 충돌 이펙트 생성 (옵션)
                // TODO: 거품 터지는 이펙트 추가
                
                Destroy(gameObject);
            }
            
            // 벽이나 장애물과 충돌
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                _hasHit = true;
                Destroy(gameObject);
            }
        }
    }
} 