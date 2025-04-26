using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace sea_survival.Scripts.UI
{
    public class PlayerExpUI : MonoBehaviour
    {
        [Header("UI 요소")] [SerializeField] private Image expFillImage;
        [SerializeField] private Text levelText;
        [SerializeField] private Text expText;

        private PlayerLevelSystem LevelSystem => PlayerLevelSystem.Ins;

        private void Start()
        {
            LevelSystem.onLevelUp.AddListener(OnLevelUp);
            LevelSystem.onExpGained.AddListener(OnExpGained);

            UpdateUI();
        }

        private void OnDestroy()
        {
            LevelSystem.onLevelUp.RemoveListener(OnLevelUp);
            LevelSystem.onExpGained.RemoveListener(OnExpGained);
        }

        private void OnLevelUp(int newLevel)
        {
            UpdateUI();
        }

        private void OnExpGained(int currentExp, int requiredExp)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            levelText.text = $"Lv. {LevelSystem.GetCurrentLevel()}";

            int currentExp = LevelSystem.GetCurrentExp();
            int requiredExp = LevelSystem.GetExpRequiredForNextLevel();
            expText.text = $"{currentExp} / {requiredExp}";

            if (expFillImage != null)
            {
                expFillImage.fillAmount = LevelSystem.GetLevelProgress();
            }
        }
    }
}