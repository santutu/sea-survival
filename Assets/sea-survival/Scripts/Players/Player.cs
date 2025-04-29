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
        [SerializeField] public float moveSpeed = 5f;
        [SerializeField] public float breathingMoveSpeedMultiplier = 0.5f; // 숨쉬기 모드에서의 이동속도 배율

        [SerializeField] public float hp;
        [SerializeField] public float hpRegen = 0;
        [SerializeField] public float maxHp;
        [SerializeField] public float HpPercent => hp / maxHp;
        [SerializeField] public Image healthBarImage;

        [Header("피격 효과")][SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private float invincibilityTime = 0.5f;

        [Header("산소 시스템")]
        [SerializeField] private float maxOxygen = 100f;
        [SerializeField] private float currentOxygen;
        [SerializeField] private float oxygenDecreaseRate = 10f; // 초당 감소량
        [SerializeField] private float oxygenIncreaseRate = 20f; // 초당 증가량
        [SerializeField] private float oxygenDamageRate = 10f; // 산소 없을 때 초당 데미지
        [SerializeField] private Image oxygenBarImage;
        [SerializeField] private Transform waterLine1; // 1번 선 위치
        [SerializeField] private Transform waterLine2; // 2번 선 위치
        [SerializeField] private Rigidbody2D blockUpper; // 2번 선 위치

        private Rigidbody2D _rb;
        public Animator animator;
        private SpriteRenderer _spriteRenderer;
        private bool _isInvincible = false;
        private float _invincibilityTimer = 0f;

        public Vector2 InputVec { get; private set; }
        public Vector2 MoveDirection { get; private set; }

        [SerializeField] public GameObject area;

        public Vector2 Direction => _spriteRenderer.flipX ? Vector2.left : Vector2.right;

        private bool isBreathing = false;
        private bool canBreath = false;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            currentOxygen = maxOxygen;
        }

        public void SetAnimation(AnimState state, bool active)
        {
            animator.SetBool(state.ToString(), active);
        }

        private void Update()
        {
            // 1번 선 위에 있는지 체크
            canBreath = transform.position.y >= waterLine1.position.y;

            // 스페이스바 입력 처리
            if (Input.GetKeyDown(KeyCode.Space) && canBreath)
            {
                isBreathing = !isBreathing;
                if (isBreathing)
                {
                    // 숨쉬기 모드 진입
                    blockUpper.gameObject.SetActive(false);
                    transform.position = new Vector3(transform.position.x, waterLine2.position.y, transform.position.z);
                }
                else
                {
                    blockUpper.gameObject.SetActive(true);
                }
            }

            // 이동 입력 처리
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = isBreathing ? 0 : Input.GetAxisRaw("Vertical");
            InputVec = new Vector2(moveX, moveY);

            // 숨쉬기 모드일 때 Y 위치 고정
            if (isBreathing)
            {
                transform.position = new Vector3(transform.position.x, waterLine2.position.y, transform.position.z);
            }

            // 산소 게이지 처리
            if (isBreathing)
            {
                currentOxygen = Mathf.Min(currentOxygen + (oxygenIncreaseRate * Time.deltaTime), maxOxygen);
            }
            else
            {
                currentOxygen = Mathf.Max(currentOxygen - (oxygenDecreaseRate * Time.deltaTime), 0);
                if (currentOxygen <= 0)
                {
                    TakeOxygenDamage(oxygenDamageRate * Time.deltaTime);
                }
            }

            // 산소 게이지 UI 업데이트
            if (oxygenBarImage != null)
            {
                oxygenBarImage.fillAmount = currentOxygen / maxOxygen;
            }

            MoveDirection = InputVec.normalized;

            if (moveX != 0)
            {
                _spriteRenderer.flipX = moveX < 0;
            }

            var isMoving = MoveDirection.magnitude > 0 && !isBreathing;
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
            float currentMoveSpeed = moveSpeed;
            if (isBreathing)
            {
                currentMoveSpeed *= breathingMoveSpeedMultiplier;
            }

            Vector2 nextVec = MoveDirection * currentMoveSpeed * Time.fixedDeltaTime;
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

        // 산소 부족으로 인한 데미지는 무적을 무시
        private void TakeOxygenDamage(float damage)
        {
            hp -= damage;

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
            animator.enabled = false;
            // animator.SetTrigger("Death");
            StageManager.Ins.GameOver();
        }
    }
}