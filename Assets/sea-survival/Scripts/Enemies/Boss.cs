using System;
using UnityEngine;
using UnityEngine.Events;

namespace sea_survival.Scripts.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Boss : MonoBehaviour
    {
        public UnityEvent onDeath = new UnityEvent();

        private void Update()
        {
            //todo: 보스 기술 구현
        }

        private void OnDestroy()
        {
            onDeath?.Invoke();
        }
    }
}