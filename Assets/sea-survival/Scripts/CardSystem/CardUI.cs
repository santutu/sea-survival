using System.Collections.Generic;
using UnityEngine;

namespace sea_survival.Scripts.CardSystem
{
    public class CardUI : MonoBehaviour
    {
        [Header("카드 선택 UI")]
        [SerializeField] private GameObject cardSelectionPanel;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private Card cardPrefab;
        [SerializeField] private int numberOfCardsToShow = 3;
        
        private List<Card> _currentCards = new List<Card>();
        
        private void Awake()
        {
            // 초기에는 카드 선택 UI 비활성화
            if (cardSelectionPanel != null) 
                cardSelectionPanel.SetActive(false);
        }
        
        // 카드 선택 UI 표시
        public void ShowCardSelection(List<CardData> cardDatas)
        {
            // 기존 카드 제거
            ClearCards();
            
            // 카드 생성
            for (int i = 0; i < Mathf.Min(numberOfCardsToShow, cardDatas.Count); i++)
            {
                CreateCard(cardDatas[i]);
            }
            
            // 카드 선택 패널 활성화
            cardSelectionPanel.SetActive(true);
            
            // 게임 일시 정지
            Time.timeScale = 0f;
        }
        
        // 카드 생성
        private void CreateCard(CardData cardData)
        {
            if (cardPrefab == null || cardContainer == null) return;
            
            // 카드 프리팹 생성
            Card cardInstance = Instantiate(cardPrefab, cardContainer);
            
            // 카드 데이터 설정
            cardInstance.SetCardData(cardData);
            
            // 현재 카드 목록에 추가
            _currentCards.Add(cardInstance);
        }
        
        // 모든 카드 제거
        private void ClearCards()
        {
            foreach (Card card in _currentCards)
            {
                Destroy(card.gameObject);
            }
            
            _currentCards.Clear();
        }
        
        // 카드 선택 UI 닫기
        public void CloseCardSelection()
        {
            // 카드 선택 패널 비활성화
            cardSelectionPanel.SetActive(false);
            
            // 게임 재개
            Time.timeScale = 1f;
            
            // 기존 카드 제거
            ClearCards();
        }
    }
}