using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;
using UnityEngine.UI;

namespace sea_survival.Scripts.StageSystem
{
    public class StageManager : SingletonMonoBehaviour<StageManager>
    {
        [SerializeField] private int currentStageLv = 1;
        [SerializeField] private int maxStages = 5;
        [SerializeField] private GameObject nextStageButton;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private Button mainMenuButton;

        [Header("스테이지 상태")] [SerializeField] private BattleStage battleStage;
        [SerializeField] private RestStage restStage;

        [Header("포탈")] [SerializeField] private GameObject portalPrefab;

        public int CurrentStageLv => currentStageLv;
        public int MaxStages => maxStages;

        private StageState _currentState;
        private GameObject _currentPortal;

        protected override void Awake()
        {
            base.Awake();

            // UI 버튼 이벤트 설정
            if (nextStageButton != null)
            {
                nextStageButton.GetComponent<Button>().onClick.AddListener(StartNextStage);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            }

            // 게임 시작 시 UI 패널 비활성화
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (nextStageButton != null) nextStageButton.SetActive(false);
        }

        private void Start()
        {
            // 게임 시작 시 첫 번째 전투 스테이지로 시작
            StartBattleStage();
        }

        public void SetStage(int stageLevel)
        {
            currentStageLv = Mathf.Clamp(stageLevel, 1, maxStages);
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

        // 전투 스테이지 시작
        public void StartBattleStage()
        {
            if (battleStage != null)
            {
                ChangeState(battleStage);
            }
        }

        // 휴식 스테이지 시작
        public void StartRestStage()
        {
            ChangeState(restStage);
            nextStageButton.SetActive(true);
        }

        // 다음 스테이지로 진행 버튼 클릭 시
        public void StartNextStage()
        {
            // 다음 스테이지 버튼 비활성화
            if (nextStageButton != null)
            {
                nextStageButton.SetActive(false);
            }

            // 모든 스테이지를 완료했는지 체크
            if (currentStageLv >= maxStages)
            {
                GameVictory();
                return;
            }

            // 다음 스테이지로 진행
            currentStageLv++;
            StartBattleStage();
        }

        // 포탈 생성
        public void SpawnPortal(Vector3 position)
        {
            if (portalPrefab != null && _currentPortal == null)
            {
                _currentPortal = Instantiate(portalPrefab, position, Quaternion.identity);
            }
        }

        // 포탈 제거
        public void DestroyPortal()
        {
            if (_currentPortal != null)
            {
                Destroy(_currentPortal);
                _currentPortal = null;
            }
        }

        // 게임 오버 처리
        public void GameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // 현재 스테이지 종료
            if (_currentState != null)
            {
                _currentState.OnExit();
                _currentState = null;
            }

            Time.timeScale = 0f; // 게임 일시 정지
        }

        // 게임 승리 처리
        public void GameVictory()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }

            // 현재 스테이지 종료
            if (_currentState != null)
            {
                _currentState.OnExit();
                _currentState = null;
            }

            Time.timeScale = 0f; // 게임 일시 정지
        }

        // 메인 메뉴로 돌아가기
        private void ReturnToMainMenu()
        {
            Time.timeScale = 1f; // 시간 스케일 복원
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}