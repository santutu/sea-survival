using sea_survival.Scripts.Enums;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        [Header("무기 기본 설정")]
        [SerializeField] protected WeaponType weaponType;
        [SerializeField] protected WeaponLevel currentLevel = WeaponLevel.Level1;
        [SerializeField] protected float baseAttackInterval = 1.5f;
        
        protected Player Player => Player.Ins;
        protected float attackTimer = 0f;
        protected bool isActive = true;
        
        public WeaponType WeaponType => weaponType;
        public WeaponLevel CurrentLevel => currentLevel;
        
        protected virtual void Update()
        {
            if (!isActive) return;
            
            // 공격 타이머 업데이트
            attackTimer += Time.deltaTime;
            
            // 공격 간격마다 공격 수행
            if (attackTimer >= GetCurrentAttackInterval())
            {
                PerformAttack();
                attackTimer = 0f;
            }
        }
        
        // 실제 공격을 수행하는 메서드 (자식 클래스에서 구현)
        protected abstract void PerformAttack();
        
        // 현재 공격 간격 계산 (레벨에 따라 달라질 수 있음)
        protected virtual float GetCurrentAttackInterval()
        {
            return baseAttackInterval;
        }
        
        // 무기 레벨업
        public virtual bool LevelUp()
        {
            if (!CanLevelUp()) return false;
            
            // 레벨업
            if (currentLevel == WeaponLevel.Level1)
                currentLevel = WeaponLevel.Level2;
            else if (currentLevel == WeaponLevel.Level2)
                currentLevel = WeaponLevel.Level3;
                
            return true;
        }
        
        // 무기 활성화/비활성화
        public virtual void SetActive(bool active)
        {
            isActive = active;
            gameObject.SetActive(active);
        }
        
        // 현재 레벨 설명
        public abstract string GetCurrentLevelDescription();
        
        // 다음 레벨 설명
        public abstract string GetNextLevelDescription();
        
        // 레벨업 가능 여부
        public virtual bool CanLevelUp()
        {
            return currentLevel != WeaponLevel.Level3;
        }
        
        // 무기 데미지 계산 (레벨에 따라 데미지 보정)
        protected virtual float CalculateDamage(float baseDamage)
        {
            float multiplier = 1.0f;
            
            // 레벨에 따른 데미지 보정
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    multiplier = 1.0f;
                    break;
                case WeaponLevel.Level2:
                    multiplier = 1.25f; // 레벨 2는 데미지 25% 증가
                    break;
                case WeaponLevel.Level3:
                    multiplier = 1.5f;  // 레벨 3는 데미지 50% 증가
                    break;
            }
            
            return baseDamage * multiplier;
        }
    }
} 