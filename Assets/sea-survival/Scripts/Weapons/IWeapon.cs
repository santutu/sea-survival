using sea_survival.Scripts.Enums;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    // 모든 무기가 구현해야 할 기본 인터페이스
    public interface IWeapon
    {
        // 무기 타입
        WeaponType WeaponType { get; }
        
        // 현재 무기 레벨
        WeaponLevel CurrentLevel { get; }
        
        // 무기 레벨업
        bool LevelUp();
        
        // 무기 활성화/비활성화
        void SetActive(bool active);
        
        // 무기의 현재 레벨에 대한 설명을 반환
        string GetCurrentLevelDescription();
        
        // 무기의 다음 레벨에 대한 설명을 반환
        string GetNextLevelDescription();
        
        // 무기의 업그레이드 여부 (최대 레벨에 도달했는지)
        bool CanLevelUp();
    }
} 