using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;
using UnityEngine.UI;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.StageSystem;

namespace sea_survival.Scripts.Players
{
    public class Player : SingletonMonoBehaviour<Player>, IDamageable
    {
        public float moveSpeed = 5f;

        [SerializeField] public float hp;
        [SerializeField] public float hpRegen = 0;
        [SerializeField] public float maxHp;
        [SerializeField] public float HpPercent => hp / maxHp;
        [SerializeField] public Image healthBarImage;

        [Header("피격 효과")][SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private float invincibilityTime = 0.5f;

        private Rigidbody2D _rb;
        public Animator animator;
        private SpriteRenderer _spriteRenderer;
        private bool _isInvincible = false;
        private float _invincibilityTimer = 0f;

        public Vector2 InputVec { get; private set; }
        public Vector2 MoveDirection { get; private set; }

        [SerializeField] public GameObject area;

        public Vector2 Direction => _spriteRenderer.flipX ? Vector2.left : Vector2.right;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetAnimation(AnimState state, bool active)
        {
            animator.SetBool(state.ToString(), active);
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

            // HP 재생 적용 (초당)
            if (hpRegen > 0 && hp < maxHp)
            {
                hp = Mathf.Min(hp + (hpRegen * Time.deltaTime), maxHp);
            }

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

        public void TakeDamage(float damage)
        {
            if (_isInvincible) return;

            hp -= damage;

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            _isInvincible = true;
            _invincibilityTimer = invincibilityTime;

            if (hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("플레이어 사망");
            SetAnimation(AnimState.IsFalling, false);
            SetAnimation(AnimState.IsIdle, false);
            SetAnimation(AnimState.IsMoving, false);
            animator.SetTrigger("Death");
            StageManager.Ins.GameOver();
        }
    }
}