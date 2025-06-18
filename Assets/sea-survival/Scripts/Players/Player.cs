using Cysharp.Threading.Tasks;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;
using UnityEngine.UI;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.StageSystem;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.Players
{
    public class Player : SingletonMonoBehaviour<Player>, IDamageable
    {
        [SerializeField] public float moveSpeed = 5f;
        [SerializeField] public float breathingMoveSpeedMultiplier = 0.5f; // 숨쉬기 모드에서의 이동속도 배율

        public float speedmultiplyModifier = 1f;


        public float CurrentMoveSpeed => moveSpeed * speedmultiplyModifier;

        [SerializeField] public float hp;
        [SerializeField] public float hpRegen = 0;
        [SerializeField] public float maxHp;
        [SerializeField] public float HpPercent => hp / maxHp;
        [SerializeField] public Image healthBarImage;

        [Header("피격 효과")] [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private float invincibilityTime = 0.5f;

        [Header("산소 시스템")] [SerializeField] private float maxOxygen = 100f;
        [SerializeField] private float currentOxygen;
        [SerializeField] private float oxygenDecreaseRate = 10f; // 초당 감소량
        [SerializeField] private float oxygenIncreaseRate = 20f; // 초당 증가량
        [SerializeField] private float oxygenDamageRate = 10f; // 산소 없을 때 초당 데미지
        [SerializeField] private Image oxygenBarImage;
        [SerializeField] private Transform waterLine1; // 1번 선 위치
        [SerializeField] private Transform waterLine2; // 2번 선 위치
        [SerializeField] private Rigidbody2D blockUpper; // 위 콜라이더

        [Header("공기방울 효과")] 
        [SerializeField] private BubbleEffectManager bubbleEffectManager;

        [SerializeField, ReadOnly] public int killedEnemiesCount = 0;


        private Rigidbody2D _rb;
        public Animator animator;
        private SpriteRenderer _spriteRenderer;
        private bool _isInvincible = false;
        private float _invincibilityTimer = 0f;
        private HeartSystem _heartSystem;

        public Vector2 InputVec { get; private set; }
        public Vector2 MoveDirection { get; private set; }

        [SerializeField] public GameObject area;

        [SerializeField] private GameObject spacebarIcon;

        public Vector2 Direction => _spriteRenderer.flipX ? Vector2.left : Vector2.right;

        private bool isBreathing = false;
        private bool canBreath = false;

        // 산소 시스템 접근용 프로퍼티
        public float MaxOxygen => maxOxygen;
        public float CurrentOxygen => currentOxygen;
        public void SetCurrentOxygen(float value) => currentOxygen = Mathf.Clamp(value, 0f, maxOxygen);
        
        // 호흡 상태 접근용 프로퍼티
        public bool IsBreathing => isBreathing;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _heartSystem = GetComponent<HeartSystem>();
            
            // 공기방울 효과 매니저 초기화
            if (bubbleEffectManager == null)
            {
                bubbleEffectManager = GetComponent<BubbleEffectManager>();
                if (bubbleEffectManager == null)
                {
                    bubbleEffectManager = gameObject.AddComponent<BubbleEffectManager>();
                }
            }
        }

        private void Start()
        {
            currentOxygen = maxOxygen;
        }

        public void SetAnimation(AnimState state, bool active)
        {
            animator.SetBool(state.ToString(), active);
        }

        public void StartInvincibility(float duration)
        {
            _isInvincible = true;
            _invincibilityTimer = duration;
        }

        private void Update()
        {
            // 1번 선 위에 있는지 체크
            canBreath = transform.position.y >= waterLine1.position.y;

            spacebarIcon.gameObject.SetActive(canBreath);


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
            float currentMoveSpeed = CurrentMoveSpeed;
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
                // HP가 0이 되면 하트 시스템을 통해 처리
                if (_heartSystem != null && _heartSystem.TryLoseHeart())
                {
                    // 하트를 잃었지만 아직 살아있음 (하트 시스템에서 HP 회복 및 무적 처리)
                    return;
                }
                
                // 하트가 없거나 하트 시스템이 없으면 죽음
                Die();
            }
        }

        // 산소 부족으로 인한 데미지는 무적을 무시
        private void TakeOxygenDamage(float damage)
        {
            hp -= damage;

            if (hp <= 0)
            {
                // HP가 0이 되면 하트 시스템을 통해 처리
                if (_heartSystem != null && _heartSystem.TryLoseHeart())
                {
                    // 하트를 잃었지만 아직 살아있음
                    return;
                }
                
                // 하트가 없거나 하트 시스템이 없으면 죽음
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
        
        // 하트 시스템 접근을 위한 공개 메서드
        public HeartSystem GetHeartSystem()
        {
            return _heartSystem;
        }
    }
}