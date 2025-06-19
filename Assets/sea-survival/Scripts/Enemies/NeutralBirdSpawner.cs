using UnityEngine;
using sea_survival.Scripts.StageSystem;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.Enemies
{
    public class NeutralBirdSpawner : MonoBehaviour
    {
        [Header("스폰 설정")]
        [SerializeField] private GameObject neutralBirdPrefab;
        [SerializeField] private int startStage = 2; // 몇 번째 스테이지부터 등장
        [SerializeField] private int maxBirds = 2; // 최대 새 개수
        [SerializeField] private float spawnInterval = 10f; // 스폰 간격
        
        [Header("스폰 영역")]
        [SerializeField] private float spawnMinX = -15f;
        [SerializeField] private float spawnMaxX = 15f;
        [SerializeField] private float spawnHeight = 10f;
        
        [SerializeField, ReadOnly] private float _spawnTimer = 0f;
        [SerializeField, ReadOnly] private int _currentBirdCount = 0;
        
        private void Start()
        {
            _spawnTimer = spawnInterval;
        }
        
        private void Update()
        {
            // 현재 스테이지가 시작 스테이지보다 낮으면 스폰하지 않음
            if (StageManager.Ins.CurrentStageLv < startStage)
                return;
            
            // 최대 새 개수에 도달했으면 스폰하지 않음
            if (_currentBirdCount >= maxBirds)
                return;
            
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnBird();
                _spawnTimer = spawnInterval;
            }
        }
        
        private void SpawnBird()
        {
            if (neutralBirdPrefab == null)
            {
                Debug.LogWarning("NeutralBirdPrefab이 설정되지 않았습니다!");
                return;
            }
            
            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnMinX, spawnMaxX),
                spawnHeight,
                0f
            );
            
            GameObject bird = Instantiate(neutralBirdPrefab, spawnPosition, Quaternion.identity);
            
            // 새가 파괴될 때 카운트 감소를 위한 이벤트 등록
            NeutralBird birdComponent = bird.GetComponent<NeutralBird>();
            if (birdComponent != null)
            {
                StartCoroutine(WaitForBirdDestroy(bird));
            }
            
            _currentBirdCount++;
            Debug.Log($"중립새 스폰! 현재 새 개수: {_currentBirdCount}");
        }
        
        private System.Collections.IEnumerator WaitForBirdDestroy(GameObject bird)
        {
            // 새가 파괴될 때까지 대기
            while (bird != null)
            {
                yield return null;
            }
            
            _currentBirdCount--;
            Debug.Log($"중립새 파괴됨. 현재 새 개수: {_currentBirdCount}");
        }
        
        [Button("강제 스폰")]
        public void ForceSpawn()
        {
            if (_currentBirdCount < maxBirds)
            {
                SpawnBird();
            }
        }
        
        [Button("모든 새 제거")]
        public void ClearAllBirds()
        {
            NeutralBird[] birds = FindObjectsOfType<NeutralBird>();
            int destroyedCount = 0;
            
            foreach (var bird in birds)
            {
                if (bird != null && bird.gameObject != null)
                {
                    Destroy(bird.gameObject);
                    destroyedCount++;
                }
            }
            
            _currentBirdCount = 0;
            Debug.Log($"모든 중립새 제거 완료 ({destroyedCount}마리)");
        }
        
        // 스포너 정지 (엔딩 연출용)
        public void StopSpawning()
        {
            enabled = false;
            Debug.Log("중립새 스포너 정지");
        }
        
        // 스포너 재시작
        public void StartSpawning()
        {
            enabled = true;
            Debug.Log("중립새 스포너 재시작");
        }
        
        // 스포너 완전 정지 및 새 제거
        public void StopAndClearAll()
        {
            StopSpawning();
            ClearAllBirds();
            Debug.Log("새 스포너 정지 및 모든 새 제거");
        }
    }
} 