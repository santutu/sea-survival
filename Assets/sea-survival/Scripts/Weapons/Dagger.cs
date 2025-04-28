using System.Collections;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Enums;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public class Dagger : WeaponBase
    {
        [Header("단검 설정")]
        [SerializeField] private float baseDamage = 8f;
        [SerializeField] private float daggerSpeed = 8f;
        [SerializeField] private float daggerLifetime = 3.0f; // 단검이 자동으로 파괴되는 시간(초)
        [SerializeField] private GameObject daggerPrefab;
        [SerializeField] private float fireRate = 1.5f; // 발사 간격
        [SerializeField] private float criticalChance = 0f; // 크리티컬 확률 (레벨 3에서 30%)
        [SerializeField] private float criticalDamageMultiplier = 2f; // 크리티컬 데미지 배율
        
        private void Awake()
        {
            weaponType = WeaponType.Dagger;
            baseAttackInterval = fireRate;
        }
        
        protected override void PerformAttack()
        {
            Vector2 direction = GetAttackDirection();
            FireDagger(direction);
        }
        
        private void FireDagger(Vector2 direction)
        {
            if (daggerPrefab == null) return;
            
            // 단검 생성
            GameObject dagger = Instantiate(daggerPrefab, transform.position, Quaternion.identity);
            DaggerController controller = dagger.AddComponent<DaggerController>();
            
            // 데미지 계산 (레벨에 따른 보정 적용)
            float damage = CalculateDamage(baseDamage);
            
            // 크리티컬 확률 계산
            bool isCritical = Random.value < criticalChance;
            if (isCritical)
            {
                damage *= criticalDamageMultiplier;
            }
            
            // 단검 초기화
            controller.Initialize(damage, direction, daggerSpeed, daggerLifetime, isCritical);
            
            // 일정 시간 후 단검 제거 (아무것도 맞추지 않아도 daggerLifetime 후 파괴됨)
            Destroy(dagger, daggerLifetime);
        }
        
        // 공격 방향 결정
        private Vector2 GetAttackDirection()
        {
            if (Player == null) return Vector2.right;
            
            SpriteRenderer playerSprite = Player.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                return playerSprite.flipX ? Vector2.left : Vector2.right;
            }
            
            return Vector2.right; // 기본값
        }
        
        // 공격 간격 계산 (레벨에 따라 달라짐)
        protected override float GetCurrentAttackInterval()
        {
            // 레벨에 따른 공격 간격 감소
            float intervalMultiplier = 1f;
            
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    intervalMultiplier = 1.0f;
                    break;
                case WeaponLevel.Level2:
                    intervalMultiplier = 0.7f; // 레벨 2는 공격 간격 30% 감소
                    break;
                case WeaponLevel.Level3:
                    intervalMultiplier = 0.5f; // 레벨 3는 공격 간격 50% 감소
                    break;
            }
            
            return baseAttackInterval * intervalMultiplier;
        }
        
        // 레벨업 시 추가 효과 적용
        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            
            if (result)
            {
                if (currentLevel == WeaponLevel.Level2)
                {
                    // 레벨 2에서 관통력 추가
                    // 컨트롤러 클래스에서 처리
                }
                else if (currentLevel == WeaponLevel.Level3)
                {
                    // 레벨 3에서 크리티컬 확률 추가
                    criticalChance = 0.3f; // 30% 크리티컬 확률
                }
            }
            
            return result;
        }
        
        // 현재 레벨 설명
        public override string GetCurrentLevelDescription()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return "빠른 발사 속도, 낮은 공격력";
                case WeaponLevel.Level2:
                    return "발사 속도 30% 증가, 관통력 추가";
                case WeaponLevel.Level3:
                    return "발사 속도 50% 증가, 크리티컬 확률 30% 추가";
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
                    return "발사 속도 30% 증가, 관통력 추가";
                case WeaponLevel.Level2:
                    return "발사 속도 50% 증가, 크리티컬 확률 30% 추가";
                case WeaponLevel.Level3:
                    return "최대 레벨입니다";
                default:
                    return "";
            }
        }
        
        // 단검 컨트롤러 클래스
        private class DaggerController : MonoBehaviour
        {
            private float damage;
            private Vector2 direction;
            private float speed;
            private bool isCritical;
            private bool hasPierce; // 관통 효과 (레벨 2 이상)
            
            public void Initialize(float damage, Vector2 direction, float speed, float lifetime, bool isCritical)
            {
                this.damage = damage;
                this.direction = direction;
                this.speed = speed;
                this.isCritical = isCritical;
                
                // 무기 레벨에 따라 관통 효과 설정
                Dagger dagger = FindObjectOfType<Dagger>();
                if (dagger != null)
                {
                    hasPierce = dagger.CurrentLevel >= WeaponLevel.Level2;
                }
                
                // 단검 회전 설정
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                
                // 왼쪽 방향으로 발사될 때 위아래 뒤집기
                if (direction.x < 0)
                {
                    // Y축을 기준으로 180도 회전하면 위아래가 뒤집힘
                    transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
                }
                
                // 크리티컬 효과 적용 (예: 색상 변경)
                if (isCritical)
                {
                    SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.color = Color.red;
                    }
                }
            }
            
            private void Update()
            {
                // 단검 이동
                transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
            }
            
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision.CompareTag("Enemy"))
                {
                    // 적중한 적에게 데미지 적용
                    IDamageable damageableTarget = collision.GetComponent<IDamageable>();
                    if (damageableTarget != null)
                    {
                        damageableTarget.TakeDamage(damage);
                    }
                    
                    // 관통 효과가 없으면 단검 제거
                    if (!hasPierce)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
} 