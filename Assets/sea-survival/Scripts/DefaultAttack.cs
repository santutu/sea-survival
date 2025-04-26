using System.Collections;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;
using TMPro;

namespace sea_survival.Scripts
{
    public class DefaultAttack : SingletonMonoBehaviour<DefaultAttack>
    {
        [Header("공격 설정")] [SerializeField] private float attackDamage = 20f; // 공격 데미지
        [SerializeField] private float attackInterval = 1.5f; // 공격 간격 (초)
        [SerializeField] private float attackRange = 5f; // 공격 범위
        [SerializeField] private float attackAngle = 60f; // 부채꼴 각도

        [Header("이펙트")] [SerializeField] private GameObject attackEffectPrefab; // 공격 이펙트 프리팹
        [SerializeField] private bool showDamageText = true; // 데미지 텍스트 표시 여부

        private Player Player => Player.Ins;
        private float _attackTimer = 0f;


        private void Update()
        {
            // 공격 타이머 업데이트
            _attackTimer += Time.deltaTime;

            // 공격 간격마다 공격 수행
            if (_attackTimer >= attackInterval)
            {
                PerformAttack();
                _attackTimer = 0f;
            }
        }

        private void PerformAttack()
        {
            // 플레이어 위치 기준으로 부채꼴 형태로 공격
            Vector2 playerPosition = transform.position;
            Vector2 attackDirection = GetAttackDirection();

            // 부채꼴 범위 내의 적 검출
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(playerPosition, attackRange);

            // 공격 이펙트 생성
            if (attackEffectPrefab != null)
            {
                GameObject effect = Instantiate(attackEffectPrefab, playerPosition, Quaternion.identity);
                // 이펙트 방향 설정
                float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
                effect.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                Destroy(effect, 1f); // 1초 후 이펙트 제거
            }

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    // 적 방향 계산
                    Vector2 directionToEnemy = enemy.transform.position - transform.position;
                    float angleToEnemy = Vector2.Angle(attackDirection, directionToEnemy);

                    // 부채꼴 각도 내에 있는 적만 데미지 적용
                    if (angleToEnemy <= attackAngle / 2)
                    {
                        Enemy enemyComponent = enemy.GetComponent<Enemy>();
                        if (enemyComponent != null)
                        {
                            enemyComponent.TakeDamage(attackDamage);
                        }
                    }
                }
            }
        }


        private IEnumerator AnimateDamageText(GameObject textObj)
        {
            float duration = 1.0f;
            float elapsed = 0.0f;
            Vector3 startPos = textObj.transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                // 위로 올라가면서 페이드 아웃
                float t = elapsed / duration;
                textObj.transform.position = startPos + new Vector3(0, t * 0.5f, 0);

                // TextMeshPro 또는 TextMesh 찾아서 알파값 조정
                TextMeshPro tmpText = textObj.GetComponent<TextMeshPro>();
                if (tmpText != null)
                {
                    Color c = tmpText.color;
                    c.a = 1 - t;
                    tmpText.color = c;
                }
                else
                {
                    TextMesh textMesh = textObj.GetComponent<TextMesh>();
                    if (textMesh != null)
                    {
                        Color c = textMesh.color;
                        c.a = 1 - t;
                        textMesh.color = c;
                    }
                }

                yield return null;
            }

            // 애니메이션 종료 후 오브젝트 제거
            Destroy(textObj);
        }

        private Vector2 GetAttackDirection()
        {
            // 플레이어가 바라보는 방향으로 공격 방향 결정
            if (Player == null) return Vector2.right; // 기본값으로 오른쪽

            SpriteRenderer playerSprite = Player.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                // flipX가 true면 왼쪽, false면 오른쪽
                return playerSprite.flipX ? Vector2.left : Vector2.right;
            }

            return Vector2.right; // 기본값
        }

        // 디버그용 기즈모 그리기 (에디터에서만 보임)
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Vector2 direction = GetAttackDirection();

            Gizmos.color = Color.red;
            Vector3 position = transform.position;

            // 부채꼴 그리기
            float halfAngle = attackAngle / 2 * Mathf.Deg2Rad;
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

            Gizmos.DrawLine(position, position + leftDir * attackRange);
            Gizmos.DrawLine(position, position + rightDir * attackRange);

            float angleStep = (attackAngle * Mathf.Deg2Rad) / segments;
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

                Gizmos.DrawLine(position + currentDir * attackRange, position + nextDir * attackRange);
            }
        }
    }
}