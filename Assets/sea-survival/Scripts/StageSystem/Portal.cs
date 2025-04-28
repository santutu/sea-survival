using sea_survival.Scripts.Players;
using UnityEngine;
using System.Collections;

namespace sea_survival.Scripts.StageSystem
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private ParticleSystem portalEffect;
        [SerializeField] private float autoEnterTime = 10f; // 자동 진입까지 대기 시간(초)
        
        private Player Player => Player.Ins;
        private Coroutine autoEnterCoroutine;
        private bool isPlayerEntered = false;
        
        private void Start()
        {
            // 포탈 생성 시 자동 진입 타이머 시작
            autoEnterCoroutine = StartCoroutine(AutoEnterTimer());
        }
        
        private void Update()
        {
            // 포탈 회전 효과
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // 플레이어가 포탈에 들어왔는지 확인
            if (other.CompareTag("Player"))
            {
                OnPlayerEnterPortal();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 2D 콜라이더 지원
            if (other.CompareTag("Player"))
            {
                OnPlayerEnterPortal();
            }
        }
        
        private void OnPlayerEnterPortal()
        {
            // 자동 진입 타이머 중지
            if (autoEnterCoroutine != null)
            {
                StopCoroutine(autoEnterCoroutine);
                autoEnterCoroutine = null;
            }
            
            isPlayerEntered = true;
            
            // 포탈 효과 재생
            if (portalEffect != null)
            {
                portalEffect.Play();
            }
            
            // BattleStage에 플레이어 포탈 진입 알림
            BattleStage battleStage = FindObjectOfType<BattleStage>();
            if (battleStage != null)
            {
                battleStage.OnPlayerEnterPortal();
            }
            
            // 포탈 비활성화
            GetComponent<Collider2D>().enabled = false;
            
            // 애니메이션 및 효과 후 1초 뒤 포탈 파괴
            Destroy(gameObject, 1f);
        }
        
        private IEnumerator AutoEnterTimer()
        {
            Debug.Log($"포탈 자동 진입 타이머 시작: {autoEnterTime}초");
            
            // 설정된 시간만큼 대기
            yield return new WaitForSeconds(autoEnterTime);
            
            // 플레이어가 아직 포탈에 들어가지 않았다면 자동 진입 처리
            if (!isPlayerEntered)
            {
                Debug.Log("포탈 자동 진입 실행");
                
                // BattleStage에 플레이어 포탈 진입 알림
                BattleStage battleStage = FindObjectOfType<BattleStage>();
                if (battleStage != null)
                {
                    battleStage.OnPlayerEnterPortal();
                }
                
                // 포탈 비활성화 및 파괴
                GetComponent<Collider2D>().enabled = false;
                Destroy(gameObject, 1f);
            }
        }
    }
} 