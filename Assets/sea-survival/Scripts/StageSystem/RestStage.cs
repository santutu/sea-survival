using System.Collections;
using System.Collections.Generic;
using sea_survival.Scripts.CardSystem;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts.Stages
{
    public class RestStage : StageState
    {
        [Header("휴식 설정")] [SerializeField] private float healAmount = 10f; // 초당 회복량

        private Player Player => Player.Ins;
        private Coroutine _healCoroutine;

        // 상태 진입시 호출
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log($"휴식 단계 진입: 스테이지 {StageManager.CurrentStageLv}");

            // 포탈 제거
            StageManager.DestroyPortal();
            Time.timeScale = 0;

            // 체력 회복 코루틴 시작
            if (_healCoroutine != null)
            {
                StopCoroutine(_healCoroutine);
            }

            _healCoroutine = StartCoroutine(HealOverTime());
        }

        // 상태 종료시 호출
        public override void OnExit()
        {
            // 체력 회복 코루틴 중지
            if (_healCoroutine != null)
            {
                StopCoroutine(_healCoroutine);
                _healCoroutine = null;
            }

            Debug.Log("휴식 단계 종료");
            Time.timeScale = 1;
            base.OnExit();
        }

        // 플레이어 체력 회복 코루틴
        private IEnumerator HealOverTime()
        {
            // 초당 체력 회복
            while (gameObject.activeSelf)
            {
                // 플레이어 체력 회복
                if (Player != null && Player.hp < Player.maxHp)
                {
                    Player.hp = Mathf.Min(Player.hp + (healAmount * Time.deltaTime), Player.maxHp);
                }

                yield return null;
            }
        }
    }

    // 카드 타입 열거형
    public enum CardType
    {
        Weapon,
        Stat
    }

    // 무기 타입 열거형
    public enum WeaponType
    {
        BasicWeapon,
        MagicMissile,
        Dagger,
        Boomerang,
        ElectricOrb,
        SonicWave
    }

    // 스탯 타입 열거형
    public enum StatType
    {
        MoveSpeed,
        MaxHP,
        HPRegen
    }
}