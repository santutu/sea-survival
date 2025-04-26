using System.Collections;
using Santutu.Modules.UI.Runtime.FloatingTexts;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class Enemy : MonoBehaviour
    {
        public float moveSpeed = 3f;
        public float maxHealth = 100f;
        public float currentHealth;

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
            // 여기에 적이 죽을 때 로직 추가 (경험치 드롭 등)
            Destroy(gameObject);
        }
    }
}