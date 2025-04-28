using sea_survival.Scripts.Players;
using sea_survival.Scripts.Weapons;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using sea_survival.Scripts.Enums;

namespace sea_survival.Scripts.CardSystem
{
    public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("카드 UI 요소")] [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardImage;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI cardDescriptionText;

        [Header("카드 애니메이션")] [SerializeField] private float hoverScaleMultiplier = 1.1f;
        [SerializeField] private float animationDuration = 0.2f;

        private CardData _cardData;
        private Vector3 _originalScale;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalScale = _rectTransform.localScale;
        }

        // 카드 데이터 설정
        public void SetCardData(CardData cardData)
        {
            _cardData = cardData;
            UpdateCardUI();
        }

        // 카드 UI 업데이트
        private void UpdateCardUI()
        {
            if (_cardData == null) return;

            // 카드 정보 설정
            cardNameText.text = _cardData.cardName;

            // 카드 설명 설정
            string description = _cardData.GetDescription();

            // 무기 카드인 경우 다음 레벨 설명 추가
            if (_cardData.IsWeaponCard())
            {
                // 무기가 이미 있는지 확인
                if (WeaponManager.Ins.HasWeapon(_cardData.weaponType))
                {
                    // 무기가 최대 레벨인지 확인
                    if (WeaponManager.Ins.IsWeaponMaxLevel(_cardData.weaponType))
                    {
                        description += "<color=#FF5555>최대 레벨 도달</color>";
                    }
                    else
                    {
                        // 활성화된 무기 목록을 가져와서 다음 레벨 설명 추가
                        var activeWeapons = WeaponManager.Ins.GetAllActiveWeapons();
                        if (activeWeapons.TryGetValue(_cardData.weaponType, out IWeapon weapon))
                        {
                            description += "<color=#00FFFF>다음 레벨:</color> " + weapon.GetNextLevelDescription();
                        }
                    }
                }
                else
                {
                    // 아직 획득하지 않은 무기인 경우 기본 설명만 표시
                    description += "<color=#00FFFF>새 무기 획득</color>";
                }
            }

            cardDescriptionText.text = description;

            // 카드 이미지 설정
            if (_cardData.cardImage != null)
            {
                cardImage.sprite = _cardData.cardImage;
                cardImage.enabled = true;
            }
            else
            {
                cardImage.enabled = false;
            }
        }

        // 카드 클릭 이벤트
        public void OnPointerClick(PointerEventData eventData)
        {
            // 카드 매니저를 통해 카드 선택 처리
            CardManager.Ins.SelectCard(_cardData);
        }

        // 마우스 진입 시 카드 확대
        public void OnPointerEnter(PointerEventData eventData)
        {
            _rectTransform.localScale = _originalScale * hoverScaleMultiplier;
        }

        // 마우스 이탈 시 카드 원래 크기로
        public void OnPointerExit(PointerEventData eventData)
        {
            _rectTransform.localScale = _originalScale;
        }

        // 카드 효과 적용
        public void ApplyCardEffect()
        {
            if (_cardData == null) return;

            if (_cardData.IsWeaponCard())
            {
                // 무기 카드 효과 적용
                WeaponManager.Ins.AddWeapon(_cardData.weaponType);
            }
            else if (_cardData.IsStatCard())
            {
                // 스탯 카드 효과 적용
                Player player = Player.Ins;

                switch (_cardData.statType)
                {
                    case Enums.StatType.MoveSpeed:
                        player.moveSpeed *= (1 + _cardData.statIncreaseAmount / 100f);
                        break;
                    case Enums.StatType.MaxHP:
                        player.maxHp *= (1 + _cardData.statIncreaseAmount / 100f);
                        player.hp = player.maxHp; // 체력 완전 회복
                        break;
                    case Enums.StatType.HPRegen:
                        player.hpRegen *= (1 + _cardData.statIncreaseAmount / 100f);
                        break;
                }
            }
        }
    }
}