using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.Weapons;
using UnityEngine;

namespace sea_survival.Scripts.Players
{
    public class StartPlayer : SingletonMonoBehaviour<StartPlayer>
    {
        private Animator _animator;
        [SerializeField] private Transform fallingStartTf;
        [SerializeField] private Transform arriveTf;
        [SerializeField] private float fallingSpeed = 1f;
        [SerializeField] private float landingDelay = 0.5f;

        private bool _isLanding = false;
        private float _landingTimer = 0f;
        private Player _playerComponent;

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            _playerComponent = GetComponent<Player>();
        }

        private void Start()
        {
            _animator.SetBool(AnimState.IsFallin, true);
            transform.position = fallingStartTf.position;
            _playerComponent.enabled = false;
            EnemySpawner.Ins.enabled = false;
            DefaultAttack.Ins.enabled = false;
        }

        private void Update()
        {
            if (_isLanding)
            {
                // 착지 후 딜레이 타이머 실행
                _landingTimer += Time.deltaTime;
                if (_landingTimer >= landingDelay)
                {
                    // 착지 애니메이션 종료 및 플레이어 조작 활성화
                    _animator.SetBool(AnimState.IsFallin, false);
                    _animator.SetBool(AnimState.IsIdle, true);

                    // 플레이어 컴포넌트 활성화 (없을 경우 추가)

                    EnemySpawner.Ins.enabled = true;
                    _playerComponent.enabled = true;
                    DefaultAttack.Ins.enabled = true;


                    // 이 스크립트는 역할을 마쳤으므로 비활성화
                    enabled = false;
                }

                return;
            }

            // 목표 지점까지 이동
            float step = fallingSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, arriveTf.position, step);

            // 목표 지점에 도달했는지 확인
            if (Vector3.Distance(transform.position, arriveTf.position) < 0.01f)
            {
                // 도착 지점에 도달하면 착지 상태로 전환
                _isLanding = true;

                // 착지 시 먼지 효과나 소리 등을 여기에 추가
                if (GetComponent<AudioSource>() != null)
                {
                    GetComponent<AudioSource>().Play();
                }
            }
        }
    }
}