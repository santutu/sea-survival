using System.Collections;
using System.Collections.Generic;
using sea_survival.Scripts.Players;
using sea_survival.Scripts.Stages;
using UnityEngine;

namespace sea_survival.Scripts.CardSystem
{
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance { get; private set; }
        
        [Header("카드 선택 UI")]
        [SerializeField] private GameObject cardSelectionPanel;
        [SerializeField] private List<GameObject> cardSlots = new List<GameObject>();
        [SerializeField] private GameObject weaponCardPrefab;
        [SerializeField] private GameObject statCardPrefab;
        
        private Player Player => Player.Ins;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            // 초기화 시 카드 선택 UI 비활성화
            if (cardSelectionPanel != null)
            {
                cardSelectionPanel.SetActive(false);
            }
        }
        
        // 레벨업 시 카드 선택 UI 표시
        public void ShowLevelUpCardSelection()
        {
            if (cardSelectionPanel != null)
            {
                // 게임 일시 정지
                Time.timeScale = 0f;
                
                cardSelectionPanel.SetActive(true);
                
                // 카드 슬롯 초기화
                foreach (var slot in cardSlots)
                {
                    // 기존 카드 제거
                    foreach (Transform child in slot.transform)
                    {
                        Destroy(child.gameObject);
                    }
                }
                
                // 3개의 랜덤 카드 생성
                GenerateRandomCards();
            }
        }
        
        // 카드 선택 UI 닫기
        public void CloseCardSelection()
        {
            if (cardSelectionPanel != null)
            {
                cardSelectionPanel.SetActive(false);
                
                // 게임 재개
                Time.timeScale = 1f;
            }
        }
        
        // 랜덤 카드 생성
        private void GenerateRandomCards()
        {
            if (cardSlots.Count < 3) return;
            
            // 무기 카드와 능력치 카드 비율 설정 (2:1)
            List<CardType> cardTypes = new List<CardType>
            {
                CardType.Weapon,
                CardType.Weapon,
                CardType.Stat
            };
            
            // 순서 섞기
            ShuffleList(cardTypes);
            
            // 3개의 카드 생성
            for (int i = 0; i < Mathf.Min(3, cardSlots.Count); i++)
            {
                GameObject cardPrefab = null;
                
                // 카드 타입에 따라 프리팹 선택
                if (cardTypes[i] == CardType.Weapon)
                {
                    cardPrefab = weaponCardPrefab;
                }
                else
                {
                    cardPrefab = statCardPrefab;
                }
                
                // 카드 생성
                if (cardPrefab != null)
                {
                    GameObject card = Instantiate(cardPrefab, cardSlots[i].transform);
                    
                    // 카드 초기화
                    Card cardComponent = card.GetComponent<Card>();
                    if (cardComponent != null)
                    {
                        if (cardTypes[i] == CardType.Weapon)
                        {
                            // 무기 카드 초기화
                            cardComponent.InitializeWeaponCard(GetRandomWeaponType());
                        }
                        else
                        {
                            // 스탯 카드 초기화
                            cardComponent.InitializeStatCard(GetRandomStatType());
                        }
                    }
                }
            }
        }
        
        // 리스트 순서 섞기 헬퍼 함수
        private void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        
        // 랜덤 무기 타입 가져오기
        private WeaponType GetRandomWeaponType()
        {
            // 모든 무기 타입 목록
            WeaponType[] allWeaponTypes = new WeaponType[]
            {
                WeaponType.BasicWeapon,
                WeaponType.MagicMissile,
                WeaponType.Dagger,
                WeaponType.Boomerang,
                WeaponType.ElectricOrb,
                WeaponType.SonicWave
            };
            
            // 랜덤 무기 타입 반환
            return allWeaponTypes[Random.Range(0, allWeaponTypes.Length)];
        }
        
        // 랜덤 스탯 타입 가져오기
        private StatType GetRandomStatType()
        {
            // 모든 스탯 타입 목록
            StatType[] allStatTypes = new StatType[]
            {
                StatType.MoveSpeed,
                StatType.MaxHP,
                StatType.HPRegen
            };
            
            // 랜덤 스탯 타입 반환
            return allStatTypes[Random.Range(0, allStatTypes.Length)];
        }
        
        // 카드 선택 함수 (UI에서 호출)
        public void OnCardSelected(Card selectedCard)
        {
            if (selectedCard == null) return;
            
            // 카드 효과 적용
            ApplyCardEffect(selectedCard);
            
            // 카드 선택 UI 닫기
            CloseCardSelection();
            
            Debug.Log("레벨업 카드 선택 완료");
        }
        
        // 카드 효과 적용
        private void ApplyCardEffect(Card card)
        {
            if (card == null) return;
            
            if (card.CardType == CardType.Weapon)
            {
                // 무기 카드 효과 적용
                Debug.Log($"무기 카드 적용: {card.WeaponType}");
                // TODO: 여기에 WeaponManager 등을 통해 무기 업그레이드 또는 획득 처리
            }
            else if (card.CardType == CardType.Stat)
            {
                // 스탯 카드 효과 적용
                Debug.Log($"스탯 카드 적용: {card.StatType}");
                
                if (Player != null)
                {
                    switch (card.StatType)
                    {
                        case StatType.MoveSpeed:
                            Player.moveSpeed += 0.5f; // 이동 속도 증가
                            break;
                        case StatType.MaxHP:
                            Player.maxHp += 20f; // 최대 체력 증가
                            Player.hp += 20f; // 현재 체력도 같이 증가
                            break;
                        case StatType.HPRegen:
                            Player.hpRegen += 0.2f; // HP 리젠 증가
                            break;
                    }
                }
            }
        }
    }
} 