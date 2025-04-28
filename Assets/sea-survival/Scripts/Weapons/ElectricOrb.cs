using System.Collections.Generic;
using System.Linq;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public class ElectricOrb : WeaponBase
    {
        [Header("전기 오브 설정")] [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float orbRadius = 2f; // 오브가 회전하는 반경
        [SerializeField] private float rotationSpeed = 90f; // 초당 회전 각도
        [SerializeField] private GameObject orbPrefab;
        [SerializeField] private GameObject chainEffectPrefab; // 전기 사슬 이펙트

        [Header("레벨별 설정")] [SerializeField] private int baseOrbCount = 2; // 기본 오브 개수
        [SerializeField] private bool hasChainEffect = false; // 전기 사슬 효과 (레벨 3에서 활성화)

        private List<GameObject> activeOrbs = new List<GameObject>();
        private float currentAngle = 0f;

        private void Awake()
        {
            weaponType = WeaponType.ElectricOrb;
        }

        private void Start()
        {
            // 초기 오브 생성
            CreateOrbs();
        }

        // 오브 생성
        private void CreateOrbs()
        {
            // 기존 오브 제거
            foreach (var orb in activeOrbs)
            {
                if (orb != null)
                {
                    Destroy(orb);
                }
            }

            activeOrbs.Clear();

            // 현재 레벨에 따른 오브 개수 계산
            int orbCount = GetOrbCount();

            // 오브 생성 및 배치
            float angleStep = 360f / orbCount;
            for (int i = 0; i < orbCount; i++)
            {
                float angle = i * angleStep;
                Vector3 position = CalculateOrbPosition(angle);

                GameObject orb = Instantiate(orbPrefab, position, Quaternion.identity, transform);
                OrbController controller = orb.AddComponent<OrbController>();

                // 데미지 계산 및 초기화
                float damage = CalculateDamage(baseDamage);
                controller.Initialize(damage, this);

                activeOrbs.Add(orb);
            }
        }

        // 레벨에 따른 오브 개수 계산
        private int GetOrbCount()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return baseOrbCount; // 기본 2개
                case WeaponLevel.Level2:
                    return baseOrbCount + 1; // 3개
                case WeaponLevel.Level3:
                    return baseOrbCount + 2; // 4개
                default:
                    return baseOrbCount;
            }
        }

        // 레벨에 따른 회전 속도 계산
        private float GetRotationSpeed()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return rotationSpeed;
                case WeaponLevel.Level2:
                    return rotationSpeed * 1.3f; // 레벨 2는 회전 속도 30% 증가
                case WeaponLevel.Level3:
                    return rotationSpeed * 1.5f; // 레벨 3는 회전 속도 50% 증가
                default:
                    return rotationSpeed;
            }
        }

        // 오브 위치 계산
        private Vector3 CalculateOrbPosition(float angle)
        {
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * orbRadius;
            float y = Mathf.Sin(radian) * orbRadius;

            // 플레이어 위치를 기준으로 계산
            return transform.position + new Vector3(x, y, 0);
        }

        // 공격 수행 (오브는 지속적으로 회전하며 공격하므로 별도로 구현)
        protected override void PerformAttack()
        {
            // 필요한 경우 이곳에 추가 공격 로직 구현
            // 기본적으로는 Update에서 지속적으로 회전 및 데미지 처리
        }

        protected override void Update()
        {
            base.Update();

            if (activeOrbs.Count <= 0 || !isActive) return;

            // 플레이어 중심으로 오브 회전
            currentAngle += GetRotationSpeed() * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f;

            // 오브 위치 업데이트
            UpdateOrbPositions();

            // 전기 사슬 효과 (레벨 3)
            if (hasChainEffect && activeOrbs.Count >= 2)
            {
                DrawChainEffects();
            }
        }

        // 오브 위치 업데이트
        private void UpdateOrbPositions()
        {
            float angleStep = 360f / activeOrbs.Count;

            for (int i = 0; i < activeOrbs.Count; i++)
            {
                if (activeOrbs[i] == null) continue;

                float angle = currentAngle + (i * angleStep);
                Vector3 newPosition = CalculateOrbPosition(angle);
                activeOrbs[i].transform.position = newPosition;
            }
        }

        // 전기 사슬 효과 그리기
        private void DrawChainEffects()
        {
            // 이 메서드는 오브 간에 전기 이펙트를 그리는 로직을 구현
            // 실제 게임에서는 LineRenderer 또는 파티클 시스템을 사용하여 구현
            // 지금은 단순화를 위해 기본 구현만 제공

            for (int i = 0; i < activeOrbs.Count; i++)
            {
                if (activeOrbs[i] == null) continue;

                int nextIndex = (i + 1) % activeOrbs.Count;
                if (activeOrbs[nextIndex] == null) continue;

                // 두 오브 사이의 중간 지점과 방향 계산
                Vector3 start = activeOrbs[i].transform.position;
                Vector3 end = activeOrbs[nextIndex].transform.position;
                Vector3 center = (start + end) / 2;
                Vector3 direction = (end - start).normalized;
                float distance = Vector3.Distance(start, end);

                // 전기 사슬 이펙트가 있으면 생성 (실제로는 LineRenderer나 파티클 시스템 사용 권장)
                if (chainEffectPrefab != null)
                {
                    GameObject chainEffect = Instantiate(chainEffectPrefab, center, Quaternion.identity);
                    ChainEffectController controller = chainEffect.AddComponent<ChainEffectController>();
                    controller.Initialize(start, end, activeOrbs[i].GetComponent<OrbController>().Damage * 0.5f);

                    // 잠시 후 이펙트 제거 (지속 시간이 짧은 이펙트)
                    Destroy(chainEffect, 0.1f);
                }

                // 전기 사슬 범위 내의 적 탐지 및 데미지 적용
                DetectEnemiesInChain(start, end, distance);
            }
        }

        // 전기 사슬 범위 내 적 감지
        private void DetectEnemiesInChain(Vector3 start, Vector3 end, float distance)
        {
            // 두 오브 사이의 선 범위에서 적 탐지
            RaycastHit2D[] hits = Physics2D.LinecastAll(start, end);

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    IDamageable damageableTarget = hit.collider.GetComponent<IDamageable>();
                    if (damageableTarget != null)
                    {
                        // 전기 사슬 데미지는 기본 데미지의 50%
                        float chainDamage = CalculateDamage(baseDamage) * 0.5f;
                        damageableTarget.TakeDamage(chainDamage);
                    }
                }
            }
        }

        // 레벨업 시 추가 효과 적용
        [Button]
        public override bool LevelUp()
        {
            bool result = base.LevelUp();

            if (result)
            {
                if (currentLevel == WeaponLevel.Level2)
                {
                    // 레벨 2: 오브 개수 증가, 회전 속도 증가
                }
                else if (currentLevel == WeaponLevel.Level3)
                {
                    // 레벨 3: 오브 개수 추가 증가, 전기 사슬 효과 추가
                    hasChainEffect = true;
                }

                // 레벨업 시 오브 재생성
                CreateOrbs();
            }

            return result;
        }

        // 현재 레벨 설명
        public override string GetCurrentLevelDescription()
        {
            switch (currentLevel)
            {
                case WeaponLevel.Level1:
                    return "2개의 오브가 주변을 회전, 접촉한 적에게 데미지";
                case WeaponLevel.Level2:
                    return "오브 개수 3개로 증가, 회전 속도 증가";
                case WeaponLevel.Level3:
                    return "오브 개수 4개, 전기 사슬 효과 (오브끼리 연결되어 추가 데미지)";
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
                    return "오브 개수 3개로 증가, 회전 속도 증가";
                case WeaponLevel.Level2:
                    return "오브 개수 4개, 전기 사슬 효과 (오브끼리 연결되어 추가 데미지)";
                case WeaponLevel.Level3:
                    return "최대 레벨입니다";
                default:
                    return "";
            }
        }

        // 오브 컨트롤러 클래스
        private class OrbController : MonoBehaviour
        {
            private float damage;
            private ElectricOrb parent;

            // 중복 데미지 방지를 위한 히트 타이머
            private Dictionary<GameObject, float> hitCooldowns = new Dictionary<GameObject, float>();
            private const float HIT_COOLDOWN = 0.5f; // 같은 적에 대한 데미지 적용 간격

            public float Damage => damage;

            public void Initialize(float damage, ElectricOrb parent)
            {
                this.damage = damage;
                this.parent = parent;
            }

            private void Update()
            {
                // 히트 쿨다운 업데이트
                List<GameObject> removeList = new List<GameObject>();
                foreach (var enemy in hitCooldowns.Keys.ToArray())
                {
                    hitCooldowns[enemy] -= Time.deltaTime;
                    if (hitCooldowns[enemy] <= 0)
                    {
                        removeList.Add(enemy);
                    }
                }

                foreach (var enemy in removeList.ToArray())
                {
                    hitCooldowns.Remove(enemy);
                }
            }

            private void OnTriggerStay2D(Collider2D collision)
            {
                if (collision.CompareTag("Enemy"))
                {
                    // 히트 쿨다운 확인
                    if (hitCooldowns.ContainsKey(collision.gameObject) && hitCooldowns[collision.gameObject] > 0)
                    {
                        return; // 아직 쿨다운 중
                    }

                    // 데미지 적용
                    IDamageable damageableTarget = collision.GetComponent<IDamageable>();
                    if (damageableTarget != null)
                    {
                        damageableTarget.TakeDamage(damage);

                        // 쿨다운 설정
                        hitCooldowns[collision.gameObject] = HIT_COOLDOWN;
                    }
                }
            }
        }

        // 전기 사슬 이펙트 컨트롤러
        private class ChainEffectController : MonoBehaviour
        {
            private Vector3 startPoint;
            private Vector3 endPoint;
            private float damage;
            private LineRenderer lineRenderer;

            public void Initialize(Vector3 start, Vector3 end, float damage)
            {
                this.startPoint = start;
                this.endPoint = end;
                this.damage = damage;

                // LineRenderer 설정
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPoint);
                lineRenderer.SetPosition(1, endPoint);

                // 전기 효과를 위한 재질 설정 (실제 게임에서는 적절한 전기 재질 사용)
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.cyan;
                lineRenderer.endColor = Color.blue;
            }
        }
    }
}