using System.Collections.Generic;
using sea_survival.Scripts.Enums;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts.Weapons
{
    public class WeaponManager : SingletonMonoBehaviour<WeaponManager>
    {
        [SerializeField] private Transform weaponsParent;
        [SerializeField] private BasicWeapon basicWeaponPrefab;
        [SerializeField] private MagicMissile magicMissilePrefab;
        [SerializeField] private GameObject daggerPrefab;
        [SerializeField] private GameObject boomerangPrefab;
        [SerializeField] private GameObject electricOrbPrefab;
        [SerializeField] private GameObject sonicWavePrefab;

        private Dictionary<WeaponType, IWeapon> activeWeapons = new Dictionary<WeaponType, IWeapon>();
        private Dictionary<WeaponType, GameObject> weaponPrefabs = new Dictionary<WeaponType, GameObject>();

        protected override void Awake()
        {
            base.Awake();
            InitializeWeaponPrefabsDictionary();
            InitializeDefaultWeapon();
        }

        // 무기 프리팹 딕셔너리 초기화
        private void InitializeWeaponPrefabsDictionary()
        {
            if (basicWeaponPrefab != null)
                weaponPrefabs[WeaponType.BasicWeapon] = basicWeaponPrefab.gameObject;

            if (magicMissilePrefab != null)
                weaponPrefabs[WeaponType.MagicMissile] = magicMissilePrefab.gameObject;

            if (daggerPrefab != null)
                weaponPrefabs[WeaponType.Dagger] = daggerPrefab;

            if (boomerangPrefab != null)
                weaponPrefabs[WeaponType.Boomerang] = boomerangPrefab;

            if (electricOrbPrefab != null)
                weaponPrefabs[WeaponType.ElectricOrb] = electricOrbPrefab;

            if (sonicWavePrefab != null)
                weaponPrefabs[WeaponType.SoundWave] = sonicWavePrefab;
        }

        // 기본 무기 초기화
        private void InitializeDefaultWeapon()
        {
            AddWeapon(WeaponType.BasicWeapon);
        }

        // 새 무기 추가
        public bool AddWeapon(WeaponType weaponType)
        {
            // 이미 해당 무기가 있는 경우 레벨업
            if (activeWeapons.ContainsKey(weaponType))
            {
                return LevelUpWeapon(weaponType);
            }

            // 새 무기 생성
            if (weaponPrefabs.TryGetValue(weaponType, out GameObject prefab))
            {
                GameObject weaponObj = Instantiate(prefab, weaponsParent);
                IWeapon weapon = weaponObj.GetComponent<IWeapon>();

                if (weapon != null)
                {
                    activeWeapons[weaponType] = weapon;
                    return true;
                }
                else
                {
                    // 아직 구현되지 않은 무기 경고 출력
                    Debug.LogWarning($"무기 타입 {weaponType}는 아직 구현되지 않았습니다.");
                    Destroy(weaponObj); // 생성된 오브젝트 제거
                    return false;
                }
            }

            Debug.LogWarning($"무기 타입 {weaponType}에 대한 프리팹을 찾을 수 없습니다.");
            return false;
        }

        // 무기 레벨업
        public bool LevelUpWeapon(WeaponType weaponType)
        {
            if (activeWeapons.TryGetValue(weaponType, out IWeapon weapon))
            {
                return weapon.LevelUp();
            }

            return false;
        }

        // 해당 무기가 이미 활성화되어 있는지 확인
        public bool HasWeapon(WeaponType weaponType)
        {
            return activeWeapons.ContainsKey(weaponType);
        }

        // 해당 무기의 현재 레벨 가져오기
        public WeaponLevel GetWeaponLevel(WeaponType weaponType)
        {
            if (activeWeapons.TryGetValue(weaponType, out IWeapon weapon))
            {
                return weapon.CurrentLevel;
            }

            return WeaponLevel.Level1; // 기본값
        }

        // 해당 무기가 최대 레벨인지 확인
        public bool IsWeaponMaxLevel(WeaponType weaponType)
        {
            if (activeWeapons.TryGetValue(weaponType, out IWeapon weapon))
            {
                return !weapon.CanLevelUp();
            }

            return false;
        }

        // 모든 활성 무기 가져오기
        public Dictionary<WeaponType, IWeapon> GetAllActiveWeapons()
        {
            return activeWeapons;
        }
        
        // 모든 무기 활성화/비활성화
        public void SetWeaponsEnabled(bool enabled)
        {
            foreach (var weaponPair in activeWeapons)
            {
                IWeapon weapon = weaponPair.Value;
                if (weapon != null)
                {
                    // IWeapon을 구현하는 MonoBehaviour 컴포넌트의 enabled 상태 변경
                    if (weapon is MonoBehaviour weaponMono)
                    {
                        weaponMono.enabled = enabled;
                    }
                }
            }
            
            Debug.Log($"모든 무기 {(enabled ? "활성화" : "비활성화")} 완료");
        }
        
        // 특정 무기 활성화/비활성화
        public void SetWeaponEnabled(WeaponType weaponType, bool enabled)
        {
            if (activeWeapons.TryGetValue(weaponType, out IWeapon weapon))
            {
                if (weapon is MonoBehaviour weaponMono)
                {
                    weaponMono.enabled = enabled;
                    Debug.Log($"무기 {weaponType} {(enabled ? "활성화" : "비활성화")}");
                }
            }
        }
    }
}