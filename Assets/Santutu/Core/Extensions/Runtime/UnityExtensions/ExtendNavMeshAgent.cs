using UnityEngine;
using UnityEngine.AI;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendNavMeshAgent
    {
        public static bool IsArrived(this NavMeshAgent agent, float stoppingDistance = 0.01f)
        {
            return Vector3.SqrMagnitude(agent.destination - agent.transform.position) <= (stoppingDistance * stoppingDistance);
        }
    }
}