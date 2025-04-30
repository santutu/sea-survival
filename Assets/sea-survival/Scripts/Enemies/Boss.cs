using System;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.StageSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace sea_survival.Scripts.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Boss : SingletonMonoBehaviour<Boss>
    {
        private Enemy _enemy;
        public Image hpbarImage;
        public Image angryGageImage;

        [SerializeField, ReadOnly] private float angryGage = 0;
        [SerializeField] private float maxAngryGage = 100f;
        [SerializeField] private float angryGageIncreaseRate = 5f; // 초당 증가량

        [Header("미사일 설정")] [SerializeField] private GameObject missilePrefab; // 미사일 프리팹
        [SerializeField] private int missileCount = 36; // 360도 발사할 미사일 개수 (10도마다 하나)
        [SerializeField] private float missileDamage = 0f; // 미사일 데미지 (적의 데미지에 비례하여 자동 계산)
        [SerializeField] private float missileSpeed = 5f; // 미사일 속도
        [SerializeField] private float missileLifetime = 3f; // 미사일 수명

        private bool _isAngrySkillActive = false;

        protected override void Awake()
        {
            base.Awake();
            _enemy = GetComponent<Enemy>();

            // 미사일 데미지 설정 (플레이어 최대 체력의 1/3)
            missileDamage = Players.Player.Ins.maxHp / 3f;
        }

        private void Update()
        {
            // 체력바 업데이트
            if (hpbarImage != null && _enemy != null)
            {
                hpbarImage.fillAmount = _enemy.currentHealth / _enemy.maxHealth;
            }

            // 분노 게이지 증가
            if (angryGage < maxAngryGage && !_isAngrySkillActive)
            {
                angryGage += angryGageIncreaseRate * Time.deltaTime;

                // 분노 게이지 UI 업데이트
                if (angryGageImage != null)
                {
                    angryGageImage.fillAmount = angryGage / maxAngryGage;
                }

                // 분노 게이지가 꽉 차면 특수 공격 발동
                if (angryGage >= maxAngryGage)
                {
                    StartAngrySkill();
                }
            }
        }

        private void StartAngrySkill()
        {
            _isAngrySkillActive = true;

            // 360도 방향으로 미사일 발사
            FireMissilesIn360Degrees();

            // 분노 게이지 초기화
            angryGage = 0;
            if (angryGageImage != null)
            {
                angryGageImage.fillAmount = 0;
            }

            // 스킬 사용 완료
            _isAngrySkillActive = false;
        }

        private void FireMissilesIn360Degrees()
        {
            // 미사일 프리팹이 없으면 리턴
            if (missilePrefab == null)
            {
                Debug.LogWarning("미사일 프리팹이 설정되지 않았습니다!");
                return;
            }

            // 360도 방향으로 미사일 발사
            float angleStep = 360f / missileCount;
            for (int i = 0; i < missileCount; i++)
            {
                float angle = i * angleStep;
                Vector2 direction = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                // 미사일 생성
                GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
                BossMissile bossMissile = missile.AddComponent<BossMissile>();

                // 미사일에 콜라이더 추가 (없다면)
                if (missile.GetComponent<Collider2D>() == null)
                {
                    CircleCollider2D collider = missile.AddComponent<CircleCollider2D>();
                    collider.isTrigger = true;
                    collider.radius = 0.3f;
                }


                // 미사일 초기화
                bossMissile.Initialize(missileDamage, direction, missileSpeed, missileLifetime);

                // 일정 시간 후 미사일 제거
                Destroy(missile, missileLifetime);
            }
        }

        private void OnDestroy()
        {
            StageManager.Ins.BossDefeated();
        }
    }

    // 보스 미사일 컨트롤러 클래스
    public class BossMissile : MonoBehaviour
    {
        private float _damage;
        private Vector2 _direction;
        private float _speed;

        public void Initialize(float damage, Vector2 direction, float speed, float lifetime)
        {
            _damage = damage;
            _direction = direction;
            _speed = speed;
        }

        private void Update()
        {
            // 설정된 방향으로 이동
            transform.Translate(_direction * _speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 플레이어와 충돌했을 경우
            if (collision.CompareTag("Player"))
            {
                // 플레이어에게 데미지 적용
                Contracts.IDamageable player = collision.GetComponent<Contracts.IDamageable>();
                if (player != null)
                {
                    player.TakeDamage(_damage);
                }

                // 미사일 제거
                Destroy(gameObject);
            }
        }
    }
}