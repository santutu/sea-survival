using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace sea_survival.Scripts.Players
{
    public class HeartSystem : MonoBehaviour
    {
        [Header("하트 시스템 설정")]
        [SerializeField] private int maxHearts = 3;
        [SerializeField] private int currentHearts;
        [SerializeField] private float invincibilityDurationOnHeartLoss = 3f; // 하트 잃을 때 무적 시간
        
        [Header("UI 설정")]
        [SerializeField] private Transform heartContainerTopRight; // 스크린 오른쪽 위
        [SerializeField] private Transform heartContainerBottom; // 캐릭터 밑
        [SerializeField] private GameObject heartPrefab; // 하트 UI 프리팹
        
        [Header("하트 스프라이트")]
        [SerializeField] private Sprite fullHeartSprite;
        [SerializeField] private Sprite emptyHeartSprite;
        
        [SerializeField, ReadOnly] private List<Image> topRightHearts = new List<Image>();
        [SerializeField, ReadOnly] private List<Image> bottomHearts = new List<Image>();
        
        private Player _player;
        
        public int CurrentHearts => currentHearts;
        public int MaxHearts => maxHearts;
        
        private void Awake()
        {
            _player = GetComponent<Player>();
            currentHearts = maxHearts;
        }
        
        private void Start()
        {
            CreateHeartUI();
            UpdateHeartDisplay();
        }
        
        private void CreateHeartUI()
        {
            // 오른쪽 위 하트 UI 생성
            if (heartContainerTopRight != null)
            {
                for (int i = 0; i < maxHearts; i++)
                {
                    GameObject heartObj = Instantiate(heartPrefab, heartContainerTopRight);
                    Image heartImage = heartObj.GetComponent<Image>();
                    if (heartImage != null)
                    {
                        topRightHearts.Add(heartImage);
                    }
                }
            }
            
            // 캐릭터 밑 하트 UI 생성
            if (heartContainerBottom != null)
            {
                for (int i = 0; i < maxHearts; i++)
                {
                    GameObject heartObj = Instantiate(heartPrefab, heartContainerBottom);
                    Image heartImage = heartObj.GetComponent<Image>();
                    if (heartImage != null)
                    {
                        bottomHearts.Add(heartImage);
                    }
                }
            }
        }
        
        public bool TryLoseHeart()
        {
            if (currentHearts <= 0)
            {
                return false; // 게임오버
            }
            
            currentHearts--;
            UpdateHeartDisplay();
            
            if (currentHearts <= 0)
            {
                return false; // 게임오버
            }
            
            // 하트를 잃었을 때 HP 회복 및 무적 시간 부여
            _player.hp = _player.maxHp;
            _player.StartInvincibility(invincibilityDurationOnHeartLoss);
            
            return true; // 살아있음
        }
        
        public void RecoverHeart()
        {
            if (currentHearts < maxHearts)
            {
                currentHearts++;
                UpdateHeartDisplay();
            }
        }
        
        [Button("하트 회복 테스트")]
        public void TestRecoverHeart()
        {
            RecoverHeart();
        }
        
        [Button("하트 잃기 테스트")]
        public void TestLoseHeart()
        {
            TryLoseHeart();
        }
        
        private void UpdateHeartDisplay()
        {
            // 오른쪽 위 하트 업데이트
            for (int i = 0; i < topRightHearts.Count; i++)
            {
                if (topRightHearts[i] != null)
                {
                    topRightHearts[i].sprite = i < currentHearts ? fullHeartSprite : emptyHeartSprite;
                }
            }
            
            // 캐릭터 밑 하트 업데이트
            for (int i = 0; i < bottomHearts.Count; i++)
            {
                if (bottomHearts[i] != null)
                {
                    bottomHearts[i].sprite = i < currentHearts ? fullHeartSprite : emptyHeartSprite;
                }
            }
        }
    }
} 