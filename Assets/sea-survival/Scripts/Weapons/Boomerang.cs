using System.Collections;
using System.Collections.Generic;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Enums;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public class Boomerang : WeaponBase
    {
        [Header("보메랑 설정")]
        [SerializeField] private float baseDamage = 12f;
        [SerializeField] private float boomerangSpeed = 6f;
        [SerializeField] private float maxDistance = 5f; // 최대 이동 거리
        [SerializeField] private float returnSpeed = 8f; // 돌아오는 속도
        [SerializeField] private GameObject boomerangPrefab;
        [SerializeField] private float attackInterval = 3f; // 발사 간격
        
        private int directionCount = 2; // 발사 방향 수 (레벨 3에서 4방향으로 증가)
        private int pierceCount = 1; // 관통 횟수 (레벨 3에서 증가)
        
        private void Awake()
        {
            weaponType = WeaponType.Boomerang;
            baseAttackInterval = attackInterval;
        }
        
        protected override void PerformAttack()
        {
            // 플레이어 위치에서 보메랑 발사
            Vector2 playerPosition = transform.position;
            
            // 방향 수에 따라 보메랑 발사
            float angleStep = 360f / directionCount;
            for (int i = 0; i < directionCount; i++)
            {
                float angle = i * angleStep;
                Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                
                StartCoroutine(LaunchBoomerang(playerPosition, direction, i * 0.2f)); // 약간의 딜레이를 두고 발사
            }
        }
        
        private IEnumerator LaunchBoomerang(Vector2 startPosition, Vector2 direction, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (boomerangPrefab == null) yield break;
            
            // 보메랑 생성
            GameObject boomerang = Instantiate(boomerangPrefab, startPosition, Quaternion.identity);
            BoomerangController controller = boomerang.AddComponent<BoomerangController>();
            
            // 데미지 계산
            float damage = CalculateDamage(baseDamage);
            
            // 보메랑 속성 설정
            float distanceMultiplier = GetDistanceMultiplier();
            controller.Initialize(damage, direction, boomerangSpeed, returnSpeed, maxDistance * distanceMultiplier, pierceCount, Player.transform);
            
            // 일정 시간 후 파괴 (안전장치)
            Destroy(boomerang, 10f);
        }
        
        // 레벨에 따른 사거리 증가 계산
        private float GetDistanceMultiplier()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return 1.0f;
                case WeaponLevel.Level2:
                    return 1.3f; // 레벨 2는 사거리 30% 증가
                case WeaponLevel.Level3:
                    return 1.5f; // 레벨 3는 사거리 50% 증가
                default:
                    return 1.0f;
            }
        }
        
        // 레벨업 시 추가 효과 적용
        public override bool LevelUp()
        {
            bool result = base.LevelUp();
            
            if (result)
            {
                if (currentLevel == WeaponLevel.Level2)
                {
                    // 레벨 2에서는 회전 속도 증가 (별도의 변수 없이 컨트롤러에서 처리)
                }
                else if (currentLevel == WeaponLevel.Level3)
                {
                    // 레벨 3에서는 4방향 발사 및 관통 횟수 증가
                    directionCount = 4;
                    pierceCount = 3;
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
                    return "두 방향으로 발사, 왕복 시 데미지";
                case WeaponLevel.Level2:
                    return "사거리 30% 증가, 회전 속도 증가";
                case WeaponLevel.Level3:
                    return "4방향 발사로 변경, 적 관통 횟수 증가";
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
                    return "사거리 30% 증가, 회전 속도 증가";
                case WeaponLevel.Level2:
                    return "4방향 발사로 변경, 적 관통 횟수 증가";
                case WeaponLevel.Level3:
                    return "최대 레벨입니다";
                default:
                    return "";
            }
        }
        
        // 보메랑 컨트롤러 클래스
        private class BoomerangController : MonoBehaviour
        {
            private float damage;
            private Vector2 direction;
            private float outSpeed;
            private float returnSpeed;
            private float maxDistance;
            private int maxPierceCount;
            private Transform target; // 플레이어 트랜스폼
            
            private Vector2 startPosition;
            private float distanceTraveled = 0f;
            private bool isReturning = false;
            private int pierceCount = 0;
            private float rotationSpeed = 360f; // 초당 회전 각도
            
            private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // 이미 타격한 적 추적
            
            public void Initialize(float damage, Vector2 direction, float outSpeed, float returnSpeed, 
                float maxDistance, int maxPierceCount, Transform target)
            {
                this.damage = damage;
                this.direction = direction.normalized;
                this.outSpeed = outSpeed;
                this.returnSpeed = returnSpeed;
                this.maxDistance = maxDistance;
                this.maxPierceCount = maxPierceCount;
                this.target = target;
                
                startPosition = transform.position;
                
                // 레벨 2 이상이면 회전 속도 증가
                Boomerang boomerang = FindObjectOfType<Boomerang>();
                if (boomerang != null && boomerang.CurrentLevel >= WeaponLevel.Level2)
                {
                    rotationSpeed = 540f; // 회전 속도 1.5배 증가
                }
            }
            
            private void Update()
            {
                // 보메랑 회전 애니메이션
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                
                // 이동 처리
                if (!isReturning)
                {
                    // 전진 방향으로 이동
                    transform.Translate(direction * outSpeed * Time.deltaTime, Space.World);
                    
                    // 이동 거리 계산
                    distanceTraveled = Vector2.Distance(startPosition, transform.position);
                    
                    // 최대 거리에 도달하면 돌아오기 시작
                    if (distanceTraveled >= maxDistance)
                    {
                        isReturning = true;
                    }
                }
                else
                {
                    // 플레이어가 존재하면 플레이어 방향으로 돌아옴
                    if (target != null)
                    {
                        Vector2 returnDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
                        transform.Translate(returnDirection * returnSpeed * Time.deltaTime, Space.World);
                        
                        // 플레이어와 일정 거리 이내면 도착한 것으로 간주하고 제거
                        if (Vector2.Distance(transform.position, target.position) < 0.5f)
                        {
                            Destroy(gameObject);
                        }
                    }
                    else
                    {
                        // 플레이어가 없는 경우 시작 위치로 돌아감
                        Vector2 returnDirection = (startPosition - (Vector2)transform.position).normalized;
                        transform.Translate(returnDirection * returnSpeed * Time.deltaTime, Space.World);
                        
                        // 시작 위치와 일정 거리 이내면 제거
                        if (Vector2.Distance(transform.position, startPosition) < 0.5f)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
            
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision.CompareTag("Enemy") && !hitEnemies.Contains(collision.gameObject))
                {
                    // 적중한 적 추가
                    hitEnemies.Add(collision.gameObject);
                    
                    // 적에게 데미지 적용
                    IDamageable damageableTarget = collision.GetComponent<IDamageable>();
                    if (damageableTarget != null)
                    {
                        damageableTarget.TakeDamage(damage);
                    }
                    
                    // 관통 횟수 증가
                    pierceCount++;
                    
                    // 최대 관통 횟수를 초과하면 돌아오기 시작
                    if (pierceCount >= maxPierceCount && !isReturning)
                    {
                        isReturning = true;
                    }
                }
            }
        }
    }
} 