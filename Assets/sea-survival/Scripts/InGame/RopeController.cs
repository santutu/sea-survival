using UnityEngine;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.InGame
{
    public class RopeController : MonoBehaviour
    {
        [Header("로프 설정")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform ropeStart; // 비행기 앵커 지점
        [SerializeField] private Transform ropeEnd; // 로프 끝 지점
        [SerializeField] private int ropeSegments = 10;
        [SerializeField] private float ropeWidth = 0.1f;
        
        [Header("물리 설정")]
        [SerializeField] private float gravity = 9.8f;
        [SerializeField] private float damping = 0.9f;
        
        [SerializeField, ReadOnly] private Vector3[] ropePoints;
        
        private void Awake()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
                if (lineRenderer == null)
                {
                    lineRenderer = gameObject.AddComponent<LineRenderer>();
                }
            }
            
            SetupLineRenderer();
            InitializeRope();
        }
        
        private void SetupLineRenderer()
        {
            lineRenderer.positionCount = ropeSegments + 1;
            lineRenderer.startWidth = ropeWidth;
            lineRenderer.endWidth = ropeWidth;
            lineRenderer.useWorldSpace = true;
            
            // 기본 머티리얼 설정 (갈색 로프)
            if (lineRenderer.material == null)
            {
                Material ropeMaterial = new Material(Shader.Find("Sprites/Default"));
                ropeMaterial.color = new Color(0.6f, 0.4f, 0.2f, 1f); // 갈색
                lineRenderer.material = ropeMaterial;
            }
        }
        
        private void InitializeRope()
        {
            ropePoints = new Vector3[ropeSegments + 1];
            
            // 로프 초기 위치 설정 (직선)
            for (int i = 0; i <= ropeSegments; i++)
            {
                float t = (float)i / ropeSegments;
                ropePoints[i] = Vector3.Lerp(transform.position, transform.position + Vector3.down, t);
            }
            
            UpdateLineRenderer();
        }
        
        public void SetRopePoints(Vector3 startPoint, Vector3 endPoint)
        {
            if (ropePoints == null || ropePoints.Length != ropeSegments + 1)
            {
                ropePoints = new Vector3[ropeSegments + 1];
            }
            
            // 시작점과 끝점 사이를 직선으로 분할
            for (int i = 0; i <= ropeSegments; i++)
            {
                float t = (float)i / ropeSegments;
                ropePoints[i] = Vector3.Lerp(startPoint, endPoint, t);
            }
            
            UpdateLineRenderer();
        }
        
        public void SetRopeLength(float length)
        {
            if (ropePoints == null) return;
            
            Vector3 startPoint = transform.position;
            Vector3 endPoint = startPoint + Vector3.down * length;
            
            SetRopePoints(startPoint, endPoint);
        }
        
        private void UpdateLineRenderer()
        {
            if (lineRenderer != null && ropePoints != null)
            {
                lineRenderer.positionCount = ropePoints.Length;
                lineRenderer.SetPositions(ropePoints);
            }
        }
        
        [Button("로프 테스트 - 직선")]
        public void TestStraightRope()
        {
            SetRopeLength(5f);
        }
        
        [Button("로프 테스트 - 곡선")]
        public void TestCurvedRope()
        {
            if (ropePoints == null) InitializeRope();
            
            // 중간 지점들에 약간의 곡선 효과 추가
            for (int i = 1; i < ropeSegments; i++)
            {
                float t = (float)i / ropeSegments;
                float curve = Mathf.Sin(t * Mathf.PI) * 0.5f; // 포물선 모양
                ropePoints[i] += Vector3.right * curve;
            }
            
            UpdateLineRenderer();
        }
        
        private void Update()
        {
            // 로프 시작점을 부모 오브젝트 위치에 맞춤
            if (ropePoints != null && ropePoints.Length > 0)
            {
                ropePoints[0] = transform.position;
                UpdateLineRenderer();
            }
        }
        
        // 외부에서 로프 전체 위치를 설정할 때 사용
        public void SetRopePosition(Vector3 newPosition)
        {
            transform.position = newPosition;
            
            if (ropePoints != null && ropePoints.Length > 0)
            {
                Vector3 offset = newPosition - ropePoints[0];
                for (int i = 0; i < ropePoints.Length; i++)
                {
                    ropePoints[i] += offset;
                }
                UpdateLineRenderer();
            }
        }
        
        // 로프 끝점 가져오기
        public Vector3 GetRopeEndPosition()
        {
            if (ropePoints != null && ropePoints.Length > 0)
            {
                return ropePoints[ropePoints.Length - 1];
            }
            return transform.position;
        }
    }
} 