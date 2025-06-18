using UnityEngine;

namespace sea_survival.Scripts.Players
{
    [System.Serializable]
    public class BubbleSpeedConfig
    {
        [Header("속도 범위")]
        public float minSpeed;
        public float maxSpeed;
        
        [Header("파티클 설정")]
        public int emissionRate;        // 초당 발생 개수
        public float minSize;           // 최소 크기
        public float maxSize;           // 최대 크기
        public float lifetime;          // 지속 시간
        public Color bubbleColor;       // 공기방울 색상
        public float alphaMultiplier;   // 투명도 배수
        
        [Header("추가 효과")]
        public bool hasSparkleEffect;   // 반짝임 효과 여부
        public bool hasWaveEffect;      // 물결 효과 여부
    }
} 