using System;

namespace DeftHands
{
    public struct RotationStep
    {
        public readonly float PitchDegrees;
        public readonly float HorizontalDegrees;
        public readonly bool UseYaw;

        public RotationStep(float pitchDegrees, float horizontalDegrees, bool useYaw)
        {
            PitchDegrees = pitchDegrees;
            HorizontalDegrees = horizontalDegrees;
            UseYaw = useYaw;
        }
    }

    public static class RotateMath
    {
        public static bool CanManipulateItem(
            bool hasHeldItem,
            bool canRotate,
            bool isInGridMode,
            bool isParentedToPlayer,
            bool isCursorLocked)
        {
            return hasHeldItem
                && canRotate
                && !isInGridMode
                && isParentedToPlayer
                && isCursorLocked;
        }

        public static RotationStep CalculateRotationStep(
            float deltaX,
            float deltaY,
            float speed,
            bool defaultYaw,
            bool swapHeld,
            bool invertPitch,
            bool invertRoll,
            bool invertYaw)
        {
            bool useYaw = defaultYaw != swapHeld;
            float pitchSign = invertPitch ? -1f : 1f;
            float horizontalSign = (useYaw ? invertYaw : invertRoll) ? 1f : -1f;

            return new RotationStep(
                deltaY * speed * pitchSign,
                deltaX * speed * horizontalSign,
                useYaw);
        }

        public static float ClampDepthTarget(float currentDepth, float targetDepth, float minDepth, float maxDepth)
        {
            float lower = Math.Min(currentDepth, minDepth);
            float upper = Math.Max(currentDepth, maxDepth);
            if (targetDepth < lower) return lower;
            if (targetDepth > upper) return upper;
            return targetDepth;
        }

        public static float EaseTowardDepth(float currentDepth, float targetDepth, float rate, float deltaTime, float snapEpsilon)
        {
            float remaining = targetDepth - currentDepth;
            if (Math.Abs(remaining) <= snapEpsilon)
            {
                return targetDepth;
            }

            float alpha = 1f - (float)Math.Exp(-rate * Math.Max(0f, deltaTime));
            return currentDepth + remaining * alpha;
        }
    }
}
