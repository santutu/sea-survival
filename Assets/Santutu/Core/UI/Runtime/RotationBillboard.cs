using System;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class RotationBillboard : MonoBehaviour
    {
        [SerializeField, HideInInspector] private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}