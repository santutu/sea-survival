using UnityEngine;
using sea_survival.Scripts.Contracts;
using System.Collections;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.Enemies.Attacks
{
    public class EnemyRushAttack : MonoBehaviour, IEnemyAttack
    {
        [Header("돌진 공격 설정")]
        [SerializeField] private float attackRange = 3f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float damage = 20f;
        
        [Header("돌진 설정")]
        [SerializeField] private float windupTime = 0.5f; // 뒤로 가는 시간
        [SerializeField] private float windupDistance = 1f; // 뒤로 가는 거리
        [SerializeField] private float rushSpeed = 15f; // 돌진 속도
        [SerializeField] private float rushDistance = 4f; // 돌진 거리
        [SerializeField] private float stunDuration = 0.3f; // 돌진 후 멈춤 시간
        
        [Header("이펙트")]
        [SerializeField] private GameObject warningEffect; // 경고 이펙트
        [SerializeField] private GameObject rushEffect; // 돌진 이펙트
        
        [SerializeField, ReadOnly] private float _attackTimer = 0f;
        [SerializeField, ReadOnly] private bool _isAttacking = false;
        
        private Rigidbody2D _rb;
        private Vector3 _originalPosition;
        private Vector3 _windupPosition;
        private Vector3 _rushTargetPosition;
        
        public bool CanAttack => _attackTimer <= 0f && !_isAttacking;
        public float AttackCooldown => attackCooldown;
        public float AttackRange => attackRange;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        
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
            _originalPosition = transform.position;
            
            // 타겟 방향 계산
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            _windupPosition = transform.position - (directionToTarget * windupDistance);
            _rushTargetPosition = transform.position + (directionToTarget * rushDistance);
            
            StartCoroutine(RushAttackSequence(target));
        }
        
        public void ResetAttackCooldown()
        {
            _attackTimer = attackCooldown;
        }
        
        private IEnumerator RushAttackSequence(Transform target)
        {
            // 1단계: 경고 및 뒤로 가기
            if (warningEffect != null)
            {
                Instantiate(warningEffect, transform.position, Quaternion.identity);
            }
            
            yield return StartCoroutine(WindupPhase());
            
            // 2단계: 돌진
            yield return StartCoroutine(RushPhase(target));
            
            // 3단계: 멈춤
            yield return new WaitForSeconds(stunDuration);
            
            // 공격 완료
            _isAttacking = false;
            ResetAttackCooldown();
        }
        
        private IEnumerator WindupPhase()
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            
            while (elapsed < windupTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / windupTime;
                
                // 뒤로 이동
                transform.position = Vector3.Lerp(startPos, _windupPosition, progress);
                
                yield return null;
            }
        }
        
        private IEnumerator RushPhase(Transform target)
        {
            // 돌진 이펙트 생성
            if (rushEffect != null)
            {
                Instantiate(rushEffect, transform.position, Quaternion.identity);
            }
            
            Vector3 startPos = transform.position;
            Vector3 direction = (_rushTargetPosition - startPos).normalized;
            
            float rushTime = rushDistance / rushSpeed;
            float elapsed = 0f;
            
            while (elapsed < rushTime)
            {
                elapsed += Time.deltaTime;
                
                // 돌진 이동
                Vector3 movement = direction * rushSpeed * Time.deltaTime;
                transform.position += movement;
                
                // 벽이나 장애물과의 충돌 확인
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f);
                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    break; // 벽에 충돌하면 돌진 중단
                }
                
                yield return null;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isAttacking) return;
            
            // 플레이어와 충돌했을 때 데미지 적용
            if (collision.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
                Debug.Log($"돌진 공격으로 {damage} 데미지를 입혔습니다!");
            }
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
} 