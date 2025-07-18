using sea_survival.Scripts.Attacks;
using sea_survival.Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public class BasicWeapon : WeaponBase
    {
        [Header("기본 무기 설정")] [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float baseWidth = 2f;
        [SerializeField] private float baseHeight = 1f;
        [SerializeField] private GameObject attackEffectPrefab;
        [SerializeField] private bool penetrateEnemies = false; // 레벨 3에서 활성화

        [SerializeField, ReadOnly] private RectangleAttack rectangleAttack;

        [SerializeField, ReadOnly] private Vector3 originalEffectScale = Vector3.one;

        private void Awake()
        {
            weaponType = WeaponType.BasicWeapon;
            
            // 이펙트 프리팹의 원본 스케일 저장
            if (attackEffectPrefab != null)
            {
                originalEffectScale = attackEffectPrefab.transform.localScale;
            }
            
            // 기본 공격 인스턴스 생성 및 초기화
            rectangleAttack = new RectangleAttack(baseDamage, baseWidth, baseHeight, attackEffectPrefab);
            rectangleAttack.SetDefaultEffectScale(originalEffectScale);
            
            // 기준 크기 설정 (이펙트 스케일 계산에 사용)
            rectangleAttack.SetBaseSize(baseWidth, baseHeight);
        }

        protected override void PerformAttack()
        {
            Vector2 playerPosition = transform.position;
            Vector2 attackDirection = GetAttackDirection();

            // 레벨에 따른 데미지 계산
            float damage = CalculateDamage(baseDamage);

            // 레벨에 따른 범위 계산
            float width = baseWidth * GetWidthMultiplier();
            float height = baseHeight * GetHeightMultiplier();

            // 이펙트 스케일 정보를 가진 새 RectangleAttack 생성
            rectangleAttack = new RectangleAttack(damage, width, height, attackEffectPrefab);
            
            // 원본 이펙트 스케일 정보와 기준 크기 설정
            rectangleAttack.SetDefaultEffectScale(originalEffectScale);
            rectangleAttack.SetBaseSize(baseWidth, baseHeight);
            
            // 공격 수행
            rectangleAttack.PerformAttack(playerPosition, attackDirection);
        }

        // 레벨에 따른 너비 배율
        private float GetWidthMultiplier()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return 1.0f;
                case WeaponLevel.Level2:
                    return 1.2f; // 레벨 2는 너비 20% 증가
                case WeaponLevel.Level3:
                    return 1.4f; // 레벨 3는 너비 40% 증가
                default:
                    return 1.0f;
            }
        }

        // 레벨에 따른 높이 배율
        private float GetHeightMultiplier()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return 1.0f;
                case WeaponLevel.Level2:
                    return 1.2f; // 레벨 2는 높이 20% 증가
                case WeaponLevel.Level3:
                    return 1.4f; // 레벨 3는 높이 40% 증가
                default:
                    return 1.0f;
            }
        }

        // 플레이어가 바라보는 방향 가져오기
        private Vector2 GetAttackDirection()
        {
            return Player.Direction;
        }

        // 레벨업 시 추가 효과 적용
        public override bool LevelUp()
        {
            bool result = base.LevelUp();

            if (result && currentLevel == WeaponLevel.Level3)
            {
                // 레벨 3에서 관통 효과 추가
                penetrateEnemies = true;
            }

            return result;
        }

        // 현재 레벨 설명
        public override string GetCurrentLevelDescription()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return "기본 공격력, 좁은 범위";
                case WeaponLevel.Level2:
                    return "공격력 25% 증가, 범위 20% 증가";
                case WeaponLevel.Level3:
                    return "공격력 50% 증가, 범위 40% 증가, 관통 효과 추가";
                default:
                    return "";
            }
        }

        // 다음 레벨 설명
        public override string GetNextLevelDescription()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return "공격력 25% 증가, 범위 20% 증가";
                case WeaponLevel.Level2:
                    return "공격력 50% 증가, 범위 40% 증가, 관통 효과 추가";
                case WeaponLevel.Level3:
                    return "최대 레벨입니다";
                default:
                    return "";
            }
        }

        // 디버그용 기즈모 그리기
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Vector2 direction = GetAttackDirection();
            rectangleAttack.DrawGizmo(transform.position, direction);
        }
    }
}