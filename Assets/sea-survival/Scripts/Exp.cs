using UnityEngine;
using System.Collections;

namespace sea_survival.Scripts
{
    public class Exp : MonoBehaviour
    {
        [Header("설정")] [SerializeField] private int expValue = 10; // 경험치 값
        [SerializeField] private float moveSpeed = 3f; // 플레이어 방향으로 이동 속도
        [SerializeField] private float collectDistance = 1.5f; // 플레이어가 획득할 수 있는 거리
        [SerializeField] private float attractDistance = 5f; // 플레이어를 감지하는 거리
        [SerializeField] private float lifeTime = 15f; // 경험치 아이템 수명

        [Header("시각 효과")] [SerializeField] private SpriteRenderer spriteRenderer; // Exp 아이템 스프라이트

        private Transform PlayerTransform => Player.Ins.transform;
        private float _timer;
        private bool _isAttracting = false;

        private void Awake()
        {
            // 스프라이트 렌더러가 없으면 자동으로 찾기
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // 랜덤한 위치로 약간 튕겨나가는 효과
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(-1f, 1f);
            Vector2 randomDir = new Vector2(randomX, randomY).normalized;
            GetComponent<Rigidbody2D>()?.AddForce(randomDir * 2f, ForceMode2D.Impulse);

            // 수명 타이머 시작
            _timer = 0f;
        }

        private void Update()
        {
            // 수명 증가 및 확인
            _timer += Time.deltaTime;
            if (_timer > lifeTime)
            {
                // 수명이 다 되면 깜빡이는 효과 후 삭제
                StartCoroutine(FadeAndDestroy());
                return;
            }

            // 수명의 3/4이 지나면 깜빡이는 효과
            if (_timer > lifeTime * 0.75f && !_isAttracting)
            {
                StartCoroutine(BlinkEffect());
            }

            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerTransform.position);

            // 플레이어가 획득 거리 내에 있으면
            if (distanceToPlayer < collectDistance)
            {
                Collect();
                return;
            }

            // 플레이어가 감지 거리 내에 있으면 플레이어 쪽으로 이동
            if (distanceToPlayer < attractDistance || _isAttracting)
            {
                _isAttracting = true;
                MoveTowardsPlayer(distanceToPlayer);
            }
        }

        // 플레이어 방향으로 이동
        private void MoveTowardsPlayer(float distance)
        {
            // 가까울수록 빨리 이동
            float speedMultiplier = Mathf.Clamp(1f + (attractDistance - distance) / attractDistance, 1f, 3f);

            Vector2 direction = (PlayerTransform.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * speedMultiplier * Time.deltaTime);
        }

        // 경험치 획득 처리
        private void Collect()
        {
            // 플레이어 레벨 시스템 가져오기


            // 경험치 추가
            PlayerLevelSystem.Ins.AddExperience(expValue);

            // 획득 효과 (소리나 파티클 효과 등)
            // TODO: 획득 효과 추가

            // 아이템 제거
            Destroy(gameObject);
        }

        // 깜빡이는 효과
        private IEnumerator BlinkEffect()
        {
            // 아직 플레이어에게 끌려가고 있지 않을 때만 깜빡임
            if (!_isAttracting)
            {
                for (int i = 0; i < 3; i++)
                {
                    spriteRenderer.enabled = false;
                    yield return new WaitForSeconds(0.1f);
                    spriteRenderer.enabled = true;
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        // 페이드 아웃 효과 후 제거
        private IEnumerator FadeAndDestroy()
        {
            float duration = 1f;
            float elapsed = 0f;
            Color originalColor = spriteRenderer.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            Destroy(gameObject);
        }

        // 경험치 값 설정 (몬스터 종류나 레벨에 따라 다른 값 설정 가능)
        public void SetExpValue(int value)
        {
            expValue = value;
        }
    }
}