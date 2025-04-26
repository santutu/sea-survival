using UnityEngine;

namespace Santutu.Modules.UI.Runtime
{
    public class WorldCanvasChild : MonoBehaviour
    {
        private void Start()
        {
            //todo: 월드 캔바스 크기는 상관없음. 원근 카메라 크기에 맞춰야함. 카메라 시야 거리에 위치해야함.
            
            var rtf = (RectTransform)transform;
            rtf.anchorMax = Vector2.one;
            rtf.anchorMin = Vector2.zero;
            rtf.pivot = new(0.5f, 0.5f);
        }

        private void Update()
        {
            var camera = Camera.main;

            //카메라 시야 거리 중간으로 이동.
            //todo:  카메라 크기에도 맞춰야함
            transform.position = (((camera.nearClipPlane + camera.farClipPlane) / 2) * camera.transform.forward) + camera.transform.position;
            
        }
    }
}