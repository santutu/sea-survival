using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Players;
using System.Collections;
using sea_survival.Scripts.Enemies;
using sea_survival.Scripts.StageSystem;
using sea_survival.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace sea_survival.Scripts
{
    public class FallingCinematicManager : SingletonMonoBehaviour<FallingCinematicManager>
    {
        [SerializeField] private GameObject airPlainStartPoint;
        [SerializeField] private GameObject airPlainEndPoint;
        [SerializeField] private GameObject playerAppearPoint;
        [SerializeField] private float airPlainSpeed = 5f;

        [SerializeField] private GameObject startCameraPoint;
        [SerializeField] private GameObject airPlainObj;

        [SerializeField] private float playerFallingSpeed = 5f;
        [SerializeField] private float cinematicDelay = 1f;

        [SerializeField] private Camera mainCamera;
        private Coroutine _cinematicCoroutine;


        public void StartCinematic()
        {
            if (_cinematicCoroutine != null)
            {
                StopCoroutine(_cinematicCoroutine);
            }

            _cinematicCoroutine = StartCoroutine(PlayCinematicSequence());
        }

        private IEnumerator PlayCinematicSequence()
        {
            Player.Ins.gameObject.SetActive(false);
            Player.Ins.area.SetActive(false);
            Player.Ins.enabled = false;
            CameraController.Ins.enabled = false;
            WeaponManager.Ins.gameObject.SetActive(false);


            // 카메라를 해당 포인트로 옮긴다
            mainCamera.transform.position = startCameraPoint.transform.position;


            yield return new WaitForSeconds(cinematicDelay);

            // 비행기 오브젝트를 시작 포인트로 옮긴다
            if (airPlainObj != null && airPlainStartPoint != null)
            {
                airPlainObj.transform.position = airPlainStartPoint.transform.position;
                airPlainObj.SetActive(true);
            }

            yield return new WaitForSeconds(cinematicDelay);

            // 비행기를 end 포인트까지 airPlainSpeed로 움직인다
            bool playerSpawned = false;

            while (airPlainObj != null && airPlainEndPoint != null && Vector3.Distance(airPlainObj.transform.position, airPlainEndPoint.transform.position) > 0.1f)
            {
                airPlainObj.transform.position = Vector3.MoveTowards(
                    airPlainObj.transform.position,
                    airPlainEndPoint.transform.position,
                    airPlainSpeed * Time.deltaTime
                );

                // playerAppearPoint의 x값보다 비행기 위치 x 값이 커지면 playerAppearPoint에서 플레이어가 나타난다
                if (!playerSpawned && playerAppearPoint != null && airPlainObj.transform.position.x > playerAppearPoint.transform.position.x)
                {
                    playerSpawned = true;
                    StartCoroutine(SpawnPlayer());
                }

                yield return null;
            }

            // 비행기가 끝 지점에 도달하면 비활성화
            if (airPlainObj != null)
            {
                airPlainObj.SetActive(false);
            }

            // 시네마틱이 끝나면 플레이어가 아직 스폰되지 않았다면 스폰
            if (!playerSpawned)
            {
                StartCoroutine(SpawnPlayer());
            }
        }

        private IEnumerator SpawnPlayer()
        {
            Player.Ins.transform.position = playerAppearPoint.transform.position;
            Player.Ins.gameObject.SetActive(true);
            Player.Ins.SetAnimation(AnimState.IsIdle, false);
            Player.Ins.SetAnimation(AnimState.IsFalling, true);


            // 플레이어가 낙하하는 효과
            float fallDuration = 2f;
            float elapsedTime = 0f;
            Vector3 startPos = Player.Ins.transform.position;
            Vector3 targetPos = new Vector3(startPos.x, startPos.y - 4.4f, startPos.z);

            while (elapsedTime < fallDuration)
            {
                Player.Ins.transform.position = Vector3.Lerp(
                    startPos,
                    targetPos,
                    elapsedTime / fallDuration
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }


            // 카메라를 플레이어 위치로 이동

            Player.Ins.transform.position = Vector3.zero;

            Player.Ins.SetAnimation(AnimState.IsFalling, false);
            Player.Ins.SetAnimation(AnimState.IsIdle, false);


            Player.Ins.enabled = true;
            WeaponManager.Ins.gameObject.SetActive(true);
            CameraController.Ins.enabled = true;
            Player.Ins.area.SetActive(true);
        }
    }
}