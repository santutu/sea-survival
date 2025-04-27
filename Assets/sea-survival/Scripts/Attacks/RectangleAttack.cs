using System;
using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Enemies;
using UnityEngine;
using Object = UnityEngine.Object;

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

        // 생성자
        public RectangleAttack(float damage, float width, float height, GameObject effectPrefab = null)
        {
            this.damage = damage;
            this.width = width;
            this.height = height;
            this.effectPrefab = effectPrefab;
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

            // 공격 이펙트 생성
            if (effectPrefab != null)
            {
                GameObject effect = Object.Instantiate(effectPrefab, center, rotation);
                // 사각형 이펙트에 맞게 크기 조정이 필요할 수 있음
                Object.Destroy(effect, 1f); // 1초 후 이펙트 제거
            }

            // 사각형 영역 내 대상 검출
            Collider2D[] hitTargets = Physics2D.OverlapBoxAll(center, new Vector2(width, height), angle);

            // 대상에게 데미지 적용
            foreach (Collider2D target in hitTargets)
            {
                if (target.CompareTag(targetTag))
                {
                    // IDamageable 인터페이스가 있다면 사용할 수 있도록
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
            Matrix4x4 originalMatrix = Gizmos.matrix;

            // 회전 적용
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);

            // 사각형 그리기 (중심이 center이므로 좌표는 상대적으로 (0,0,0))
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, 0));

            // 원래 매트릭스로 복원
            Gizmos.matrix = originalMatrix;
        }
    }
}