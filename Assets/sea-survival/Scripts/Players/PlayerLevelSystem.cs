using System;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.CardSystem;
using UnityEngine;
using UnityEngine.Events;

namespace sea_survival.Scripts.Players
{
    public class PlayerLevelSystem : SingletonMonoBehaviour<PlayerLevelSystem>
    {
        [Serializable]
        public class LevelUpEvent : UnityEvent<int>
        {
        }

        [Header("경험치 및 레벨 설정")] [SerializeField]
        private int currentLevel = 1;

        [SerializeField] private int currentExp = 0;
        [SerializeField] private int[] expRequiredForLevel; // 각 레벨업에 필요한 경험치
        [SerializeField] private int maxLevel = 10;

        [Header("이벤트")] public LevelUpEvent onLevelUp = new LevelUpEvent();
        public UnityEvent<int, int> onExpGained = new UnityEvent<int, int>(); // 현재 경험치, 필요 경험치

        private Player Player => Player.Ins;

        private void Awake()
        {
            // 기본 경험치 테이블 설정 (설정되지 않은 경우)
            if (expRequiredForLevel == null || expRequiredForLevel.Length == 0)
            {
                InitializeDefaultExpTable();
            }
        }

        private void Start()
        {
            // 초기 경험치 이벤트 발생
            onExpGained.Invoke(currentExp, GetExpRequiredForNextLevel());
            
            // 레벨업 이벤트에 카드 선택 UI 표시 함수 연결
            onLevelUp.AddListener(OnPlayerLevelUp);
        }

        // 기본 경험치 테이블 초기화
        private void InitializeDefaultExpTable()
        {
            expRequiredForLevel = new int[maxLevel];

            // 레벨별 필요 경험치 계산 (예: 100, 200, 350, 550, 800, ...)
            for (int i = 0; i < maxLevel; i++)
            {
                expRequiredForLevel[i] = 100 + (i * 50) + (i * i * 25);
            }
        }

        // 경험치 추가
        public void AddExperience(int amount)
        {
            // 이미 최대 레벨이면 무시
            if (currentLevel >= maxLevel)
                return;

            // 경험치 추가
            currentExp += amount;

            // 레벨업 체크
            CheckForLevelUp();

            // 경험치 획득 이벤트 발생
            onExpGained.Invoke(currentExp, GetExpRequiredForNextLevel());
        }

        // 레벨업 체크
        private void CheckForLevelUp()
        {
            int expRequired = GetExpRequiredForNextLevel();

            // 필요 경험치를 충족하면 레벨업
            while (currentExp >= expRequired && currentLevel < maxLevel)
            {
                currentExp -= expRequired;
                currentLevel++;

                // 레벨업 이벤트 발생
                onLevelUp.Invoke(currentLevel);

                // 다음 레벨 필요 경험치 계산
                if (currentLevel < maxLevel)
                {
                    expRequired = GetExpRequiredForNextLevel();
                }
                else
                {
                    // 최대 레벨에 도달하면 경험치를 0으로 설정
                    currentExp = 0;
                    break;
                }
            }
        }

        // 플레이어 레벨업 시 호출
        private void OnPlayerLevelUp(int newLevel)
        {
            Debug.Log($"플레이어 레벨업! 새 레벨: {newLevel}");
            
            // 레벨업 효과음, 이펙트 등 추가 가능
            
            // 레벨업 시 카드 선택 UI 표시
            if (CardManager.Instance != null)
            {
                CardManager.Instance.ShowLevelUpCardSelection();
            }
        }

        // 다음 레벨에 필요한 경험치 반환
        public int GetExpRequiredForNextLevel()
        {
            // 인덱스는 0부터 시작하므로 현재 레벨 - 1
            int index = currentLevel - 1;

            // 유효한 인덱스 확인
            if (index >= 0 && index < expRequiredForLevel.Length)
            {
                return expRequiredForLevel[index];
            }

            // 기본값 반환
            return 100;
        }

        // 현재 레벨 반환
        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        // 현재 경험치 반환
        public int GetCurrentExp()
        {
            return currentExp;
        }

        // 현재 레벨 진행률 (0.0 ~ 1.0)
        public float GetLevelProgress()
        {
            int expRequired = GetExpRequiredForNextLevel();
            if (expRequired <= 0) return 1f;

            return (float)currentExp / expRequired;
        }
    }
}