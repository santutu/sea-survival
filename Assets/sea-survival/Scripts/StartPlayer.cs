using System;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class StartPlayer : SingletonMonoBehaviour<StartPlayer>
    {
        private Animator _animator;
        [SerializeField] private Transform fallingStartTf;
        [SerializeField] private Transform arriveTf;
        [SerializeField] private float fallingSpeed = 1f;

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _animator.SetBool(AnimState.IsFallin, true);
            transform.position = fallingStartTf.position;
        }

        private void Update()
        {
            //todo arriveTf 까지 이동후 애니메이션 false  
        }
    }
}