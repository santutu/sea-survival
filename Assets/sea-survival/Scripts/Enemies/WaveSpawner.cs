using System.Collections;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts.Enemies
{
    public class WaveSpawner : SingletonMonoBehaviour<WaveSpawner>
    {
        [Header("파도 생성 설정")] [SerializeField] private GameObject wavePrefab;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private float spawnYOffset = 5f; // Y축 생성 위치 오프셋

        [Header("생성 위치")] [SerializeField] private Transform leftSpawnPoint;
        [SerializeField] private Transform rightSpawnPoint;
        
        [Header("기즈모 설정")]
        [SerializeField] private Color gizmoColor = new Color(0.2f, 0.8f, 1f, 0.5f); // 옅은 파란색
        [SerializeField] private float gizmoSphereRadius = 0.5f; // 기즈모 구체 크기

        private Coroutine _spawnCoroutine;

        private void OnEnable()
        {
            // 활성화될 때 스폰 시작
            StartSpawning();
        }

        private void OnDisable()
        {
            // 비활성화될 때 스폰 중지
            StopSpawning();
        }

        // 파도 스폰 시작
        public void StartSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }

            _spawnCoroutine = StartCoroutine(SpawnWavesRoutine());
        }

        // 파도 스폰 중지
        public void StopSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        // 파도 스폰 코루틴
        private IEnumerator SpawnWavesRoutine()
        {
            while (true)
            {
                // 랜덤 파도 생성
                SpawnWave();

                // 다음 생성까지 대기
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        // 파도 생성 함수
        private void SpawnWave()
        {
            // 왼쪽 또는 오른쪽에서 랜덤하게 생성
            bool spawnFromLeft = Random.Range(0, 2) == 0;

            Transform spawnPoint = spawnFromLeft ? leftSpawnPoint : rightSpawnPoint;
            Vector3 spawnPosition = spawnPoint.position;

            // Y축 위치 랜덤 조정
            spawnPosition.y += Random.Range(-spawnYOffset, spawnYOffset);

            // 파도 생성
            GameObject wave = Instantiate(wavePrefab, spawnPosition, Quaternion.identity);

            // 방향 설정
            Wave waveComponent = wave.GetComponent<Wave>();
            if (waveComponent != null)
            {
                if (spawnFromLeft)
                {
                    // 왼쪽에서 오른쪽으로 이동하는 파도
                    wave.transform.rotation = Quaternion.identity;
                }
                else
                {
                    // 오른쪽에서 왼쪽으로 이동하는 파도 (위아래 뒤집힘 방지)
                    // 180도 회전 대신 로컬 스케일의 x 값을 음수로 설정하여 방향만 뒤집음
                    wave.transform.rotation = Quaternion.identity;
                    wave.transform.localScale = new Vector3(-1, 1, 1);
                    
                    // 이동 방향도 반대로 설정
                    if (waveComponent != null)
                    {
                        waveComponent.SetMoveDirection(Vector3.left);
                    }
                }
            }

            Debug.Log($"파도 생성: {(spawnFromLeft ? "왼쪽" : "오른쪽")}에서 생성됨");
        }

        // 스폰 포인트가 없을 경우 게임 시작 시 생성
        private void Awake()
        {
            base.Awake();
            
            if (leftSpawnPoint == null)
            {
                GameObject left = new GameObject("LeftSpawnPoint");
                leftSpawnPoint = left.transform;
                leftSpawnPoint.position = new Vector3(-10f, 0f, 0f);
                leftSpawnPoint.parent = transform;
            }

            if (rightSpawnPoint == null)
            {
                GameObject right = new GameObject("RightSpawnPoint");
                rightSpawnPoint = right.transform;
                rightSpawnPoint.position = new Vector3(10f, 0f, 0f);
                rightSpawnPoint.parent = transform;
            }
        }
        
        // 기즈모 그리기 (항상 표시)
        private void OnDrawGizmos()
        {
            DrawSpawnPointGizmos();
        }

        // 선택 시 기즈모 그리기 (더 강조되어 표시)
        private void OnDrawGizmosSelected()
        {
            DrawSpawnPointGizmos(true);
        }
        
        // 스폰 위치 기즈모 그리기
        private void DrawSpawnPointGizmos(bool isSelected = false)
        {
            if (leftSpawnPoint == null || rightSpawnPoint == null) return;
            
            // 색상 설정
            Gizmos.color = isSelected ? Color.yellow : gizmoColor;
            
            // 왼쪽 스폰 포인트 기즈모
            DrawSpawnRangeGizmo(leftSpawnPoint.position);
            
            // 오른쪽 스폰 포인트 기즈모
            DrawSpawnRangeGizmo(rightSpawnPoint.position);
            
            // 선택되었을 때만 추가 정보 표시
            if (isSelected)
            {
                // 연결선 표시
                Gizmos.color = new Color(1f, 0.7f, 0f, 0.4f); // 반투명 주황색
                Gizmos.DrawLine(leftSpawnPoint.position + new Vector3(0, spawnYOffset, 0), 
                               rightSpawnPoint.position + new Vector3(0, spawnYOffset, 0));
                Gizmos.DrawLine(leftSpawnPoint.position - new Vector3(0, spawnYOffset, 0), 
                               rightSpawnPoint.position - new Vector3(0, spawnYOffset, 0));
            }
        }
        
        // 특정 위치에 스폰 범위 기즈모 그리기
        private void DrawSpawnRangeGizmo(Vector3 position)
        {
            // 중앙 지점
            Gizmos.DrawSphere(position, gizmoSphereRadius);
            
            // 상단 지점
            Gizmos.DrawSphere(position + new Vector3(0, spawnYOffset, 0), gizmoSphereRadius * 0.7f);
            
            // 하단 지점
            Gizmos.DrawSphere(position - new Vector3(0, spawnYOffset, 0), gizmoSphereRadius * 0.7f);
            
            // 중앙과 상/하단 연결선
            Gizmos.DrawLine(position, position + new Vector3(0, spawnYOffset, 0));
            Gizmos.DrawLine(position, position - new Vector3(0, spawnYOffset, 0));
        }
    }
}