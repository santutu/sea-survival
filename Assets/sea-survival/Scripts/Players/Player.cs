using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts.Players
{
    public class Player : SingletonMonoBehaviour<Player>
    {
        public float moveSpeed = 5f;

        private Rigidbody2D _rb;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        public Vector2 InputVec { get; private set; }
        public Vector2 MoveDirection { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
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
            _animator.SetBool(AnimState.IsMoving, isMoving);
            _animator.SetBool(AnimState.IsIdle, !isMoving);
        }

        private void FixedUpdate()
        {
            Vector2 nextVec = MoveDirection * moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + nextVec);
        }
    }
}