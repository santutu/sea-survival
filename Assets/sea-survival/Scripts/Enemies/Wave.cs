using System.Collections;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts.Enemies
{
    public class Wave : MonoBehaviour
    {
        [Header("파도 설정")] [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float damageInterval = 1f; // 데미지를 입히는 간격 (초)
        [SerializeField] private float lifeTime = 10f; // 파도 존재 시간

        private Player Player => Player.Ins;
        private bool _canDamage = true;
        private Coroutine _damageCoroutine;
        private Vector3 _moveDirection = Vector3.right; // 기본 이동 방향

        private void OnEnable()
        {
            // 파도가 활성화될 때 일정 시간 후 제거
            StartCoroutine(DestroyAfterTime());
        }

        private void OnDisable()
        {
            // 코루틴 정리
            StopAllCoroutines();
            _canDamage = true;
        }

        private void Update()
        {
            // 지정된 방향으로 이동
            transform.Translate(_moveDirection * moveSpeed * Time.deltaTime);
        }

        // 이동 방향 설정 메서드
        public void SetMoveDirection(Vector3 direction)
        {
            _moveDirection = direction.normalized;
        }

        // 이동 속도 설정 메서드
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = Mathf.Max(0.1f, speed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CheckPlayerCollision(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            CheckPlayerCollision(other);
        }

        private void CheckPlayerCollision(Collider2D other)
        {
            // 플레이어와 충돌 감지
            if (_canDamage && other.CompareTag("Player"))
            {
                ApplyDamageToPlayer();

                // 다음 데미지 적용까지 잠시 대기 (연속으로 데미지를 입는 것 방지)
                _canDamage = false;
                _damageCoroutine = StartCoroutine(ResetDamageFlag());
            }
        }

        private void ApplyDamageToPlayer()
        {
            if (Player == null) return;

            // 현재 플레이어 체력의 1/3 계산
            float damageAmount = Player.hp / 3f;

            // 최소 1의 데미지는 보장
            damageAmount = Mathf.Max(1f, damageAmount);

            // 데미지 적용
            Player.TakeDamage(damageAmount);

            Debug.Log($"파도 데미지 적용: {damageAmount:F1}, 남은 체력: {Player.hp:F1}");
        }

        private IEnumerator ResetDamageFlag()
        {
            yield return new WaitForSeconds(damageInterval);
            _canDamage = true;
        }

        private IEnumerator DestroyAfterTime()
        {
            yield return new WaitForSeconds(lifeTime);

            // 파도 비활성화 또는 제거
            gameObject.SetActive(false);
        }
    }
}