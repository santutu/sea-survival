using System;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class CameraController : SingletonMonoBehaviour<CameraController>
    {

        float offsetX;

        protected override void Awake()
        {
            base.Awake();
            offsetX = Player.Ins.transform.position.x - transform.position.x;

        }



        private void LateUpdate()
        {
            if (Player.Ins == null)
            {
                return;
            }

            Vector3 newPosition = transform.position;
            newPosition.x = Player.Ins.transform.position.x - offsetX;

            transform.position = newPosition;
        }
    }
}