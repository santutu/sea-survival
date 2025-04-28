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

        protected override void Awake()
        {
            base.Awake();

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

        // 경험치 획득
        public void AddExperience(int amount)
        {
            // 최대 레벨에 도달한 경우
            if (currentLevel >= maxLevel)
            {
                return;
            }

            currentExp += amount;
            int requiredExp = GetExpRequiredForNextLevel();

            // 레벨업 체크
            while (currentExp >= requiredExp && currentLevel < maxLevel)
            {
                currentExp -= requiredExp;
                currentLevel++;

                // 레벨업 이벤트 발생
                onLevelUp.Invoke(currentLevel);

                // 최대 레벨 도달 시 경험치를 0으로 설정
                if (currentLevel >= maxLevel)
                {
                    currentExp = 0;
                    break;
                }

                // 다음 레벨에 필요한 경험치 계산
                requiredExp = GetExpRequiredForNextLevel();
            }

            // 경험치 획득 이벤트 발생
            onExpGained.Invoke(currentExp, requiredExp);
        }

        // 다음 레벨에 필요한 경험치 반환
        public int GetExpRequiredForNextLevel()
        {
            if (currentLevel >= maxLevel)
            {
                return int.MaxValue; // 최대 레벨에 도달한 경우 무한대
            }

            return expRequiredForLevel[currentLevel - 1];
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

        // 다음 레벨까지 남은 경험치 반환
        public int GetRemainingExp()
        {
            return GetExpRequiredForNextLevel() - currentExp;
        }
        
        // 현재 레벨 진행률 (0.0 ~ 1.0)
        public float GetLevelProgress()
        {
            int expRequired = GetExpRequiredForNextLevel();
            if (expRequired <= 0) return 1f;
            
            return (float)currentExp / expRequired;
        }

        // 플레이어 레벨업 시 호출되는 메서드
        private void OnPlayerLevelUp(int newLevel)
        {
            Debug.Log($"플레이어 레벨업! 레벨 {newLevel}로 상승");

            // 여기서 카드 선택 UI를 표시하거나 다른 레벨업 보상 로직을 실행
            // CardManager가 레벨업 이벤트를 구독하고 있으므로 자동으로 카드 선택 UI가 표시됨

            // 추가 레벨업 보상이 있다면 여기에 구현
            // 예: 레벨업 효과음 재생, 레벨업 이펙트 표시 등

            // 플레이어 일부 회복
            if (Player != null)
            {
                Player.hp += Player.maxHp * 0.2f; // 최대 체력의 20% 회복
                Player.hp = Mathf.Min(Player.hp, Player.maxHp); // 최대 체력 초과하지 않도록
            }
        }
    }
}