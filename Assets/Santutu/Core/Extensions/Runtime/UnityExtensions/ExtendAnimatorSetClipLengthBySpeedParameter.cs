using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendAnimatorSetClipLengthBySpeedParameter
    {
        public static bool TrySetClipLengthBySpeedParameter(
            this Animator animator,
            string layerName,
            int nameHash,
            int speedHash,
            float desiredLength
        )
        {
            return animator.TrySetClipLengthBySpeedParameter(
                animator.GetLayerIndex(layerName),
                nameHash,
                speedHash,
                desiredLength
            );
        }

        public static bool TrySetClipLengthBySpeedParameter(
            this Animator animator,
            int layerIndex,
            int nameHash,
            int speedHash,
            float desiredLength
        )
        {
            if (animator.IsInTransition(layerIndex))
            {
                return animator.TrySetNextStateClipLengthBySpeedParameter(
                    layerIndex,
                    nameHash,
                    speedHash,
                    desiredLength
                );
            }
            else
            {
                return animator.TrySetCurrentStateClipLengthBySpeedParameter(
                    layerIndex,
                    nameHash,
                    speedHash,
                    desiredLength
                );
            }
        }

        private static void TrySetCurrentStateClipLengthBySpeedParameter(
            this Animator animator,
            string layerName,
            int nameHash,
            int speedHash,
            float desiredLength
        )
        {
            animator.TrySetCurrentStateClipLengthBySpeedParameter(
                animator.GetLayerIndex(layerName),
                nameHash,
                speedHash,
                desiredLength
            );
        }

        private static bool TrySetNextStateClipLengthBySpeedParameter(
            this Animator animator,
            int layer,
            int nameHash,
            int speedHash,
            float desiredLength
        )
        {
            var aniStateInfo = animator.GetNextAnimatorStateInfo(layer);

            if (aniStateInfo.shortNameHash != nameHash)
            {
                return false;
            }

            var originalLength = aniStateInfo.length * aniStateInfo.speedMultiplier;
            animator.SetFloat(speedHash, originalLength / desiredLength);

            return true;
        }

        private static bool TrySetCurrentStateClipLengthBySpeedParameter(
            this Animator animator,
            int layer,
            int nameHash,
            int speedHash,
            float desiredLength
        )
        {
            var aniStateInfo = animator.GetCurrentAnimatorStateInfo(layer);


            if (aniStateInfo.shortNameHash != nameHash)
            {
                return false;
            }

            var originalLength = aniStateInfo.length * aniStateInfo.speedMultiplier;
            animator.SetFloat(speedHash, originalLength / desiredLength);


            return true;
        }
    }
}