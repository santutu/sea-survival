using System;
using Santutu.Modules.UI.Runtime.FloatingTexts;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Players;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.Enemies
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        public float moveSpeed = 3f;
        public float maxHealth = 100f;
        public float currentHealth;

        [Header("경험치 설정")] [SerializeField] private GameObject expPrefab; // 경험치 아이템 프리팹
        [SerializeField] private int expAmount = 10; // 기본 경험치 양
        [SerializeField] private float expDropChance = 1.0f; // 경험치 드롭 확률 (0.0 ~ 1.0)

        [Header("플레이어 충돌 설정")] [SerializeField]
        private float damageToPlayer = 10f; // 플레이어에게 주는 데미지

        [SerializeField] private float knockbackForce = 5f; // 플레이어에게 밀려날 때의 힘
        [SerializeField] private float attackCooldown = 0.5f; // 공격 쿨다운 시간
        [SerializeField] private float knockbackDuration = 0.5f; // 넉백 지속 시간

        [Header("공격 시스템")]
        [SerializeField] private List<AttackType> availableAttacks = new List<AttackType>(); // 사용 가능한 공격 타입
        [SerializeField] private float specialAttackChance = 0.3f; // 특수 공격 확률 (0.0 ~ 1.0)
        [SerializeField] private float attackDecisionDistance = 5f; // 공격 결정 거리

        private Player Player => Player.Ins;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private bool _canAttack = true;
        private float _attackTimer = 0f;
        private bool _isKnockedBack = false;
        private float _knockbackTimer = 0f;

        // 공격 모듈들
        private Dictionary<AttackType, IEnemyAttack> _attackModules = new Dictionary<AttackType, IEnemyAttack>();

        [SerializeField] public GameObject hitEffectPrefab;

        [System.Serializable]
        public enum AttackType
        {
            Melee,      // 기본 근접 공격 (접촉)
            Rush,       // 돌진 공격
            Bubble      // 거품 원거리 공격
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            currentHealth = maxHealth;
            
            // 공격 모듈들 초기화
            InitializeAttackModules();
        }

        private void InitializeAttackModules()
        {
            // 각 공격 타입에 해당하는 컴포넌트 찾기
            var rushAttack = GetComponent<Attacks.EnemyRushAttack>();
            if (rushAttack != null)
            {
                _attackModules[AttackType.Rush] = rushAttack;
            }

            var bubbleAttack = GetComponent<Attacks.EnemyBubbleAttack>();
            if (bubbleAttack != null)
            {
                _attackModules[AttackType.Bubble] = bubbleAttack;
            }
        }

        private void Update()
        {
            if (Player == null) return;

            Vector2 direction = Player.transform.position - transform.position;
            direction.Normalize();
            float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

            if (direction.x != 0)
            {
                _spriteRenderer.flipX = direction.x < 0;
            }

            // 공격 쿨다운 처리
            if (!_canAttack)
            {
                _attackTimer -= Time.deltaTime;
                if (_attackTimer <= 0f)
                {
                    _canAttack = true;
                }
            }

            // 넉백 타이머 처리
            if (_isKnockedBack)
            {
                _knockbackTimer -= Time.deltaTime;
                if (_knockbackTimer <= 0f)
                {
                    _isKnockedBack = false;
                }
            }

            // 공격 결정 로직
            if (_canAttack && distanceToPlayer <= attackDecisionDistance)
            {
                TrySpecialAttack();
            }
        }

        private void TrySpecialAttack()
        {
            // 특수 공격 확률 체크
            if (Random.value > specialAttackChance)
                return;

            // 사용 가능한 공격 중에서 랜덤 선택
            List<AttackType> usableAttacks = new List<AttackType>();

            foreach (var attackType in availableAttacks)
            {
                if (_attackModules.TryGetValue(attackType, out IEnemyAttack attackModule))
                {
                    float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
                    if (attackModule.CanAttack && distanceToPlayer <= attackModule.AttackRange)
                    {
                        usableAttacks.Add(attackType);
                    }
                }
            }

            if (usableAttacks.Count > 0)
            {
                AttackType selectedAttack = usableAttacks[Random.Range(0, usableAttacks.Count)];
                ExecuteAttack(selectedAttack);
            }
        }

        private void ExecuteAttack(AttackType attackType)
        {
            if (_attackModules.TryGetValue(attackType, out IEnemyAttack attackModule))
            {
                attackModule.Attack(Player.transform);
                _canAttack = false;
                _attackTimer = attackCooldown;
                
                Debug.Log($"{gameObject.name}이(가) {attackType} 공격을 사용했습니다!");
            }
        }

        private void FixedUpdate()
        {
            if (Player == null) return;

            // 넉백 상태일 때는 플레이어 추적을 중단
            if (_isKnockedBack) return;

            // 공격 중일 때는 이동하지 않음 (돌진 공격 제외)
            bool isAttacking = false;
            foreach (var attackModule in _attackModules.Values)
            {
                if (attackModule is Attacks.EnemyRushAttack rushAttack && !rushAttack.CanAttack)
                {
                    // 돌진 공격 중에는 자체적으로 이동하므로 일반 이동 중단
                    return;
                }
                if (attackModule is Attacks.EnemyBubbleAttack bubbleAttack && !bubbleAttack.CanAttack)
                {
                    isAttacking = true;
                    break;
                }
            }

            if (isAttacking) return;

            // 플레이어 쪽으로 이동
            Vector2 direction = Player.transform.position - transform.position;
            direction.Normalize();
            _rb.linearVelocity = direction * moveSpeed;
        }

        // IDamageable 인터페이스 구현
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;

            FloatingDamageTextManager.Ins.Instantiate((int)damage, transform.position);

            // 피격 이펙트 생성
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // 체력이 0 이하면 적 제거
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // 경험치 드롭
            DropExp();
            Player.Ins.killedEnemiesCount++;

            // 여기에 추가적인 죽었을 때 로직 (예: 애니메이션, 소리 등)

            // 적 제거
            Destroy(gameObject);
        }

        private void DropExp()
        {
            // 확률에 따라 경험치 드롭
            if (Random.value <= expDropChance)
            {
                // 경험치 프리팹이 없으면 기본 프리팹 찾기 시도
                if (expPrefab == null)
                {
                    expPrefab = Resources.Load<GameObject>("Prefabs/Exp");

                    // 아직도 없으면 로그 출력 후 종료
                    if (expPrefab == null)
                    {
                        Debug.LogWarning("Exp 프리팹을 찾을 수 없습니다! Resources/Prefabs/Exp 위치에 경험치 프리팹을 추가하세요.");
                        return;
                    }
                }

                // 경험치 아이템 생성
                GameObject expObject = Instantiate(expPrefab, transform.position, Quaternion.identity);

                // Exp 컴포넌트가 있으면 경험치 값 설정
                Exp expComponent = expObject.GetComponent<Exp>();
                if (expComponent != null)
                {
                    expComponent.SetExpValue(expAmount);
                }
            }
        }

        // 플레이어와의 충돌 처리 (기본 근접 공격)
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                // 기본 근접 공격만 충돌로 처리 (다른 공격들은 각자 모듈에서 처리)
                if (availableAttacks.Contains(AttackType.Melee) && _canAttack)
                {
                    ApplyDamageToPlayer(player);

                    // 공격 쿨다운 설정
                    _canAttack = false;
                    _attackTimer = attackCooldown;
                }

                // 적이 밀려나도록 처리 (항상 적용)
                ApplyKnockbackToSelf(player.transform.position);
            }
        }

        // 플레이어에게 데미지를 주는 함수
        private void ApplyDamageToPlayer(Player player)
        {
            // IDamageable 인터페이스를 통해 데미지 적용
            player.TakeDamage(damageToPlayer);
        }

        // 자신을 플레이어로부터 밀쳐내는 함수
        private void ApplyKnockbackToSelf(Vector3 playerPosition)
        {
            Vector2 knockbackDirection = (transform.position - playerPosition).normalized;

            // 속도를 직접 설정하여 넉백 효과 적용
            _rb.linearVelocity = knockbackDirection * knockbackForce;

            // 넉백 상태 설정
            _isKnockedBack = true;
            _knockbackTimer = knockbackDuration;
        }

        [Button("돌진 공격 추가")]
        public void AddRushAttack()
        {
            if (!availableAttacks.Contains(AttackType.Rush))
            {
                availableAttacks.Add(AttackType.Rush);
                
                if (GetComponent<Attacks.EnemyRushAttack>() == null)
                {
                    gameObject.AddComponent<Attacks.EnemyRushAttack>();
                }
                
                InitializeAttackModules();
            }
        }

        [Button("거품 공격 추가")]
        public void AddBubbleAttack()
        {
            if (!availableAttacks.Contains(AttackType.Bubble))
            {
                availableAttacks.Add(AttackType.Bubble);
                
                if (GetComponent<Attacks.EnemyBubbleAttack>() == null)
                {
                    gameObject.AddComponent<Attacks.EnemyBubbleAttack>();
                }
                
                InitializeAttackModules();
            }
        }

        [Button("기본 공격만 사용")]
        public void UseOnlyMeleeAttack()
        {
            availableAttacks.Clear();
            availableAttacks.Add(AttackType.Melee);
        }
    }
}