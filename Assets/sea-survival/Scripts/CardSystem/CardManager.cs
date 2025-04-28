using System.Collections;
using System.Collections.Generic;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Enums;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.Weapons;
using UnityEngine;

namespace sea_survival.Scripts.CardSystem
{
    public class CardManager : SingletonMonoBehaviour<CardManager>
    {
        [Header("카드 풀 설정")] [SerializeField] private List<CardData> allCardDatas = new List<CardData>();
        [SerializeField] private CardUI cardUI;

        private List<CardData> _availableCardDatas = new List<CardData>();
        private CardData _selectedCard = null;

        protected override void Awake()
        {
            base.Awake();
            RefreshAvailableCards();
        }

        private void Start()
        {
            // 레벨업 이벤트에 카드 선택 표시 함수 연결
            PlayerLevelSystem.Ins.onLevelUp.AddListener(ShowLevelUpCards);
        }

        // 사용 가능한 카드 목록 새로 고침
        private void RefreshAvailableCards()
        {
            _availableCardDatas.Clear();

            foreach (CardData card in allCardDatas)
            {
                // 무기 카드인 경우 해당 무기가 최대 레벨인지 확인
                if (card.IsWeaponCard())
                {
                    // 최대 레벨이 아닌 경우에만 추가
                    if (!WeaponManager.Ins.IsWeaponMaxLevel(card.weaponType))
                    {
                        _availableCardDatas.Add(card);
                    }
                }
                else
                {
                    // 스탯 카드는 항상 추가
                    _availableCardDatas.Add(card);
                }
            }
        }

        // 레벨업 시 카드 선택 UI 표시
        public void ShowLevelUpCards(int currentLevel)
        {
            if (cardUI == null) return;

            // 사용 가능한 카드 목록 새로 고침
            RefreshAvailableCards();

            // 표시할 카드 선택
            List<CardData> cardsToShow = SelectRandomCards(3);

            // 카드 UI 표시
            cardUI.ShowCardSelection(cardsToShow);
        }

        // 랜덤 카드 선택
        private List<CardData> SelectRandomCards(int count)
        {
            List<CardData> selectedCards = new List<CardData>();
            List<CardData> tempCards = new List<CardData>(_availableCardDatas);

            // 템프 카드 목록이 비어있는 경우 모든 카드 사용
            if (tempCards.Count == 0)
            {
                tempCards = new List<CardData>(allCardDatas);
            }

            // 랜덤 카드 선택
            for (int i = 0; i < count && tempCards.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, tempCards.Count);
                selectedCards.Add(tempCards[randomIndex]);
                tempCards.RemoveAt(randomIndex);
            }

            return selectedCards;
        }

        // 카드 선택 처리
        public void SelectCard(Card card)
        {
            _selectedCard = card.cardData;

            card.ApplyCardEffect();

            // 카드 선택 UI 닫기
            cardUI.CloseCardSelection();

            // 사용 가능한 카드 목록 업데이트
            RefreshAvailableCards();
        }


        // 무기 카드 데이터 생성 (에디터용 헬퍼 메서드)
        public CardData CreateWeaponCardData(WeaponType weaponType)
        {
            CardData card = ScriptableObject.CreateInstance<CardData>();
            card.cardType = CardType.Weapon;
            card.weaponType = weaponType;

            // 무기 이름 설정
            switch (weaponType)
            {
                case WeaponType.BasicWeapon:
                    card.cardName = "기본 무기";
                    card.description = "바라보는 방향으로 범위 공격";
                    break;
                case WeaponType.MagicMissile:
                    card.cardName = "마법 미사일";
                    card.description = "가장 가까운 적에게 발사되는 미사일";
                    break;
                case WeaponType.Dagger:
                    card.cardName = "단검";
                    card.description = "빠른 속도로 발사되는 단검";
                    break;
                case WeaponType.Boomerang:
                    card.cardName = "보메랑";
                    card.description = "양옆으로 발사되어 돌아오는 보메랑";
                    break;
                case WeaponType.ElectricOrb:
                    card.cardName = "전기 오브";
                    card.description = "플레이어 주변을 회전하는 전기 구체";
                    break;
                case WeaponType.SoundWave:
                    card.cardName = "음파 폭발";
                    card.description = "일정 시간마다 주변으로 원형 파동 방출";
                    break;
            }

            return card;
        }

        // 스탯 카드 데이터 생성 (에디터용 헬퍼 메서드)
        public CardData CreateStatCardData(StatType statType, float increaseAmount)
        {
            CardData card = ScriptableObject.CreateInstance<CardData>();
            card.cardType = CardType.Stat;
            card.statType = statType;
            card.statIncreaseAmount = increaseAmount;

            // 스탯 이름 설정
            switch (statType)
            {
                case StatType.MoveSpeed:
                    card.cardName = "이동 속도 증가";
                    break;
                case StatType.MaxHP:
                    card.cardName = "최대 체력 증가";
                    break;
                case StatType.HPRegen:
                    card.cardName = "체력 회복 증가";
                    break;
            }

            return card;
        }
    }
}