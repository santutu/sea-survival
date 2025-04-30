using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.Weapons;
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
        [Header("스테이지 상태")] [SerializeField] private BattleStageState battleStage;
        [SerializeField] private RestState restStage;

        [Header("포탈")] [SerializeField] private GameObject portalPrefab;

        [SerializeField] private Text scoreText1;
        [SerializeField] private Text scoreText2;
        [SerializeField] private Text currentScoreText;

        public int CurrentStageLv => currentStageLv;
        public int MaxStages => maxStages;

        private StageState _currentState;
        private GameObject _currentPortal;

        protected override void Awake()
        {
            base.Awake();

            // UI 버튼 이벤트 설정

            nextStageButton.GetComponentInChildren<Button>().onClick.AddListener(StartNextStage);


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
            
            currentScoreText.text  = $"Current  Score: {Player.Ins.killedEnemiesCount}";
            Time.timeScale = 0;
        }


        // 다음 스테이지로 진행 버튼 클릭 시
        public void StartNextStage()
        {
            GameManager.Ins.ClearAllEnemiesAndExp();

            Player.Ins.transform.position = Portal.Ins.startPoint.transform.position;
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

        // 보스 처치 시 호출되는 메서드
        public void BossDefeated()
        {
            Debug.Log("보스 처치! 게임 승리!");
            GameVictory();
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

        // 게임 오버 처리
        public void GameOver()
        {
            gameOverPanel.SetActive(true);
            Player.Ins.enabled = false;
            WeaponManager.Ins.gameObject.SetActive(false);
            PlayerLevelSystem.Ins.enabled = false;

            scoreText1.text = $"Score: {Player.Ins.killedEnemiesCount}";
            scoreText2.text = $"Score: {Player.Ins.killedEnemiesCount}";

            // 현재 스테이지 종료
            if (_currentState != null)
            {
                _currentState.OnExit();
                _currentState = null;
            }
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

            scoreText1.text = $"Score: {Player.Ins.killedEnemiesCount}";
            scoreText2.text = $"Score: {Player.Ins.killedEnemiesCount}";

            GameManager.Ins.ClearAllEnemies();
            EnemyAllSpawners.Ins.gameObject.SetActive(false);
        }

        // 메인 메뉴로 돌아가기
        private void ReturnToMainMenu()
        {
            Time.timeScale = 1f; // 시간 스케일 복원
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}