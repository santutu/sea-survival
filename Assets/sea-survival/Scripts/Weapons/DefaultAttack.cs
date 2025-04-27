using System.Collections;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Attacks;
using sea_survival.Scripts.Players;
using TMPro;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public class DefaultAttack : SingletonMonoBehaviour<DefaultAttack>
    {
        [SerializeField] private float attackInterval = 1.5f; // 공격 간격 (초)

        private Player Player => Player.Ins;
        private float _attackTimer = 0f;
        [SerializeField] private RectangleAttack rectangleAttack;


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
            // 플레이어 위치 기준으로 사각형 형태로 공격
            Vector2 playerPosition = transform.position;
            Vector2 attackDirection = GetAttackDirection();

            // RectangleAttack 클래스를 사용하여 공격 수행
            rectangleAttack.PerformAttack(playerPosition, attackDirection);
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
            Vector2 direction = GetAttackDirection();
            rectangleAttack.DrawGizmo(transform.position, direction);
        }
    }
}