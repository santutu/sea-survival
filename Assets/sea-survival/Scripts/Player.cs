using sea_survival.Scripts.Singletons;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class Player : SingletonMonoBehaviour<Player>
    {
        public float moveSpeed = 5f;

        private Rigidbody2D _rb;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private Vector2 _moveDirection;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            // 기본 공격 컴포넌트 추가 (없는 경우)
            if (GetComponent<DefaultAttack>() == null)
            {
                gameObject.AddComponent<DefaultAttack>();
            }
        }

        private void Update()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            _moveDirection = new Vector2(moveX, moveY).normalized;

            if (moveX != 0)
            {
                _spriteRenderer.flipX = moveX < 0;
            }


            var isMoving = _moveDirection.magnitude > 0;
            _animator.SetBool("IsMoving", isMoving);
            _animator.SetBool("IsIdle", !isMoving);
        }

        private void FixedUpdate()
        {
            Vector2 nextVec = _moveDirection * moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + nextVec);
        }
    }
}