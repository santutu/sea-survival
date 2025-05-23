using UnityEngine;
using UnityEngine.UI;
using sea_survival.Scripts.Players;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.InGame
{
    public class MiniMapSystem : MonoBehaviour
    {
        [Header("미니맵 설정")]
        [SerializeField] private RectTransform miniMapContainer; // 미니맵 컨테이너
        [SerializeField] private Image miniMapBackground; // 미니맵 배경
        [SerializeField] private RectTransform playerIcon; // 플레이어 아이콘
        [SerializeField] private RectTransform portalIcon; // 포탈 아이콘 (optional)
        
        [Header("월드 범위")]
        [SerializeField] private float worldMinX = -20f;
        [SerializeField] private float worldMaxX = 20f;
        [SerializeField] private float worldMinY = -10f;
        [SerializeField] private float worldMaxY = 10f;
        
        [Header("미니맵 크기")]
        [SerializeField] private float miniMapWidth = 150f;
        [SerializeField] private float miniMapHeight = 100f;
        
        [Header("색상 설정")]
        [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0.2f, 0.8f);
        [SerializeField] private Color playerColor = Color.red;
        [SerializeField] private Color portalColor = Color.yellow;
        
        private Player _player;
        private Transform _portalTransform;
        
        private void Awake()
        {
            _player = Player.Ins;
        }
        
        private void Start()
        {
            SetupMiniMap();
        }
        
        private void Update()
        {
            UpdatePlayerPosition();
            UpdatePortalPosition();
        }
        
        private void SetupMiniMap()
        {
            if (miniMapContainer == null)
            {
                Debug.LogWarning("MiniMapContainer가 설정되지 않았습니다!");
                return;
            }
            
            // 미니맵 크기 설정
            miniMapContainer.sizeDelta = new Vector2(miniMapWidth, miniMapHeight);
            
            // 배경 색상 설정
            if (miniMapBackground != null)
            {
                miniMapBackground.color = backgroundColor;
            }
            
            // 플레이어 아이콘 색상 설정
            if (playerIcon != null)
            {
                Image playerImage = playerIcon.GetComponent<Image>();
                if (playerImage != null)
                {
                    playerImage.color = playerColor;
                }
            }
            
            // 포탈 아이콘 색상 설정
            if (portalIcon != null)
            {
                Image portalImage = portalIcon.GetComponent<Image>();
                if (portalImage != null)
                {
                    portalImage.color = portalColor;
                }
            }
        }
        
        private void UpdatePlayerPosition()
        {
            if (_player == null || playerIcon == null)
                return;
            
            Vector3 playerWorldPos = _player.transform.position;
            Vector2 miniMapPos = WorldToMiniMap(playerWorldPos);
            
            playerIcon.anchoredPosition = miniMapPos;
        }
        
        private void UpdatePortalPosition()
        {
            if (_portalTransform == null)
            {
                // 포탈 찾기 (Portal 태그를 가진 오브젝트)
                GameObject portalObj = GameObject.FindGameObjectWithTag("Portal");
                if (portalObj != null)
                {
                    _portalTransform = portalObj.transform;
                    
                    // 포탈 아이콘 활성화
                    if (portalIcon != null)
                    {
                        portalIcon.gameObject.SetActive(true);
                    }
                }
            }
            
            if (_portalTransform != null && portalIcon != null)
            {
                Vector3 portalWorldPos = _portalTransform.position;
                Vector2 miniMapPos = WorldToMiniMap(portalWorldPos);
                
                portalIcon.anchoredPosition = miniMapPos;
            }
        }
        
        private Vector2 WorldToMiniMap(Vector3 worldPosition)
        {
            // 월드 좌표를 0~1 범위로 정규화
            float normalizedX = Mathf.InverseLerp(worldMinX, worldMaxX, worldPosition.x);
            float normalizedY = Mathf.InverseLerp(worldMinY, worldMaxY, worldPosition.y);
            
            // 미니맵 좌표로 변환 (중심을 0,0으로)
            float miniMapX = (normalizedX - 0.5f) * miniMapWidth;
            float miniMapY = (normalizedY - 0.5f) * miniMapHeight;
            
            return new Vector2(miniMapX, miniMapY);
        }
        
        [Button("월드 범위 자동 설정")]
        public void AutoSetWorldBounds()
        {
            // 현재 씬의 콜라이더들을 기반으로 월드 범위 자동 설정
            Collider2D[] colliders = FindObjectsOfType<Collider2D>();
            
            if (colliders.Length == 0)
            {
                Debug.LogWarning("월드에 콜라이더가 없습니다!");
                return;
            }
            
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            
            foreach (var collider in colliders)
            {
                Bounds bounds = collider.bounds;
                minX = Mathf.Min(minX, bounds.min.x);
                maxX = Mathf.Max(maxX, bounds.max.x);
                minY = Mathf.Min(minY, bounds.min.y);
                maxY = Mathf.Max(maxY, bounds.max.y);
            }
            
            worldMinX = minX;
            worldMaxX = maxX;
            worldMinY = minY;
            worldMaxY = maxY;
            
            Debug.Log($"월드 범위 자동 설정: X({minX}, {maxX}), Y({minY}, {maxY})");
        }
        
        [Button("포탈 찾기")]
        public void FindPortal()
        {
            UpdatePortalPosition();
        }
    }
} 