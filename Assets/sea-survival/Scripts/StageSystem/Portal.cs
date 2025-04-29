using sea_survival.Scripts.Players;
using UnityEngine;
using System.Collections;
using Santutu.Core.Base.Runtime.Singletons;

namespace sea_survival.Scripts.StageSystem
{
    public class Portal : SingletonMonoBehaviour<Portal>
    {
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private ParticleSystem portalEffect;
        [SerializeField] private float autoEnterTime = 10f; // 자동 진입까지 대기 시간(초)

        [SerializeField] public GameObject startPoint;

        private Player Player => Player.Ins;
        private bool isPlayerEntered = false;

        private void Update()
        {
            // 포탈 회전 효과
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
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
            isPlayerEntered = true;

            // 포탈 효과 재생
            if (portalEffect != null)
            {
                portalEffect.Play();
            }

            // BattleStage에 플레이어 포탈 진입 알림
            BattleStageState battleStage = FindObjectOfType<BattleStageState>();
            if (battleStage != null)
            {
              
                battleStage.OnPlayerEnterPortal();
            }

        }


    }
}