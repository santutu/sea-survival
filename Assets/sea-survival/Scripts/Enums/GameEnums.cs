using UnityEngine;

namespace sea_survival.Scripts.Enums
{
    // 카드 타입 열거형
    public enum CardType
    {
        Weapon, // 무기 카드
        Stat // 능력치 카드
    }

    // 무기 타입 열거형


    // 스탯 타입 열거형
    public enum StatType
    {
        MoveSpeed, // 이동 속도
        MaxHP, // 최대 HP
        HPRegen // HP 회복
    }

    // 무기 레벨 열거형
    public enum WeaponLevel
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3
    }
}