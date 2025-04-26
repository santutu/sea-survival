using System;
using System.Collections;
using Santutu.Core.Extensions.Runtime.CSharpExtensions;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityStaticExtensions
{
    public static class MathfEx
    {
        public static bool Percent(float percent)
        {
            percent.ThrowIfNotInRange(0, 1);

            float randomValue = (float)new System.Random().NextDouble();

            return randomValue < percent;
        }

        public static float Ceil(float f, int @decimal = 1)
        {
            if (@decimal == 1)
            {
                return Mathf.Ceil(f);
            }

            var decimalMultiply = (10 ^ (@decimal - 1));
            f *= decimalMultiply;
            f = Mathf.Ceil(f);

            return f / decimalMultiply;
        }

        public static float Floor(float f, int @decimal = 1)
        {
            if (@decimal == 1)
            {
                return Mathf.Floor(f);
            }

            var decimalMultiply = (10 ^ (@decimal - 1));
            f *= decimalMultiply;
            f = Mathf.Floor(f);

            return f / decimalMultiply;
        }

        public static IEnumerator LerpInterpolationRoutine(float a, float b, float duration, Action<float, float> cb)
        {
            float elapsed = 0;

            while (true)
            {
                var time = elapsed / duration;
                var value = Mathf.Lerp(a, b, time);

                cb(value, time);

                if (time >= 1)
                {
                    break;
                }

                yield return null;
                elapsed += Time.deltaTime;
            }
        }

        public static IEnumerator LerpInterpolationRoutine(
            float a, float b, float duration, float startInterpolation, Action<float, float> cb
        )
        {
            var distance = b - a;
            var startTime = (startInterpolation - a) / distance;
            var startElapsed = duration * startTime;
            var elapsed = startElapsed;

            while (true)
            {
                var time = elapsed / duration;
                var value = Mathf.Lerp(a, b, time);

                cb(value, time);

                if (time >= 1)
                {
                    break;
                }

                yield return null;
                elapsed += Time.deltaTime;
            }
        }
    }
}