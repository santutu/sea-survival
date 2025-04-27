using sea_survival.Scripts.Players;
using sea_survival.Scripts.Stages;
using UnityEngine;

namespace sea_survival.Scripts.StageSystem
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private ParticleSystem portalEffect;
        
        private Player Player => Player.Ins;
        
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
    }
} 