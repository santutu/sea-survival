using UnityEngine;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.Players
{
    public class BubbleEffectManager : MonoBehaviour
    {
        [Header("공기방울 프리팹")]
        public GameObject bubblePrefab;

        [Header("속도별 설정")]
        public BubbleSpeedConfig[] speedConfigs = new BubbleSpeedConfig[5];

        [Header("발생 위치")]
        public Transform leftFootTransform;
        public Transform rightFootTransform;

        [Header("기본 설정")]
        public bool enableBubbleEffect = true;
        public float emissionInterval = 0.1f; // 발생 간격

        private PlayerSpeedDetector speedDetector;
        private Player player;
        private float lastEmissionTime;

        [SerializeField, ReadOnly]
        private bool isUnderwater = true;
        
        [SerializeField, ReadOnly]
        private int currentSpeedIndex = 0;

        private void Start()
        {
            speedDetector = GetComponent<PlayerSpeedDetector>();
            player = GetComponent<Player>();

            if (speedDetector == null)
            {
                speedDetector = gameObject.AddComponent<PlayerSpeedDetector>();
            }

            InitializeDefaultConfigs();
        }

        private void InitializeDefaultConfigs()
        {
            if (speedConfigs == null || speedConfigs.Length != 5)
            {
                speedConfigs = new BubbleSpeedConfig[5];
            }

            // 정지 상태 (속도 0)
            if (speedConfigs[0] == null) speedConfigs[0] = new BubbleSpeedConfig();


            // 느린 이동 (속도 1-3)
            if (speedConfigs[1] == null) speedConfigs[1] = new BubbleSpeedConfig();


            // 보통 이동 (속도 4-6)
            if (speedConfigs[2] == null) speedConfigs[2] = new BubbleSpeedConfig();


            // 빠른 이동 (속도 7-9)
            if (speedConfigs[3] == null) speedConfigs[3] = new BubbleSpeedConfig();

            // 최고속 이동 (속도 10+)
            if (speedConfigs[4] == null) speedConfigs[4] = new BubbleSpeedConfig();

        }

        private void Update()
        {
            if (!enableBubbleEffect || bubblePrefab == null) return;

            // 물 속에 있는지 확인
            CheckUnderwaterStatus();

            // 물 속에 있고 움직이고 있을 때만 공기방울 생성
            if (isUnderwater)
            {
                UpdateBubbleEffect();
            }
        }

        private void CheckUnderwaterStatus()
        {
            if (player != null)
            {
                // 호흡 중이라면 물 위(false), 호흡 중이 아니라면 물 속(true)
                isUnderwater = !player.IsBreathing;

                // 추가로 플레이어가 움직이고 있는지도 확인
                float currentSpeed = speedDetector.GetSmoothedSpeed();
                bool isMoving = currentSpeed > 0.1f; // 최소 속도 임계값

                // 물 속에 있고 움직이고 있을 때만 효과 활성화
                if (!isUnderwater || !isMoving)
                {
                    return;
                }
            }
        }

        private void UpdateBubbleEffect()
        {
            float currentSpeed = speedDetector.GetSmoothedSpeed();
            BubbleSpeedConfig config = GetSpeedConfig(currentSpeed);

            if (config != null && Time.time - lastEmissionTime >= emissionInterval)
            {
                // 발생률에 따라 공기방울 생성
                float emissionChance = config.emissionRate * emissionInterval;

                if (Random.value < emissionChance)
                {
                    SpawnBubble(config);
                    lastEmissionTime = Time.time;
                }
            }
        }

        private BubbleSpeedConfig GetSpeedConfig(float speed)
        {
            for (int i = 0; i < speedConfigs.Length; i++)
            {
                if (speedConfigs[i] != null && speed >= speedConfigs[i].minSpeed && speed <= speedConfigs[i].maxSpeed)
                {
                    currentSpeedIndex = i;
                    return speedConfigs[i];
                }
            }
            currentSpeedIndex = 0;
            return speedConfigs[0]; // 기본값 반환
        }

        private void SpawnBubble(BubbleSpeedConfig config)
        {
            // 발 위치 결정 (좌우 발 중 랜덤 선택)
            Transform footTransform = Random.value > 0.5f ? leftFootTransform : rightFootTransform;
            if (footTransform == null) footTransform = transform;

            // 이동 방향의 반대로 오프셋 적용
            Vector2 moveDirection = speedDetector.GetMoveDirection();
            Vector2 oppositeDirection = -moveDirection;

            // 발생 위치 계산
            Vector3 spawnPosition = footTransform.position + (Vector3)(oppositeDirection * 0f);
            spawnPosition += new Vector3(
                Random.Range(-0.3f, 0.3f), // X축 랜덤
                Random.Range(-0.2f, 0.2f), // Y축 랜덤
                0f
            );

            // 초기 속도 (플레이어 이동의 반대 방향으로 설정)
            Vector2 initialVelocity = oppositeDirection * 1.5f;

            // 공기방울 생성
            GameObject bubble = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);

            // BubbleController가 없으면 런타임에 추가
            BubbleController controller = bubble.GetComponent<BubbleController>();
            if (controller == null)
            {
                controller = bubble.AddComponent<BubbleController>();
            }

            // 기존 컴포넌트들을 공기방울에 맞게 조정
            SetupBubbleComponents(bubble, config);

            // 컨트롤러 초기화 (플레이어 이동 방향 전달)
            controller.Initialize(config, spawnPosition, initialVelocity, moveDirection);
        }

        private void SetupBubbleComponents(GameObject bubble, BubbleSpeedConfig config)
        {
            // Rigidbody2D 설정 조정
            Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.mass = 0.1f;
                rb.linearDamping = 1.5f; // 약간 줄여서 더 멀리 이동
                rb.angularDamping = 0.05f;
                rb.gravityScale = 0f; // 중력 비활성화 (왼쪽으로만 이동)
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            // SpriteRenderer 설정 조정
            SpriteRenderer sr = bubble.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10; // 다른 오브젝트 위에 표시
                sr.color = config.bubbleColor;
            }

            // Collider는 필요 없으므로 제거
            CapsuleCollider2D collider = bubble.GetComponent<CapsuleCollider2D>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }

            // 크기 조정
            float randomSize = Random.Range(config.minSize, config.maxSize);
            bubble.transform.localScale = Vector3.one * randomSize;
        }

        [Button("기본 속도별 설정 적용")]
        private void ApplyDefaultSpeedConfigs()
        {
            InitializeDefaultConfigs();
            Debug.Log("BubbleEffectManager: 기본 속도별 설정이 적용되었습니다!");
        }

        [Button("모든 설정 초기화")]
        private void ResetAllConfigs()
        {
            speedConfigs = new BubbleSpeedConfig[5];
            InitializeDefaultConfigs();
            Debug.Log("BubbleEffectManager: 모든 설정이 초기화되었습니다!");
        }

        [Button("Test Bubble Effect")]
        private void TestBubbleEffect()
        {
            if (bubblePrefab != null && speedConfigs.Length > 0)
            {
                SpawnBubble(speedConfigs[2]); // 보통 속도 테스트
            }
            else
            {
                Debug.LogWarning("BubbleEffectManager: bubblePrefab이 할당되지 않았거나 speedConfigs가 비어있습니다!");
            }
        }

        [Button("속도별 테스트 - 정지")]
        private void TestSpeed0()
        {
            if (bubblePrefab != null && speedConfigs.Length > 0)
                SpawnBubble(speedConfigs[0]);
        }

        [Button("속도별 테스트 - 느림")]
        private void TestSpeed1()
        {
            if (bubblePrefab != null && speedConfigs.Length > 1)
                SpawnBubble(speedConfigs[1]);
        }

        [Button("속도별 테스트 - 보통")]
        private void TestSpeed2()
        {
            if (bubblePrefab != null && speedConfigs.Length > 2)
                SpawnBubble(speedConfigs[2]);
        }

        [Button("속도별 테스트 - 빠름")]
        private void TestSpeed3()
        {
            if (bubblePrefab != null && speedConfigs.Length > 3)
                SpawnBubble(speedConfigs[3]);
        }

        [Button("속도별 테스트 - 최고속")]
        private void TestSpeed4()
        {
            if (bubblePrefab != null && speedConfigs.Length > 4)
                SpawnBubble(speedConfigs[4]);
        }

        [Button("프리셋 적용 - 절약형 (성능 우선)")]
        private void ApplyPerformancePreset()
        {
            ApplyCustomPreset(
                new int[] { 1, 3, 5, 8, 12 },          // 발생률 (낮춤)
                new float[] { 0.8f, 1.5f, 2f, 2.5f, 3f }, // 수명 (단축)
                new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f }, // 최소 크기
                new float[] { 0.15f, 0.3f, 0.45f, 0.6f, 0.8f } // 최대 크기
            );
            Debug.Log("BubbleEffectManager: 절약형 프리셋이 적용되었습니다! (성능 최적화)");
        }

        [Button("프리셋 적용 - 화려형 (시각 효과 우선)")]
        private void ApplyVisualPreset()
        {
            ApplyCustomPreset(
                new int[] { 5, 8, 12, 18, 25 },        // 발생률 (증가)
                new float[] { 2f, 2.5f, 3f, 3.5f, 4f }, // 수명 (연장)
                new float[] { 0.2f, 0.4f, 0.6f, 0.8f, 1f }, // 최소 크기
                new float[] { 0.4f, 0.6f, 0.8f, 1.2f, 1.5f } // 최대 크기
            );
            Debug.Log("BubbleEffectManager: 화려형 프리셋이 적용되었습니다! (시각 효과 강화)");
        }

        [Button("프리셋 적용 - 현실적 (물리 시뮬레이션 우선)")]
        private void ApplyRealisticPreset()
        {
            ApplyCustomPreset(
                new int[] { 2, 4, 6, 10, 15 },         // 발생률 (현실적)
                new float[] { 1.5f, 2f, 2.5f, 3f, 3.5f }, // 수명 (현실적)
                new float[] { 0.05f, 0.1f, 0.15f, 0.2f, 0.25f }, // 최소 크기 (작게)
                new float[] { 0.15f, 0.25f, 0.35f, 0.5f, 0.7f } // 최대 크기 (작게)
            );
            Debug.Log("BubbleEffectManager: 현실적 프리셋이 적용되었습니다! (물리 시뮬레이션 우선)");
        }

        private void ApplyCustomPreset(int[] emissionRates, float[] lifetimes, float[] minSizes, float[] maxSizes)
        {
            if (speedConfigs == null || speedConfigs.Length != 5)
            {
                speedConfigs = new BubbleSpeedConfig[5];
            }

            for (int i = 0; i < 5; i++)
            {
                if (speedConfigs[i] == null) speedConfigs[i] = new BubbleSpeedConfig();

                // 기본 속도 범위는 유지
                switch (i)
                {
                    case 0:
                        speedConfigs[i].minSpeed = 0f;
                        speedConfigs[i].maxSpeed = 0.5f;
                        break;
                    case 1:
                        speedConfigs[i].minSpeed = 0.5f;
                        speedConfigs[i].maxSpeed = 3f;
                        break;
                    case 2:
                        speedConfigs[i].minSpeed = 3f;
                        speedConfigs[i].maxSpeed = 6f;
                        break;
                    case 3:
                        speedConfigs[i].minSpeed = 6f;
                        speedConfigs[i].maxSpeed = 9f;
                        break;
                    case 4:
                        speedConfigs[i].minSpeed = 9f;
                        speedConfigs[i].maxSpeed = 999f;
                        break;
                }

                // 커스텀 값 적용
                speedConfigs[i].emissionRate = emissionRates[i];
                speedConfigs[i].lifetime = lifetimes[i];
                speedConfigs[i].minSize = minSizes[i];
                speedConfigs[i].maxSize = maxSizes[i];

                // 색상 및 투명도는 속도에 따라 자동 계산
                float alpha = 0.3f + (i * 0.175f); // 0.3 ~ 1.0
                speedConfigs[i].bubbleColor = new Color(1f, 1f, 1f, alpha);
                speedConfigs[i].alphaMultiplier = 0.5f + (i * 0.125f); // 0.5 ~ 1.0

                // 최고속에서만 특수 효과
                speedConfigs[i].hasSparkleEffect = (i == 4);
                speedConfigs[i].hasWaveEffect = (i == 4);
            }
        }

        [Button("발생률 +50% 증가")]
        private void IncreaseEmissionRate()
        {
            for (int i = 0; i < speedConfigs.Length; i++)
            {
                if (speedConfigs[i] != null)
                    speedConfigs[i].emissionRate = Mathf.RoundToInt(speedConfigs[i].emissionRate * 1.5f);
            }
            Debug.Log("BubbleEffectManager: 모든 속도의 발생률이 50% 증가했습니다!");
        }

        [Button("발생률 -25% 감소")]
        private void DecreaseEmissionRate()
        {
            for (int i = 0; i < speedConfigs.Length; i++)
            {
                if (speedConfigs[i] != null)
                    speedConfigs[i].emissionRate = Mathf.Max(1, Mathf.RoundToInt(speedConfigs[i].emissionRate * 0.75f));
            }
            Debug.Log("BubbleEffectManager: 모든 속도의 발생률이 25% 감소했습니다!");
        }

        [Button("크기 +25% 증가")]
        private void IncreaseBubbleSize()
        {
            for (int i = 0; i < speedConfigs.Length; i++)
            {
                if (speedConfigs[i] != null)
                {
                    speedConfigs[i].minSize *= 1.25f;
                    speedConfigs[i].maxSize *= 1.25f;
                }
            }
            Debug.Log("BubbleEffectManager: 모든 공기방울 크기가 25% 증가했습니다!");
        }

        [Button("크기 -20% 감소")]
        private void DecreaseBubbleSize()
        {
            for (int i = 0; i < speedConfigs.Length; i++)
            {
                if (speedConfigs[i] != null)
                {
                    speedConfigs[i].minSize = Mathf.Max(0.05f, speedConfigs[i].minSize * 0.8f);
                    speedConfigs[i].maxSize = Mathf.Max(0.1f, speedConfigs[i].maxSize * 0.8f);
                }
            }
            Debug.Log("BubbleEffectManager: 모든 공기방울 크기가 20% 감소했습니다!");
        }

        [Button("수명 +0.5초 연장")]
        private void IncreaseLifetime()
        {
            for (int i = 0; i < speedConfigs.Length; i++)
            {
                if (speedConfigs[i] != null)
                    speedConfigs[i].lifetime += 0.5f;
            }
            Debug.Log("BubbleEffectManager: 모든 공기방울 수명이 0.5초 연장되었습니다!");
        }

        [Button("수명 -0.3초 단축")]
        private void DecreaseLifetime()
        {
            for (int i = 0; i < speedConfigs.Length; i++)
            {
                if (speedConfigs[i] != null)
                    speedConfigs[i].lifetime = Mathf.Max(0.5f, speedConfigs[i].lifetime - 0.3f);
            }
            Debug.Log("BubbleEffectManager: 모든 공기방울 수명이 0.3초 단축되었습니다!");
        }

        public void SetBubbleEffectEnabled(bool enabled)
        {
            enableBubbleEffect = enabled;
        }

        // 디버그용 현재 속도 표시
        private void OnGUI()
        {
            if (speedDetector != null)
            {
                float speed = speedDetector.GetSmoothedSpeed();
                GUI.Label(new Rect(10, 50, 200, 20), $"Current Speed: {speed:F2}");
                GUI.Label(new Rect(10, 70, 200, 20), $"Underwater: {isUnderwater}");
                GUI.Label(new Rect(10, 90, 200, 20), $"Speed Index: {currentSpeedIndex}");
            }
        }
    }
}