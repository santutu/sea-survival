using UnityEngine;
using UnityEngine.UI;
using sea_survival.Scripts.Players;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.InGame
{
    public class MiniMapSystem : MonoBehaviour
    {
        [Header("미니맵 카메라 설정")]
        [SerializeField] private Camera miniMapCamera; // 미니맵용 카메라
        [SerializeField] private RenderTexture renderTexture; // 렌더 텍스처
        [SerializeField] private RawImage miniMapDisplay; // 미니맵 UI 디스플레이
        
        [Header("미니맵 크기 및 위치")]
        
        [Header("카메라 설정")]
        [SerializeField] private LayerMask miniMapLayers = -1; // 미니맵에 표시할 레이어
        
        [Header("카메라 따라가기 설정")]
        [SerializeField] private bool followPlayer = true; // 플레이어 따라가기
        [SerializeField] private float followSpeed = 5f; // 따라가기 속도 (0이면 즉시)
        
        private Player _player;
        private RectTransform _miniMapRect;
        
        private void Awake()
        {
            _player = Player.Ins;
            _miniMapRect = miniMapDisplay.GetComponent<RectTransform>();
        }
        
        private void Start()
        {
            SetupMiniMap();
        }
        
        private void LateUpdate()
        {
            if (followPlayer && _player != null && miniMapCamera != null)
            {
                // UpdateCameraPosition();
            }
        }
        
        private void SetupMiniMap()
        {
            // RenderTexture 생성 (없는 경우)
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(256, 256, 16);
                renderTexture.Create();
            }
            
            // 미니맵 카메라 설정
            if (miniMapCamera != null)
            {
                miniMapCamera.targetTexture = renderTexture;
                miniMapCamera.orthographic = true;
                miniMapCamera.cullingMask = miniMapLayers;
                miniMapCamera.clearFlags = CameraClearFlags.SolidColor;
            }
            
            // UI 설정
            if (miniMapDisplay != null)
            {
                miniMapDisplay.texture = renderTexture;
            }
        }
        
        
        
        [Button("미니맵 카메라 생성")]
        public void CreateMiniMapCamera()
        {
            // 기존 미니맵 카메라 제거
            if (miniMapCamera != null)
            {
                DestroyImmediate(miniMapCamera.gameObject);
            }
            
            // 새 카메라 오브젝트 생성
            GameObject cameraObj = new GameObject("MiniMapCamera");
            cameraObj.transform.SetParent(transform);
            
            // 카메라 컴포넌트 추가 및 설정
            miniMapCamera = cameraObj.AddComponent<Camera>();
            
            // 자동 설정 실행
            SetupMiniMap();
            
            Debug.Log("미니맵 카메라가 생성되었습니다!");
        }
        
        [Button("RenderTexture 생성")]
        public void CreateRenderTexture()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }
            
            renderTexture = new RenderTexture(256, 256, 16);
            renderTexture.Create();
            
            if (miniMapDisplay != null)
            {
                miniMapDisplay.texture = renderTexture;
            }
            
            if (miniMapCamera != null)
            {
                miniMapCamera.targetTexture = renderTexture;
            }
            
            Debug.Log("RenderTexture가 생성되었습니다!");
        }
        
        [Button("미니맵 테스트")]
        public void TestMiniMap()
        {
            if (miniMapCamera == null)
            {
                Debug.LogWarning("미니맵 카메라가 없습니다!");
                return;
            }
            
            if (renderTexture == null)
            {
                Debug.LogWarning("RenderTexture가 없습니다!");
                return;
            }
            
            if (miniMapDisplay == null)
            {
                Debug.LogWarning("미니맵 디스플레이가 없습니다!");
                return;
            }
            
            Debug.Log("미니맵 시스템이 정상적으로 설정되었습니다!");
            Debug.Log($"카메라 위치: {miniMapCamera.transform.position}");
            Debug.Log($"RenderTexture 크기: {renderTexture.width}x{renderTexture.height}");
        }
        
        // 정리
        private void OnDestroy()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }
        }
    }
} 