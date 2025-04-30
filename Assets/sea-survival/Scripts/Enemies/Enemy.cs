using System;
using Santutu.Modules.UI.Runtime.FloatingTexts;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Players;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private Player Player => Player.Ins;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private bool _canAttack = true;
        private float _attackTimer = 0f;
        private bool _isKnockedBack = false;
        private float _knockbackTimer = 0f;

        [SerializeField] public GameObject hitEffectPrefab;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            currentHealth = maxHealth;
        }

        private void Update()
        {
            if (Player == null) return;

            Vector2 direction = Player.transform.position - transform.position;
            direction.Normalize();

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
        }

        private void FixedUpdate()
        {
            if (Player == null) return;

            // 넉백 상태일 때는 플레이어 추적을 중단
            if (_isKnockedBack) return;

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

        // 플레이어와의 충돌 처리
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                // 플레이어 데미지 처리 (쿨타임에 따라 적용)
                if (_canAttack)
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
    }
}