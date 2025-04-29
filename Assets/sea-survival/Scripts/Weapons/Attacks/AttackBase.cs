using UnityEngine;

namespace sea_survival.Scripts.Attacks
{
    // 모든 공격 타입의 기본 인터페이스
    public interface IAttack
    {
        void PerformAttack(Vector2 position, Vector2 direction, string targetTag = "Enemy");
        void DrawGizmo(Vector3 position, Vector2 direction);
    }
} 