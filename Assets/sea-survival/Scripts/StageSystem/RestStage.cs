using System.Collections;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts.Stages
{
    public class RestStage : StageState
    {
        [SerializeField] private float restDuration = 30f; // 휴식 시간 (초)
        [SerializeField] private float healAmount = 10f; // 초당 회복량
        
        private Player Player => Player.Ins;
        private Coroutine _healCoroutine;

        // 상태 진입시 호출
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log($"휴식 단계 진입: 스테이지 {StageManager.CurrentStageLv}");
            
            // 체력 회복 코루틴 시작
            if (_healCoroutine != null)
            {
                StopCoroutine(_healCoroutine);
            }
            _healCoroutine = StartCoroutine(HealOverTime());
            
            // UI 활성화 등 추가 작업
        }

        // 상태 종료시 호출
        public override void OnExit()
        {
            // 체력 회복 코루틴 중지
            if (_healCoroutine != null)
            {
                StopCoroutine(_healCoroutine);
                _healCoroutine = null;
            }
            
            Debug.Log("휴식 단계 종료");
            base.OnExit();
        }
        
        // 플레이어 체력 회복 코루틴
        private IEnumerator HealOverTime()
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < restDuration)
            {
                // 플레이어 체력 회복
                if (Player != null && Player.hp < Player.maxHp)
                {
                    Player.hp = Mathf.Min(Player.hp + (healAmount * Time.deltaTime), Player.maxHp);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // 휴식 시간이 끝나면 다음 단계로 전환
            Debug.Log("휴식 시간 종료");
            // 추가 작업이 끝나면 다음 단계로 이동
            StageManager.NextStage();
        }
    }
} 