using System;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class CameraController : SingletonMonoBehaviour<CameraController>
    {
        private void LateUpdate()
        {
            Vector3 newPosition = Player.Ins.transform.position;
            transform.position = newPosition;
        }
    }
}