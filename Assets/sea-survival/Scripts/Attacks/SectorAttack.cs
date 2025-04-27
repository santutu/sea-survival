using sea_survival.Scripts.Contracts;
using sea_survival.Scripts.Enemies;
using UnityEngine;

namespace sea_survival.Scripts.Attacks
{
    public class SectorAttack : IAttack
    {
        // 공격 속성
        private float damage;
        private float range;
        private float angle;
        private GameObject effectPrefab;
        
        // 생성자
        public SectorAttack(float damage, float range, float angle, GameObject effectPrefab = null)
        {
            this.damage = damage;
            this.range = range;
            this.angle = angle;
            this.effectPrefab = effectPrefab;
        }
        
        // 공격 실행
        public void PerformAttack(Vector2 position, Vector2 direction, string targetTag = "Enemy")
        {
            // 부채꼴 범위 내의 타겟 검출
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(position, range);
            
            // 공격 이펙트 생성
            if (effectPrefab != null)
            {
                GameObject effect = Object.Instantiate(effectPrefab, position, Quaternion.identity);
                // 이펙트 방향 설정
                float effectAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                effect.transform.rotation = Quaternion.AngleAxis(effectAngle, Vector3.forward);
                Object.Destroy(effect, 1f); // 1초 후 이펙트 제거
            }
            
            // 대상에게 데미지 적용
            foreach (Collider2D target in hitTargets)
            {
                if (target.CompareTag(targetTag))
                {
                    // 타겟 방향 계산
                    Vector2 directionToTarget = target.transform.position - new Vector3(position.x, position.y, 0);
                    float angleToTarget = Vector2.Angle(direction, directionToTarget);
                    
                    // 부채꼴 각도 내에 있는 타겟만 데미지 적용
                    if (angleToTarget <= angle / 2)
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
        }
        
        // 디버그용 기즈모 그리기
        public void DrawGizmo(Vector3 position, Vector2 direction)
        {
            Gizmos.color = Color.red;
            
            // 부채꼴 그리기
            float halfAngle = angle / 2 * Mathf.Deg2Rad;
            int segments = 20;
            
            Vector3 leftDir = new Vector3(
                direction.x * Mathf.Cos(-halfAngle) - direction.y * Mathf.Sin(-halfAngle),
                direction.x * Mathf.Sin(-halfAngle) + direction.y * Mathf.Cos(-halfAngle),
                0
            );
            
            Vector3 rightDir = new Vector3(
                direction.x * Mathf.Cos(halfAngle) - direction.y * Mathf.Sin(halfAngle),
                direction.x * Mathf.Sin(halfAngle) + direction.y * Mathf.Cos(halfAngle),
                0
            );
            
            Gizmos.DrawLine(position, position + leftDir * range);
            Gizmos.DrawLine(position, position + rightDir * range);
            
            float angleStep = (angle * Mathf.Deg2Rad) / segments;
            for (int i = 0; i < segments; i++)
            {
                float currentAngle = -halfAngle + angleStep * i;
                float nextAngle = -halfAngle + angleStep * (i + 1);
                
                Vector3 currentDir = new Vector3(
                    direction.x * Mathf.Cos(currentAngle) - direction.y * Mathf.Sin(currentAngle),
                    direction.x * Mathf.Sin(currentAngle) + direction.y * Mathf.Cos(currentAngle),
                    0
                );
                
                Vector3 nextDir = new Vector3(
                    direction.x * Mathf.Cos(nextAngle) - direction.y * Mathf.Sin(nextAngle),
                    direction.x * Mathf.Sin(nextAngle) + direction.y * Mathf.Cos(nextAngle),
                    0
                );
                
                Gizmos.DrawLine(position + currentDir * range, position + nextDir * range);
            }
        }
    }
    
   
} 