using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace sea_survival.Scripts.Players
{
    public class PlayerSpeedDetector : MonoBehaviour
    {
        [Header("속도 계산 설정")]
        public float speedSmoothTime = 0.1f;
        public int speedSampleCount = 5;
        
        private Queue<float> speedSamples;
        private Vector3 lastPosition;
        private Vector3 currentPosition;
        private Vector2 moveDirection;

        [SerializeField,ReadOnly]
        private float smoothedSpeed;
        [SerializeField,ReadOnly]
        private float speedVelocity;
        
        [SerializeField,ReadOnly]
        private Vector2 currentMoveDirection;
        
        private void Start()
        {
            speedSamples = new Queue<float>();
            lastPosition = transform.position;
        }
        
        private void Update()
        {
            CalculateSpeed();
            SmoothSpeed();
        }
        
        private void CalculateSpeed()
        {
            currentPosition = transform.position;
            float currentSpeed = Vector3.Distance(currentPosition, lastPosition) / Time.deltaTime;
            
            // 이동 방향 계산
            Vector3 movement = currentPosition - lastPosition;
            if (movement.magnitude > 0.01f) // 최소 이동 거리 임계값
            {
                moveDirection = new Vector2(movement.x, movement.y).normalized;
                currentMoveDirection = moveDirection;
            }
            
            speedSamples.Enqueue(currentSpeed);
            if (speedSamples.Count > speedSampleCount)
                speedSamples.Dequeue();
            
            lastPosition = currentPosition;
        }
        
        private void SmoothSpeed()
        {
            float averageSpeed = 0f;
            foreach (float speed in speedSamples)
                averageSpeed += speed;
            
            averageSpeed /= speedSamples.Count;
            
            smoothedSpeed = Mathf.SmoothDamp(smoothedSpeed, averageSpeed, 
                                            ref speedVelocity, speedSmoothTime);
        }
        
        public float GetSmoothedSpeed()
        {
            return smoothedSpeed;
        }
        
        public Vector2 GetMoveDirection()
        {
            return moveDirection;
        }
        
        public Vector2 GetCurrentVelocity()
        {
            return (transform.position - lastPosition) / Time.deltaTime;
        }
    }
} 