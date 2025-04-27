using System.Collections;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts.Stages
{
    public class BattleStage : StageState
    {
        [SerializeField] private float stageTime = 60f; // 전투 단계 시간
        [SerializeField] private float enemySpawnMultiplier = 1.2f; // 스테이지별 적 스폰 증가량
        
        private Coroutine _stageTimerCoroutine;
        
        // 상태 진입시 호출
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log($"전투 단계 시작: 스테이지 {StageManager.CurrentStageLv}");
            
            // 적 스폰 증가
            AdjustEnemySpawn();
            
            // 타이머 코루틴 시작
            if (_stageTimerCoroutine != null)
            {
                StopCoroutine(_stageTimerCoroutine);
            }
            _stageTimerCoroutine = StartCoroutine(StageDurationTimer());
            
            // 전투 관련 UI 활성화 등 추가 작업
        }

        // 상태 종료시 호출
        public override void OnExit()
        {
            // 타이머 코루틴 중지
            if (_stageTimerCoroutine != null)
            {
                StopCoroutine(_stageTimerCoroutine);
                _stageTimerCoroutine = null;
            }
            
            Debug.Log("전투 단계 종료");
            base.OnExit();
        }
        
        // 스테이지에 따른 적 스폰 조정
        private void AdjustEnemySpawn()
        {
            // 여기서 EnemySpawner에 접근하여 스폰 설정 조정
            // 예: EnemySpawner.Ins.SetSpawnRate(baseSpawnRate * Mathf.Pow(enemySpawnMultiplier, StageSystem.CurrentStageLv - 1));
            
            Debug.Log($"스테이지 {StageManager.CurrentStageLv}에 맞춰 적 스폰 조정됨");
        }
        
        // 스테이지 타이머 코루틴
        private IEnumerator StageDurationTimer()
        {
            float remainingTime = stageTime;
            
            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                
                // UI에 남은 시간 표시 등의 작업
                
                yield return null;
            }
            
            // 스테이지 시간이 끝나면 휴식 단계로 전환
            Debug.Log("전투 단계 시간 종료");
            
            // 휴식 단계로 전환
            TransitionToRestStage();
        }
        
        // 휴식 단계로 전환하는 함수
        private void TransitionToRestStage()
        {
            // RestStage로 전환하는 로직
            // 예: StageSystem.Ins.ChangeState(RestStageManager.Ins.GetStage());
            
            // 임시 로직: 다음 스테이지로 바로 진행
            StageManager.NextStage();
        }
    }
} 