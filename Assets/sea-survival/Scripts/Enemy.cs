using System.Collections;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class Enemy : MonoBehaviour
    {
        public float moveSpeed = 3f;

        private Player Player => Player.Ins;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
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
    }
}