using System;
using Santutu.Core.Base.Runtime.Singletons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace sea_survival.Scripts.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Boss : SingletonMonoBehaviour<Boss>
    {
        public UnityEvent onDeath = new UnityEvent();


        private Enemy _enemy;
        public Image hpbarImage;
        public Image angryGageImage;

        [SerializeField, ReadOnly] private float angryGage = 0;

        protected override void Awake()
        {
            _enemy = GetComponent<Enemy>();
        }

        private void Update()
        {
            //todo: 보스 기술 구현  angryGage 가 꽉차면 360 방향으로 미사일 다수 발사 (맞으면 플레이어피 max 피의 1/3 깍임)
            //todo :  hpbarImage 와 enemy 의 hp 동기화
        }

        private void OnDestroy()
        {
            onDeath?.Invoke();
        }
    }
}