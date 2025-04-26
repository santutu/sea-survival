using UnityEngine;
using UnityEngine.Audio;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendAudioMixer
    {
        public static AudioMixerGroup GetGroup(this AudioMixer audioMixer, string groupName)
        {
            var group = audioMixer.FindMatchingGroups(groupName);
            Debug.Assert(group.Length == 1);
            return group[0];
        }

        public static AudioMixer SetdB(this AudioMixer audioMixer, string parameterName, float sliderValue)
        {
            float dB;

            if (sliderValue <= 0.5f)
            {
                dB = Mathf.Lerp(-80f, 0f, sliderValue * 2f);
                //0~ 0.5
                //-80 ~ 0
            }
            else
            {
                //0.5~1
                //0~ 20

                float normalizedValue = (sliderValue - 0.5f) * 2f;
                dB = Mathf.Lerp(0f, 20f, normalizedValue);
            }

            audioMixer.SetFloat(parameterName, dB);

            return audioMixer;
        }
    }
}