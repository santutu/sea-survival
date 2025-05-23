using UnityEngine;

namespace sea_survival.Scripts.Contracts
{
    public interface IEnemyAttack
    {
        bool CanAttack { get; }
        float AttackCooldown { get; }
        float AttackRange { get; }
        void Attack(Transform target);
        void ResetAttackCooldown();
    }
} 