using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace sea_survival.Scripts.Players
{
    public class BubbleController : MonoBehaviour
    {
        [Header("물리 설정")]
        public float oppositeForce = 2f; // 플레이어 이동 반대 방향으로 이동하는 힘
        public float waterResistance = 0.5f;
        public float randomMovement = 0.3f;

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private float lifetime;
        private float currentLife;
        private float initialSize;

        [SerializeField, ReadOnly]
        private Vector2 playerSpriteDirection; // 플레이어 sprite 방향
        
        [SerializeField, ReadOnly]
        private Vector2 bubbleDirection; // 거품 이동 방향 (디버그용)

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(BubbleSpeedConfig config, Vector3 position, Vector2 initialVelocity, Vector2 playerDirection)
        {
            transform.position = position;
            lifetime = config.lifetime;
            currentLife = 0f;
            playerSpriteDirection = playerDirection;

            // 디버그 로그
            Debug.Log($"Bubble Initialize - PlayerSpriteDirection: {playerDirection}, BubbleDirection: {playerDirection}");

            // 초기 속도 설정 (캐릭터의 sprite 방향으로)
            rb.linearVelocity = initialVelocity + GetRandomVelocity();

            // 크기와 색상 설정
            float randomSize = Random.Range(config.minSize, config.maxSize);
            initialSize = randomSize;
            transform.localScale = Vector3.one * 0.1f; // 작게 시작
            spriteRenderer.color = config.bubbleColor;

            StartCoroutine(BubbleLifecycle(config));
        }

        private IEnumerator BubbleLifecycle(BubbleSpeedConfig config)
        {
            while (currentLife < lifetime)
            {
                currentLife += Time.deltaTime;
                float lifeRatio = currentLife / lifetime;

                // 캐릭터의 sprite 방향으로 힘 적용
                bubbleDirection = playerSpriteDirection;
                if (bubbleDirection.magnitude < 0.1f) // sprite 방향이 명확하지 않으면 기본적으로 왼쪽
                    bubbleDirection = Vector2.left;
                rb.AddForce(bubbleDirection * oppositeForce * Time.deltaTime, ForceMode2D.Impulse);

                // 저항 적용
                rb.linearVelocity *= (1f - waterResistance * Time.deltaTime);

                // 랜덤 움직임
                Vector2 randomForce = GetRandomVelocity() * randomMovement * Time.deltaTime;
                rb.AddForce(randomForce, ForceMode2D.Impulse);

                // 크기 변화 (생성 → 성장 → 축소)
                float sizeMultiplier = GetSizeMultiplier(lifeRatio);
                transform.localScale = Vector3.one * initialSize * sizeMultiplier;

                // 투명도 변화
                Color color = spriteRenderer.color;
                color.a = GetAlphaMultiplier(lifeRatio) * config.alphaMultiplier;
                spriteRenderer.color = color;

                yield return null;
            }

            // 수명 완료 시 오브젝트 제거
            Destroy(gameObject);
        }

        private float GetSizeMultiplier(float lifeRatio)
        {
            if (lifeRatio < 0.3f)
                return Mathf.Lerp(0.1f, 1f, lifeRatio / 0.3f);  // 성장
            else if (lifeRatio > 0.8f)
                return Mathf.Lerp(1f, 0.8f, (lifeRatio - 0.8f) / 0.2f);  // 축소
            else
                return 1f;  // 유지
        }

        private float GetAlphaMultiplier(float lifeRatio)
        {
            if (lifeRatio > 0.7f)
                return Mathf.Lerp(1f, 0f, (lifeRatio - 0.7f) / 0.3f);  // 페이드 아웃
            else
                return 1f;
        }

        private Vector2 GetRandomVelocity()
        {
            // 캐릭터의 sprite 방향으로 편향된 랜덤 속도
            Vector2 spriteDirection = playerSpriteDirection;
            if (spriteDirection.magnitude < 0.1f) // sprite 방향이 명확하지 않으면 기본적으로 왼쪽
                spriteDirection = Vector2.left;

            return new Vector2(
                Random.Range(-0.5f, 0.5f) + spriteDirection.x * 0.3f, // sprite 방향 편향
                Random.Range(-0.5f, 0.5f) // Y축은 무작위로 유지
            );
        }
    }
}