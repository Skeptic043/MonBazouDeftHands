using EasyRotate;
using System;

internal static class CoreTests
{
    private static int Main()
    {
        TestEligibility();
        TestAxisSelectionAndInversion();
        TestDepthClamping();
        TestDepthEasing();
        Console.WriteLine("All EasyRotate core tests passed.");
        return 0;
    }

    private static void TestEligibility()
    {
        Assert(RotateMath.CanManipulateItem(true, true, false, true, true), "Valid held item should be controllable");
        Assert(!RotateMath.CanManipulateItem(false, true, false, true, true), "Null held item should not be controllable");
        Assert(!RotateMath.CanManipulateItem(true, false, false, true, true), "Non-rotatable (snapped part) item should not be controllable");
        Assert(!RotateMath.CanManipulateItem(true, true, true, true, true), "Grid mode should disable control");
        Assert(!RotateMath.CanManipulateItem(true, true, false, false, true), "Reparented (snapped bolt) item should not be controllable");
        Assert(!RotateMath.CanManipulateItem(true, true, false, true, false), "Unlocked cursor should disable control");
    }

    private static void TestAxisSelectionAndInversion()
    {
        RotationStep defaultRoll = RotateMath.CalculateRotationStep(1.5f, -2.0f, 2.0f, false, false, false, false, false);
        Assert(!defaultRoll.UseYaw, "Default horizontal axis should be Roll");
        AssertNear(defaultRoll.HorizontalDegrees, -3.0f, "Roll horizontal degrees");
        AssertNear(defaultRoll.PitchDegrees, -4.0f, "Pitch degrees");

        RotationStep swappedToYaw = RotateMath.CalculateRotationStep(1.5f, 2.0f, 2.0f, false, true, false, false, false);
        Assert(swappedToYaw.UseYaw, "Holding swap key should switch Roll to Yaw");
        AssertNear(swappedToYaw.HorizontalDegrees, -3.0f, "Yaw horizontal degrees");

        RotationStep yawSwappedBackToRoll = RotateMath.CalculateRotationStep(1.5f, 0f, 2.0f, true, true, false, true, false);
        Assert(!yawSwappedBackToRoll.UseYaw, "Holding swap key with Yaw default should switch back to Roll");
        AssertNear(yawSwappedBackToRoll.HorizontalDegrees, 3.0f, "Inverted roll should negate horizontal degrees");

        RotationStep invertedYawAndPitch = RotateMath.CalculateRotationStep(1.5f, 2.0f, 2.0f, true, false, true, false, true);
        Assert(invertedYawAndPitch.UseYaw, "Yaw default without swap should use Yaw");
        AssertNear(invertedYawAndPitch.HorizontalDegrees, 3.0f, "Inverted yaw should negate horizontal degrees");
        AssertNear(invertedYawAndPitch.PitchDegrees, -4.0f, "Inverted pitch should negate vertical degrees");
    }

    private static void TestDepthClamping()
    {
        AssertNear(RotateMath.ClampDepthTarget(1.5f, 1.65f, 0.5f, 3.0f), 1.65f, "Normal push within bounds");
        AssertNear(RotateMath.ClampDepthTarget(1.5f, 3.5f, 0.5f, 3.0f), 3.0f, "Clamp to max depth");
        AssertNear(RotateMath.ClampDepthTarget(1.5f, 0.2f, 0.5f, 3.0f), 0.5f, "Clamp to min depth");
        AssertNear(RotateMath.ClampDepthTarget(3.4f, 3.25f, 0.5f, 3.0f), 3.25f, "Pulling back from beyond max should not jump");
        AssertNear(RotateMath.ClampDepthTarget(0.3f, 0.45f, 0.5f, 3.0f), 0.45f, "Pushing out from closer than min should not jump");
    }

    private static void TestDepthEasing()
    {
        float mid = RotateMath.EaseTowardDepth(1.0f, 2.0f, 18f, 0.016f, 0.001f);
        Assert(mid > 1.0f && mid < 2.0f, "Exponential ease should advance smoothly toward target");

        float snapped = RotateMath.EaseTowardDepth(1.9995f, 2.0f, 18f, 0.016f, 0.001f);
        AssertNear(snapped, 2.0f, "Depth within snap epsilon should settle exactly on target");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception("Assertion failed: " + message);
        }
    }

    private static void AssertNear(float actual, float expected, string message)
    {
        if (Math.Abs(actual - expected) > 0.0001f)
        {
            throw new Exception(string.Format("Assertion failed ({0}): expected {1}, got {2}", message, expected, actual));
        }
    }
}
