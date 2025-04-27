using System.Collections;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.StageSystem.Stages;
using UnityEngine;
using UnityEngine.UI;

namespace sea_survival.Scripts.StageSystem
{
    public class BattleStage : StageState
    {
        [Header("스테이지 설정")] [SerializeField] private float stageTime = 60f; // 전투 단계 시간

        [Header("포탈 설정")] [SerializeField] private float portalSpawnTime = 50f; // 포탈이 소환되는 시간 (스테이지 종료 전)

        [Header("UI 설정")] [SerializeField] private Text timerText; // 남은 시간 표시할 텍스트
        [SerializeField] private string timerFormat = "{0:00}:{1:00}"; // 타이머 형식 (분:초)
        [SerializeField] private Text stageText; // 스테이지 레벨 표시할 텍스트
        [SerializeField] private string stageFormat = "스테이지 {0}"; // 스테이지 표시 형식

        [SerializeField] public GameObject bossPrefab;

        private Coroutine _stageTimerCoroutine;
        private Player Player => Player.Ins;

        // 스테이지 레벨 인스턴스
        private IStageLevel[] _stageLevels;

        private void Awake()
        {
            // 스테이지 레벨 인스턴스 초기화
            InitializeStageLevels();
        }

        // 스테이지 레벨 인스턴스 초기화
        private void InitializeStageLevels()
        {
            _stageLevels = new IStageLevel[5]
                           {
                               new Lv1(), // 스테이지 1
                               new Lv2(), // 스테이지 2
                               new Lv3(), // 스테이지 3
                               new Lv4(), // 스테이지 4
                               new Lv5() // 스테이지 5
                           };
        }

        // 상태 진입시 호출
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log($"전투 단계 시작: 스테이지 {StageManager.CurrentStageLv}");

            // 포탈 제거 (이전 스테이지에서 남아있을 경우)
            StageManager.DestroyPortal();

            // 적 스폰 설정
            SetupEnemiesForCurrentStage();

            // 타이머 초기화 및 UI 업데이트
            UpdateTimerUI(stageTime);

            // 스테이지 레벨 UI 업데이트
            UpdateStageUI();

            // 타이머 코루틴 시작
            if (_stageTimerCoroutine != null)
            {
                StopCoroutine(_stageTimerCoroutine);
            }

            _stageTimerCoroutine = StartCoroutine(StageDurationTimer());
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

        // 현재 스테이지에 맞는 적 설정
        private void SetupEnemiesForCurrentStage()
        {
            // 현재 스테이지 레벨 가져오기 (1부터 시작)
            int stageLevel = StageManager.CurrentStageLv;

            // 유효한 스테이지 레벨인지 확인
            if (stageLevel >= 1 && stageLevel <= _stageLevels.Length)
            {
                // 스테이지 레벨에 맞는 적 설정 호출 (인덱스는 0부터 시작)
                _stageLevels[stageLevel - 1].SetupEnemies(this);
            }
            else
            {
                // 유효하지 않은 스테이지 레벨이면 기본(레벨 1) 설정 사용
                _stageLevels[0].SetupEnemies(this);
            }
        }


        // 특정 적 타입 활성화 (외부에서 접근 가능하도록 public으로 변경)


        // 타이머 UI 업데이트 함수
        private void UpdateTimerUI(float timeRemaining)
        {
            if (timerText != null)
            {
                // 시간을 분:초 형식으로 변환
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);

                // UI 텍스트 업데이트
                timerText.text = string.Format(timerFormat, minutes, seconds);
            }
        }

        // 스테이지 레벨 UI 업데이트 함수
        private void UpdateStageUI()
        {
            stageText.text = string.Format(stageFormat, StageManager.CurrentStageLv);
        }

        // 스테이지 타이머 코루틴
        private IEnumerator StageDurationTimer()
        {
            float remainingTime = stageTime;
            bool portalSpawned = false;

            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;

                // UI에 남은 시간 표시
                UpdateTimerUI(remainingTime);

                // 포탈 소환 시간이 되었고 아직 소환되지 않았다면 포탈 소환
                if (remainingTime <= portalSpawnTime && !portalSpawned && Player != null)
                {
                    SpawnPortalNearPlayer();
                    portalSpawned = true;
                }

                yield return null;
            }

            // 타이머가 0에 도달하면 0:00으로 표시
            UpdateTimerUI(0);

            // 스테이지 시간이 끝나면 자동으로 다음 단계로 전환
            Debug.Log("전투 단계 시간 종료");

            // 플레이어가 포탈에 들어가지 않았다면 강제로 휴식 단계로 전환
            if (!portalSpawned)
            {
                SpawnPortalNearPlayer();
            }
        }

        // 플레이어 주변에 포탈 소환
        private void SpawnPortalNearPlayer()
        {
            if (Player != null)
            {
                // 플레이어 위치에서 약간 떨어진 곳에 포탈 생성
                Vector3 portalPosition = Player.transform.position + new Vector3(2f, 0f, 0f);
                StageManager.SpawnPortal(portalPosition);
                Debug.Log("포탈 소환됨");
            }
        }

        // 휴식 단계로 전환하는 함수
        private void TransitionToRestStage()
        {
            // 휴식 단계로 전환
            StageManager.StartRestStage();
        }

        // 포탈 진입 감지 (콜라이더 트리거에서 호출)
        public void OnPlayerEnterPortal()
        {
            // 포탈 진입 시 즉시 휴식 단계로 전환
            TransitionToRestStage();
        }
    }
}