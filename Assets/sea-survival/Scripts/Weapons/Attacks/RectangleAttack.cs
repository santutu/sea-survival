using System;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Enemies;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace sea_survival.Scripts.Attacks
{
    [Serializable]
    public class RectangleAttack : IAttack
    {
        [SerializeField]
        // 공격 속성
        private float damage;

        [SerializeField] private float width;
        [SerializeField] private float height;
        [SerializeField] private GameObject effectPrefab;

        // 기본 크기 저장용 변수
        private Vector3 defaultEffectScale = Vector3.one;
        private bool isDefaultScaleSet = false;

        // 기준 크기 (초기화 시 설정)
        private float baseWidth = 1f;
        private float baseHeight = 1f;
        private bool isBaseSet = false;

        // 이미 타격한 대상을 추적하기 위한 HashSet
        private HashSet<GameObject> hitGameObjects = new HashSet<GameObject>();

        // 생성자
        public RectangleAttack(float damage, float width, float height, GameObject effectPrefab = null)
        {
            this.damage = damage;
            this.width = width;
            this.height = height;
            this.effectPrefab = effectPrefab;

            // 기준 크기 설정 (처음 생성 시만)
            if (!isBaseSet)
            {
                baseWidth = width;
                baseHeight = height;
                isBaseSet = true;
            }

            // 이펙트 프리팹이 있으면 기본 스케일 설정
            if (effectPrefab != null && !isDefaultScaleSet)
            {
                defaultEffectScale = effectPrefab.transform.localScale;
                isDefaultScaleSet = true;
            }
        }

        // 공격 실행
        public void PerformAttack(Vector2 position, Vector2 direction, string targetTag = "Enemy")
        {
            // 사각형 중심 위치 계산 (캐릭터 앞쪽에 사각형 중심이 오도록)
            float distanceToCenter = width / 2;
            Vector2 center = position + direction.normalized * distanceToCenter;

            // 회전된 사각형에 해당하는 영역 검사를 위한 각도 계산
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // 공격 시작 시 히트 GameObjects 초기화
            hitGameObjects.Clear();

            // 공격 이펙트 생성
            if (effectPrefab != null)
            {
                GameObject effect = Object.Instantiate(effectPrefab, center, rotation);

                // 이펙트 기본 스케일 확인 및 설정
                if (!isDefaultScaleSet)
                {
                    defaultEffectScale = effectPrefab.transform.localScale;
                    isDefaultScaleSet = true;
                }

                // 공격 범위에 맞게 이펙트 크기 조정
                if (effect != null)
                {
                    // 현재 너비/높이와 기준 너비/높이의 비율 계산
                    float scaleX = width / baseWidth;
                    float scaleY = height / baseHeight;

                    // 정확한 스케일 적용
                    Vector3 newScale = new Vector3(
                        defaultEffectScale.x * scaleX,
                        defaultEffectScale.y * scaleY,
                        defaultEffectScale.z
                    );

                    effect.transform.localScale = newScale;
                }

                // 1초 후 이펙트 제거
                Object.Destroy(effect, 1f);
            }

            // 사각형 영역 내 대상 검출
            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(center, new Vector2(width, height), angle);

            // 대상에게 데미지 적용
            foreach (Collider2D target in hitTargets)
            {
                if (target.CompareTag(targetTag) && !hitGameObjects.Contains(target.gameObject))
                {
                    hitGameObjects.Add(target.gameObject);
                    IDamageable damageableTarget = target.GetComponent<IDamageable>();
                    if (damageableTarget != null)
                    {
                        damageableTarget.TakeDamage(damage);
                    }
                    else
                    {
                        // 기존 Enemy 클래스 지원
                        Enemy enemyComponent = target.GetComponent<Enemy>();
                        if (enemyComponent != null)
                        {
                            enemyComponent.TakeDamage(damage);
                        }
                    }
                }
            }
        }

        // 디버그용 기즈모 그리기
        public void DrawGizmo(Vector3 position, Vector2 direction)
        {
            Gizmos.color = Color.blue;

            // 사각형 중심 위치 계산
            float distanceToCenter = width / 2;
            Vector3 center = position + new Vector3(direction.normalized.x, direction.normalized.y, 0) * distanceToCenter;

            // 회전된 사각형 그리기
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // 확장 메서드를 사용하여 회전된 와이어 큐브 그리기
            GizmosExtensions.DrawWireCube(center, new Vector3(width, height, 0.1f), rotation);
        }

        // 이펙트 프리팹의 기본 스케일 설정 메서드 (외부에서 직접 호출하여 설정 가능)
        public void SetDefaultEffectScale(Vector3 scale)
        {
            defaultEffectScale = scale;
            isDefaultScaleSet = true;
        }

        // 기준 너비/높이 설정 메서드
        public void SetBaseSize(float baseW, float baseH)
        {
            baseWidth = baseW;
            baseHeight = baseH;
            isBaseSet = true;
        }

        // 현재 공격 속성에 접근하기 위한 프로퍼티
        public float Width { get { return width; } }
        public float Height { get { return height; } }
        public float Damage { get { return damage; } }
    }

    // Gizmos 확장 메서드 클래스
    public static class GizmosExtensions
    {
        // 회전된 와이어 큐브 그리기
        public static void DrawWireCube(Vector3 position, Vector3 size, Quaternion rotation)
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = originalMatrix;
        }
    }
}