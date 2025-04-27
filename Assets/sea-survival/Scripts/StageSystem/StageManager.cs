using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts.Stages
{
    public class StageManager : SingletonMonoBehaviour<StageManager>
    {
        [SerializeField] private int currentStageLv = 1;
        public int CurrentStageLv => currentStageLv;
        
        private StageState _currentState;

        protected override void Awake()
        {
            base.Awake();
            // 초기 상태 설정
        }

        public void SetStage(int stageLevel)
        {
            currentStageLv = stageLevel;
        }

        public void ChangeState(StageState newState)
        {
            // 현재 상태가 있으면 종료
            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            // 새 상태로 변경
            _currentState = newState;
            
            // 새 상태 시작
            if (_currentState != null)
            {
                _currentState.OnEnter();
            }
        }
        
        // 다음 스테이지로 진행
        public void NextStage()
        {
            currentStageLv++;
            // 여기에 다음 스테이지로 전환하는 로직 추가
        }
    }
} 