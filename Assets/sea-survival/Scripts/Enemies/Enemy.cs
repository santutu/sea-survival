using Santutu.Modules.UI.Runtime.FloatingTexts;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Players;
using UnityEngine;

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

        private Player Player => Player.Ins;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;

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
        }

        private void FixedUpdate()
        {
            if (Player == null) return;

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
    }
}