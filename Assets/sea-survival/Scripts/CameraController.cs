using System;
using Santutu.Core.Base.Runtime.Singletons;
using sea_survival.Scripts.Players;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class CameraController : SingletonMonoBehaviour<CameraController>
    {
        private void LateUpdate()
        {
            if (Player.Ins == null)
            {
                return;
            }

            Vector3 newPosition = Player.Ins.transform.position;
            transform.position = newPosition;
        }
    }
}