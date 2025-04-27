using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;
using UnityEngine.UI;
using sea_survival.Scripts.Contracts;

namespace sea_survival.Scripts.Players
{
    public class Player : SingletonMonoBehaviour<Player>, IDamageable
    {
        public float moveSpeed = 5f;

        [SerializeField] public float hp;
        [SerializeField] public float maxHp;
        [SerializeField] public float HpPercent => hp / maxHp;
        [SerializeField] public Image healthBarImage;

        [Header("피격 효과")] [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private float invincibilityTime = 0.5f;

        private Rigidbody2D _rb;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private bool _isInvincible = false;
        private float _invincibilityTimer = 0f;

        public Vector2 InputVec { get; private set; }
        public Vector2 MoveDirection { get; private set; }

        [SerializeField]
        public GameObject area;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetAnimation(AnimState state, bool active)
        {
            _animator.SetBool(state.ToString(), active);
        }

        private void Update()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            InputVec = new Vector2(moveX, moveY);
            MoveDirection = InputVec.normalized;

            if (moveX != 0)
            {
                _spriteRenderer.flipX = moveX < 0;
            }

            var isMoving = MoveDirection.magnitude > 0;
            SetAnimation(AnimState.IsMoving, isMoving);
            SetAnimation(AnimState.IsIdle, !isMoving);

            healthBarImage.fillAmount = HpPercent;

            // 무적 시간 처리
            if (_isInvincible)
            {
                _invincibilityTimer -= Time.deltaTime;
                if (_invincibilityTimer <= 0f)
                {
                    _isInvincible = false;
                    // 깜빡임 효과 제거
                    _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    // 깜빡임 효과
                    float alpha = Mathf.PingPong(Time.time * 10f, 1f);
                    _spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
                }
            }
        }

        private void FixedUpdate()
        {
            Vector2 nextVec = MoveDirection * moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + nextVec);
        }

        // IDamageable 인터페이스 구현
        public void TakeDamage(float damage)
        {
            // 무적 상태면 데미지를 받지 않음
            if (_isInvincible) return;

            hp -= damage;

            // 피격 이펙트 생성
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // 무적 상태 설정
            _isInvincible = true;
            _invincibilityTimer = invincibilityTime;

            // 체력이 0 이하면 사망 처리
            if (hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // 사망 로직 구현
            Debug.Log("플레이어 사망");
            // 게임 오버 처리 등을 여기에 추가
        }
    }
}