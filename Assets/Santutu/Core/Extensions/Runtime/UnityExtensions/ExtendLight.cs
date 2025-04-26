using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendLight
    {
        public static async UniTaskVoid To(this Light light, Light to, float duration)
        {
            await TransitionLightAsync(light, to, duration);
        }

        

        private static async UniTask TransitionLightAsync(Light light, Light to, float duration)
        {
            float elapsedTime = 0f;
            Color startColor = light.color;
            float startIntensity = light.intensity;
            float startIndirectMultiplier = light.bounceIntensity;
            float startTemperature = light.colorTemperature;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                light.color = Color.Lerp(startColor, to.color, t);
                light.intensity = Mathf.Lerp(startIntensity, to.intensity, t);
                light.bounceIntensity = Mathf.Lerp(startIndirectMultiplier, to.bounceIntensity, t);
                light.colorTemperature = Mathf.Lerp(startTemperature, to.colorTemperature, t);

                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            light.color = to.color;
            light.intensity = to.intensity;
            light.bounceIntensity = to.bounceIntensity;
            light.colorTemperature = to.colorTemperature;
        }
    }
}