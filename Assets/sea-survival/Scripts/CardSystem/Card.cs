using sea_survival.Scripts.Stages;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace sea_survival.Scripts.CardSystem
{
    public class Card : MonoBehaviour
    {
        [Header("카드 정보")]
        [SerializeField] private Image cardImage;
        [SerializeField] private TextMeshProUGUI cardTitle;
        [SerializeField] private TextMeshProUGUI cardDescription;
        
        [Header("무기 카드 이미지")]
        [SerializeField] private Sprite basicWeaponSprite;
        [SerializeField] private Sprite magicMissileSprite;
        [SerializeField] private Sprite daggerSprite;
        [SerializeField] private Sprite boomerangSprite;
        [SerializeField] private Sprite electricOrbSprite;
        [SerializeField] private Sprite sonicWaveSprite;
        
        [Header("스탯 카드 이미지")]
        [SerializeField] private Sprite moveSpeedSprite;
        [SerializeField] private Sprite maxHpSprite;
        [SerializeField] private Sprite hpRegenSprite;
        
        // 카드 타입 저장
        private CardType _cardType;
        private WeaponType _weaponType;
        private StatType _statType;
        
        public CardType CardType => _cardType;
        public WeaponType WeaponType => _weaponType;
        public StatType StatType => _statType;
        
        // 무기 카드 초기화
        public void InitializeWeaponCard(WeaponType weaponType)
        {
            _cardType = CardType.Weapon;
            _weaponType = weaponType;
            
            // 무기 타입에 따라 이미지, 제목, 설명 설정
            switch (weaponType)
            {
                case WeaponType.BasicWeapon:
                    SetCardInfo(basicWeaponSprite, "기본 무기", "바라보는 방향으로 범위 공격\n- 업그레이드 시 공격력 및 범위 증가");
                    break;
                case WeaponType.MagicMissile:
                    SetCardInfo(magicMissileSprite, "마법 미사일", "가장 가까운 적에게 발사\n- 업그레이드 시 미사일 개수 증가");
                    break;
                case WeaponType.Dagger:
                    SetCardInfo(daggerSprite, "단검", "빠른 속도로 공격\n- 업그레이드 시 발사 속도 및 관통력 증가");
                    break;
                case WeaponType.Boomerang:
                    SetCardInfo(boomerangSprite, "보메랑", "양옆으로 발사되어 돌아옴\n- 업그레이드 시 사거리 및 방향 증가");
                    break;
                case WeaponType.ElectricOrb:
                    SetCardInfo(electricOrbSprite, "전기 오브", "주변을 회전하는 구체\n- 업그레이드 시 오브 개수 및 효과 증가");
                    break;
                case WeaponType.SonicWave:
                    SetCardInfo(sonicWaveSprite, "음파 폭발", "주변으로 원형 파동 방출\n- 업그레이드 시 발동 주기 및 효과 증가");
                    break;
            }
        }
        
        // 스탯 카드 초기화
        public void InitializeStatCard(StatType statType)
        {
            _cardType = CardType.Stat;
            _statType = statType;
            
            // 스탯 타입에 따라 이미지, 제목, 설명 설정
            switch (statType)
            {
                case StatType.MoveSpeed:
                    SetCardInfo(moveSpeedSprite, "이동 속도 증가", "이동 속도가 0.5 증가합니다.");
                    break;
                case StatType.MaxHP:
                    SetCardInfo(maxHpSprite, "최대 체력 증가", "최대 체력이 20 증가합니다.");
                    break;
                case StatType.HPRegen:
                    SetCardInfo(hpRegenSprite, "체력 회복력 증가", "초당 체력 회복량이 0.2 증가합니다.");
                    break;
            }
        }
        
        // 카드 정보 설정 헬퍼 함수
        private void SetCardInfo(Sprite sprite, string title, string description)
        {
            if (cardImage != null) cardImage.sprite = sprite;
            if (cardTitle != null) cardTitle.text = title;
            if (cardDescription != null) cardDescription.text = description;
        }
        
        // 카드 선택 시 호출될 함수
        public void OnCardClicked()
        {
            // CardManager에 카드 선택 알림
            if (CardManager.Instance != null)
            {
                CardManager.Instance.OnCardSelected(this);
            }
        }
    }
} 