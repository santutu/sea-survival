using sea_survival.Scripts.Enums;
using UnityEngine;

namespace sea_survival.Scripts.CardSystem
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Sea Survival/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("카드 기본 정보")]
        public string cardName;
        public string description;
        public Sprite cardImage;
        public CardType cardType;
        
        [Header("무기 카드 설정")]
        public WeaponType weaponType;
        
        [Header("스탯 카드 설정")]
        public StatType statType;
        public float statIncreaseAmount = 10f; // 기본 증가량 (% 또는 절대값)
        
        // 카드 설명 생성
        public string GetDescription()
        {
            if (cardType == CardType.Weapon)
            {
                // 무기 카드인 경우 무기 설명 반환
                return description;
            }
            else if (cardType == CardType.Stat)
            {
                // 스탯 카드인 경우 스탯 증가량 포함한 설명 반환
                string statName = GetStatName();
                
                // 스탯 타입에 따라 다른 설명 형식 사용
                switch (statType)
                {
                    case StatType.MoveSpeed:
                        // 이동 속도는 백분율로 표시
                        return $"{statName} {statIncreaseAmount}% 증가";
                    case StatType.MaxHP:
                        // 최대 HP는 절대값으로 표시
                        return $"{statName} {statIncreaseAmount} 증가";
                    case StatType.HPRegen:
                        // HP 재생은 절대값으로 표시
                        return $"{statName} {statIncreaseAmount}/초 증가";
                    default:
                        return $"{statName} {statIncreaseAmount} 증가";
                }
            }
            
            return description;
        }
        
        // 스탯 이름 가져오기
        private string GetStatName()
        {
            switch (statType)
            {
                case StatType.MoveSpeed:
                    return "이동 속도";
                case StatType.MaxHP:
                    return "최대 체력";
                case StatType.HPRegen:
                    return "체력 회복";
                default:
                    return "";
            }
        }
        
        // 무기 카드인지 확인
        public bool IsWeaponCard()
        {
            return cardType == CardType.Weapon;
        }
        
        // 스탯 카드인지 확인
        public bool IsStatCard()
        {
            return cardType == CardType.Stat;
        }
    }
} 