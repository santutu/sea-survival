
using System.Collections;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.StageSystem;
using sea_survival.Scripts.Weapons;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.CardSystem;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.InGame
{
    public class EndingSequenceManager : SingletonMonoBehaviour<EndingSequenceManager>
    {
        [Header("비행기 설정")]
        [SerializeField] private GameObject airplaneObject;
        [SerializeField] private float airplaneSpeed = 5f;
        [SerializeField] private Vector3 airplaneStartPosition = new Vector3(-15f, 25f, 0f);
        [SerializeField] private Vector3 airplaneTargetPosition = new Vector3(0f, 25f, 0f);
        
        [Header("로프 설정")]
        [SerializeField] private GameObject ropePrefab;
        [SerializeField] private Transform ropeAnchorPoint; // 비행기에서 로프가 나오는 지점
        
        [Header("카메라 설정")]
        [SerializeField] private float cameraHeightOffset = 20f;
        [SerializeField] private float cameraTransitionDuration = 2f;
        
        [Header("페이드 아웃 설정")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeOutDuration = 2f;
        
        [Header("타이밍 설정")]
        [SerializeField] private float playerMoveToRopeDuration = 2f;
        [SerializeField] private float rescueAscentDuration = 3f;
        [SerializeField] private float ropeDeployDuration = 2f;
        
        [Header("구조 설정")]
        [SerializeField] private float rescueHeightOffset = 2f; // 비행기 아래로부터의 거리
        
        [SerializeField, ReadOnly] private bool _isSequenceRunning = false;
        
        private GameObject _currentRope;
        private Vector3 _originalCameraPosition;
        private Vector3 _originalPlayerPosition;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 페이드 이미지가 없다면 생성
            if (fadeImage == null)
            {
                CreateFadeImage();
            }
            
            // 로프 앵커 포인트가 설정되지 않았다면 비행기 중심으로 설정
            if (ropeAnchorPoint == null && airplaneObject != null)
            {
                ropeAnchorPoint = airplaneObject.transform;
            }
        }
        
        [Button("엔딩 시퀀스 테스트")]
        public void StartEndingSequence()
        {
            if (_isSequenceRunning)
            {
                Debug.LogWarning("엔딩 시퀀스가 이미 실행 중입니다.");
                return;
            }
            
            StartCoroutine(ExecuteEndingSequence());
        }
        
        private IEnumerator ExecuteEndingSequence()
        {
            _isSequenceRunning = true;
            
            // 1단계: 캐릭터 컨트롤 비활성화
            DisablePlayerControl();
            
            // 2단계: 카메라 하늘로 전환 (2초)
            yield return StartCoroutine(MoveCameraToSky());
            
            // 3단계: 비행기 등장 (3초)
            yield return StartCoroutine(MoveAirplaneToCenter());
            
            // 대기 시간 (0.5초)
            yield return new WaitForSeconds(0.5f);
            
            // 4단계: 로프 투하 (2초)
            yield return StartCoroutine(DeployRope());
            
            // 5단계: 캐릭터 로프로 이동 (2초)
            yield return StartCoroutine(MovePlayerToRope());
            
            // 6단계: 구조 완료 (3초 + 페이드아웃)
            yield return StartCoroutine(RescuePlayer());
            
            // Victory UI 표시
            ShowVictoryUI();
            
            _isSequenceRunning = false;
        }
        
        private void DisablePlayerControl()
        {
            var player = Player.Ins;
            if (player != null)
            {
                player.enabled = false;
                _originalPlayerPosition = player.transform.position;
                
                // 플레이어 강제로 보이게 하기
                player.ForceVisible();
                
                // 거품 효과 비활성화
                var bubbleEffectManager = player.GetComponent<BubbleEffectManager>();
                if (bubbleEffectManager != null)
                {
                    bubbleEffectManager.SetBubbleEffectEnabled(false);
                    Debug.Log("거품 효과 비활성화");
                }
                
                Debug.Log("플레이어 컨트롤 비활성화");
            }
            
            // 무기 시스템 비활성화
            var weaponManager = WeaponManager.Ins;
            if (weaponManager != null)
            {
                weaponManager.SetWeaponsEnabled(false);
                Debug.Log("무기 시스템 비활성화");
            }
            
            // 적 스포너 정지
            var enemySpawners = EnemyAllSpawners.Ins;
            if (enemySpawners != null)
            {
                enemySpawners.StopAllSpawners();
                Debug.Log("적 스포너 정지");
            }
            
            // 모든 적 제거
            KillAllEnemies();
            
            // 파도 스포너 정지 및 파도 제거
            var waveSpawner = WaveSpawner.Ins;
            if (waveSpawner != null)
            {
                waveSpawner.StopAndClearAll();
                Debug.Log("파도 시스템 정지");
            }
            
            // 중립새 스포너 정지 및 새 제거
            var neutralBirdSpawner = FindObjectOfType<NeutralBirdSpawner>();
            if (neutralBirdSpawner != null)
            {
                neutralBirdSpawner.StopAndClearAll();
                Debug.Log("중립새 시스템 정지");
            }
            
            // 경험치 시스템 정지
            var levelSystem = PlayerLevelSystem.Ins;
            if (levelSystem != null)
            {
                levelSystem.StopLevelSystem();
                Debug.Log("경험치 시스템 정지");
            }
            
            // 카드 시스템 정지 및 UI 닫기
            var cardManager = CardManager.Ins;
            if (cardManager != null)
            {
                cardManager.StopCardSystem();
                Debug.Log("카드 시스템 정지");
            }
            
            // 카메라 컨트롤러도 비활성화
            var cameraController = CameraController.Ins;
            if (cameraController != null)
            {
                cameraController.enabled = false;
            }
        }
        
        private IEnumerator MoveCameraToSky()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null) yield break;
            
            _originalCameraPosition = mainCamera.transform.position;
            var targetPosition = new Vector3(
                _originalCameraPosition.x,
                _originalCameraPosition.y + cameraHeightOffset,
                _originalCameraPosition.z
            );
            
            float elapsedTime = 0f;
            
            while (elapsedTime < cameraTransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / cameraTransitionDuration;
                
                mainCamera.transform.position = Vector3.Lerp(_originalCameraPosition, targetPosition, t);
                
                yield return null;
            }
            
            mainCamera.transform.position = targetPosition;
            Debug.Log("카메라 하늘로 이동 완료");
        }
        
        private IEnumerator MoveAirplaneToCenter()
        {
            if (airplaneObject == null) yield break;
            
            // 비행기 활성화 및 시작 위치 설정
            airplaneObject.SetActive(true);
            
            // 플레이어 위치 기준으로 Inspector 설정값 적용 (Y값은 절대값)
            var playerPos = _originalPlayerPosition;
            var startPos = new Vector3(
                playerPos.x + airplaneStartPosition.x, 
                airplaneStartPosition.y, 
                playerPos.z + airplaneStartPosition.z
            );
            var targetPos = new Vector3(
                playerPos.x + airplaneTargetPosition.x, 
                airplaneTargetPosition.y, 
                playerPos.z + airplaneTargetPosition.z
            );
            
            airplaneObject.transform.position = startPos;
            
            // 거리 기반으로 이동 시간 계산 (airplaneSpeed 적용)
            float distance = Vector3.Distance(startPos, targetPos);
            float moveDuration = distance / airplaneSpeed;
            
            float elapsedTime = 0f;
            
            while (elapsedTime < moveDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / moveDuration;
                
                airplaneObject.transform.position = Vector3.Lerp(startPos, targetPos, t);
                
                yield return null;
            }
            
            airplaneObject.transform.position = targetPos;
            Debug.Log($"비행기 중앙 이동 완료 (거리: {distance:F1}, 시간: {moveDuration:F1}초, 속도: {airplaneSpeed})");
        }
        
        private IEnumerator DeployRope()
        {
            if (ropePrefab == null || ropeAnchorPoint == null) yield break;
            
            // 로프 생성
            _currentRope = Instantiate(ropePrefab, ropeAnchorPoint.position, Quaternion.identity);
            var ropeController = _currentRope.GetComponent<RopeController>();
            
            // 플레이어 위치까지의 거리 계산
            var playerPos = _originalPlayerPosition;
            var ropeStartPos = ropeAnchorPoint.position;
            var ropeLength = Vector3.Distance(ropeStartPos, new Vector3(playerPos.x, playerPos.y, playerPos.z));
            
            float elapsedTime = 0f;
            
            // RopeController를 사용하여 로프 투하
            if (ropeController != null)
            {
                while (elapsedTime < ropeDeployDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / ropeDeployDuration;
                    
                    // 현재 로프 길이 계산
                    float currentLength = Mathf.Lerp(0.1f, ropeLength, t);
                    ropeController.SetRopeLength(currentLength);
                    
                    yield return null;
                }
                
                ropeController.SetRopeLength(ropeLength);
            }
            else
            {
                // RopeController가 없다면 기본 Transform 기반 처리
                var ropeTransform = _currentRope.transform;
                var initialScale = ropeTransform.localScale;
                var targetScale = new Vector3(initialScale.x, ropeLength, initialScale.z);
                
                while (elapsedTime < ropeDeployDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / ropeDeployDuration;
                    
                    ropeTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
                    
                    // 로프 위치도 조정 (아래로 내려가면서)
                    var currentPos = Vector3.Lerp(ropeStartPos, 
                        new Vector3(ropeStartPos.x, playerPos.y + ropeLength/2, ropeStartPos.z), t);
                    ropeTransform.position = currentPos;
                    
                    yield return null;
                }
                
                ropeTransform.localScale = targetScale;
            }
            
            Debug.Log("로프 투하 완료");
        }
        
        private IEnumerator MovePlayerToRope()
        {
            var player = Player.Ins;
            if (player == null || _currentRope == null) yield break;
            
            // 플레이어 애니메이션을 idle로 설정
            player.SetAnimation(AnimState.IsMoving, false);
            player.SetAnimation(AnimState.IsFalling, false);
            player.SetAnimation(AnimState.IsIdle, true);
            
            var startPos = player.transform.position;
            var ropePos = _currentRope.transform.position;
            var targetPos = new Vector3(ropePos.x, startPos.y, ropePos.z);
            
            float elapsedTime = 0f;
            
            while (elapsedTime < playerMoveToRopeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / playerMoveToRopeDuration;
                
                player.transform.position = Vector3.Lerp(startPos, targetPos, t);
                
                yield return null;
            }
            
            player.transform.position = targetPos;
            Debug.Log("플레이어 로프 이동 완료 - Idle 애니메이션 적용");
        }
        
        private IEnumerator RescuePlayer()
        {
            var player = Player.Ins;
            if (player == null || _currentRope == null || airplaneObject == null) yield break;
            
            // 구조 과정에서도 idle 애니메이션 유지
            player.SetAnimation(AnimState.IsMoving, false);
            player.SetAnimation(AnimState.IsFalling, false);
            player.SetAnimation(AnimState.IsIdle, true);
            
            var startPlayerPos = player.transform.position;
            var startRopePos = _currentRope.transform.position;
            var airplanePos = airplaneObject.transform.position;
            
            var targetPlayerPos = new Vector3(airplanePos.x, airplanePos.y - rescueHeightOffset, airplanePos.z);
            var targetRopePos = new Vector3(airplanePos.x, airplanePos.y - (rescueHeightOffset / 2f), airplanePos.z);
            
            // 페이드아웃 시작
            StartCoroutine(FadeOut());
            
            float elapsedTime = 0f;
            
            while (elapsedTime < rescueAscentDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / rescueAscentDuration;
                
                // 플레이어와 로프를 함께 위로 이동
                player.transform.position = Vector3.Lerp(startPlayerPos, targetPlayerPos, t);
                _currentRope.transform.position = Vector3.Lerp(startRopePos, targetRopePos, t);
                
                yield return null;
            }
            
            player.transform.position = targetPlayerPos;
            _currentRope.transform.position = targetRopePos;
            
            Debug.Log("구조 완료 - Idle 애니메이션 유지");
        }
        
        private IEnumerator FadeOut()
        {
            if (fadeImage == null) yield break;
            
            fadeImage.gameObject.SetActive(true);
            var color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
            
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fadeOutDuration;
                
                color.a = t;
                fadeImage.color = color;
                
                yield return null;
            }
            
            color.a = 1f;
            fadeImage.color = color;
        }
        
        private void ShowVictoryUI()
        {
            // 거품 효과 복구 (Victory UI 표시 전에)
            var player = Player.Ins;
            if (player != null)
            {
                var bubbleEffectManager = player.GetComponent<BubbleEffectManager>();
                if (bubbleEffectManager != null)
                {
                    bubbleEffectManager.SetBubbleEffectEnabled(true);
                    Debug.Log("엔딩 완료 - 거품 효과 복구");
                }
            }
            
            // 무기 시스템 복구
            var weaponManager = WeaponManager.Ins;
            if (weaponManager != null)
            {
                weaponManager.SetWeaponsEnabled(true);
                Debug.Log("엔딩 완료 - 무기 시스템 복구");
            }
            
            // 적 스포너 복구
            var enemySpawners = EnemyAllSpawners.Ins;
            if (enemySpawners != null)
            {
                enemySpawners.StartAllSpawners();
                Debug.Log("엔딩 완료 - 적 스포너 복구");
            }
            
            // 경험치 시스템 복구
            var levelSystem = PlayerLevelSystem.Ins;
            if (levelSystem != null)
            {
                levelSystem.StartLevelSystem();
                Debug.Log("엔딩 완료 - 경험치 시스템 복구");
            }
            
            // 카드 시스템 복구
            var cardManager = CardManager.Ins;
            if (cardManager != null)
            {
                cardManager.StartCardSystem();
                Debug.Log("엔딩 완료 - 카드 시스템 복구");
            }
            
            // 파도 스포너 복구
            var waveSpawner = WaveSpawner.Ins;
            if (waveSpawner != null)
            {
                waveSpawner.StartSpawning();
                Debug.Log("엔딩 완료 - 파도 시스템 복구");
            }
            
            // 중립새 스포너 복구
            var neutralBirdSpawner = FindObjectOfType<NeutralBirdSpawner>();
            if (neutralBirdSpawner != null)
            {
                neutralBirdSpawner.StartSpawning();
                Debug.Log("엔딩 완료 - 중립새 시스템 복구");
            }
            
            StageManager.Ins.GameVictory();
            Debug.Log("Victory UI 표시");
        }
        
        // 모든 적 제거
        private void KillAllEnemies()
        {
            // 씬에 있는 모든 Enemy 컴포넌트 찾기
            Enemy[] allEnemies = FindObjectsOfType<Enemy>();
            
            int killedCount = 0;
            foreach (var enemy in allEnemies)
            {
                if (enemy != null && enemy.gameObject != null)
                {
                    // 보스는 제외 (이미 죽은 상태)
                    if (enemy.GetComponent<Boss>() == null)
                    {
                        Destroy(enemy.gameObject);
                        killedCount++;
                    }
                }
            }
            
            Debug.Log($"모든 적 제거 완료 ({killedCount}마리)");
        }
        
        private void CreateFadeImage()
        {
            // Canvas 찾기
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas를 찾을 수 없습니다. 페이드 효과를 적용할 수 없습니다.");
                return;
            }
            
            // 페이드 이미지 생성
            GameObject fadeGO = new GameObject("FadeImage");
            fadeGO.transform.SetParent(canvas.transform, false);
            
            Image image = fadeGO.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            
            RectTransform rectTransform = fadeGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            fadeImage = image;
            fadeGO.SetActive(false);
        }
        
        private void OnValidate()
        {
            // 비행기 오브젝트가 설정되지 않았다면 FallingCinematicManager에서 찾기 시도
            if (airplaneObject == null)
            {
                var fallingCinematic = FindObjectOfType<FallingCinematicManager>();
                if (fallingCinematic != null)
                {
                    // FallingCinematicManager의 airPlainObj 필드에 접근
                    var field = fallingCinematic.GetType().GetField("airPlainObj", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null)
                    {
                        airplaneObject = field.GetValue(fallingCinematic) as GameObject;
                    }
                }
            }
        }
        
        // 엔딩 시퀀스 중 게임이 종료되거나 문제가 발생했을 때 정리
        private void OnDestroy()
        {
            if (_currentRope != null)
            {
                Destroy(_currentRope);
            }
            
            // 카메라 컨트롤러 복구
            var cameraController = CameraController.Ins;
            if (cameraController != null)
            {
                cameraController.enabled = true;
            }
            
            // 무기 시스템 복구
            var weaponManager = WeaponManager.Ins;
            if (weaponManager != null)
            {
                weaponManager.SetWeaponsEnabled(true);
                Debug.Log("비상 상황 - 무기 시스템 복구");
            }
            
            // 적 스포너 복구
            var enemySpawners = EnemyAllSpawners.Ins;
            if (enemySpawners != null)
            {
                enemySpawners.StartAllSpawners();
                Debug.Log("비상 상황 - 적 스포너 복구");
            }
            
            // 경험치 시스템 복구
            var levelSystem = PlayerLevelSystem.Ins;
            if (levelSystem != null)
            {
                levelSystem.StartLevelSystem();
                Debug.Log("비상 상황 - 경험치 시스템 복구");
            }
            
            // 카드 시스템 복구
            var cardManager = CardManager.Ins;
            if (cardManager != null)
            {
                cardManager.StartCardSystem();
                Debug.Log("비상 상황 - 카드 시스템 복구");
            }
            
            // 파도 스포너 복구
            var waveSpawner = WaveSpawner.Ins;
            if (waveSpawner != null)
            {
                waveSpawner.StartSpawning();
                Debug.Log("비상 상황 - 파도 시스템 복구");
            }
            
            // 중립새 스포너 복구
            var neutralBirdSpawner = FindObjectOfType<NeutralBirdSpawner>();
            if (neutralBirdSpawner != null)
            {
                neutralBirdSpawner.StartSpawning();
                Debug.Log("비상 상황 - 중립새 시스템 복구");
            }
            
            // 플레이어 컨트롤 복구
            var player = Player.Ins;
            if (player != null)
            {
                player.enabled = true;
                
                // 거품 효과 복구
                var bubbleEffectManager = player.GetComponent<BubbleEffectManager>();
                if (bubbleEffectManager != null)
                {
                    bubbleEffectManager.SetBubbleEffectEnabled(true);
                    Debug.Log("거품 효과 복구");
                }
            }
        }
        
        // 디버깅용 Gizmos
        private void OnDrawGizmos()
        {
            if (airplaneObject != null)
            {
                // 비행기 경로 표시 (Inspector 설정값 반영, Y값은 절대값)
                Gizmos.color = Color.cyan;
                var playerPos = Player.Ins != null ? Player.Ins.transform.position : Vector3.zero;
                var startPos = new Vector3(
                    playerPos.x + airplaneStartPosition.x, 
                    airplaneStartPosition.y, 
                    playerPos.z + airplaneStartPosition.z
                );
                var targetPos = new Vector3(
                    playerPos.x + airplaneTargetPosition.x, 
                    airplaneTargetPosition.y, 
                    playerPos.z + airplaneTargetPosition.z
                );
                
                Gizmos.DrawLine(startPos, targetPos);
                Gizmos.DrawWireSphere(startPos, 1f);
                Gizmos.DrawWireSphere(targetPos, 1f);
                
                // 로프 투하 지점 표시
                if (ropeAnchorPoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(targetPos, playerPos);
                }
            }
        }
    }
} 