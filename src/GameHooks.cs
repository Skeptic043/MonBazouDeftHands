using BepInEx.Logging;
using CMF;
using HarmonyLib;
using System.Reflection;

namespace EasyRotate
{
    internal static class GameHooks
    {
        private static RotateController controller;

        public static void Apply(Harmony harmony, RotateController activeController, ManualLogSource log)
        {
            controller = activeController;

            PatchOrWarn(
                harmony,
                AccessTools.Method(typeof(PlayerInteractable), "Start"),
                null,
                new HarmonyMethod(typeof(GameHooks), "AfterPlayerInteractableStart"),
                "PlayerInteractable.Start",
                log);

            PatchOrWarn(
                harmony,
                AccessTools.Method(typeof(CameraController), "HandleCameraRotation"),
                new HarmonyMethod(typeof(GameHooks), "BeforeHandleCameraRotation"),
                null,
                "CMF.CameraController.HandleCameraRotation",
                log);

            PatchOrWarn(
                harmony,
                AccessTools.Method(typeof(PlayerInteractable), "MouseRightClickDown"),
                new HarmonyMethod(typeof(GameHooks), "BeforeMouseRightClickDown"),
                null,
                "PlayerInteractable.MouseRightClickDown",
                log);

            PatchOrWarn(
                harmony,
                AccessTools.Method(typeof(PlayerInteractable), "MouseRightClickUp"),
                new HarmonyMethod(typeof(GameHooks), "BeforeMouseRightClickUp"),
                null,
                "PlayerInteractable.MouseRightClickUp",
                log);

            PatchOrWarn(
                harmony,
                AccessTools.Method(typeof(PlayerInteractable), "ScrollThisObject", new[] { typeof(float) }),
                new HarmonyMethod(typeof(GameHooks), "BeforeScrollThisObject"),
                null,
                "PlayerInteractable.ScrollThisObject",
                log);
        }

        private static void PatchOrWarn(
            Harmony harmony,
            MethodBase target,
            HarmonyMethod prefix,
            HarmonyMethod postfix,
            string label,
            ManualLogSource log)
        {
            if (target == null)
            {
                log.LogWarning("Could not find target method " + label + "; related behavior will be inactive.");
                return;
            }

            harmony.Patch(target, prefix: prefix, postfix: postfix);
        }

        private static void AfterPlayerInteractableStart(PlayerInteractable __instance)
        {
            if (controller != null)
            {
                controller.BindPlayer(__instance);
            }
        }

        private static bool BeforeHandleCameraRotation()
        {
            return controller == null || !controller.IsRotating;
        }

        private static bool BeforeMouseRightClickDown()
        {
            return controller == null || !controller.ShouldBlockRightClickDown();
        }

        private static bool BeforeMouseRightClickUp()
        {
            return controller == null || !controller.ShouldBlockRightClickUp();
        }

        private static bool BeforeScrollThisObject(PlayerInteractable __instance, float value)
        {
            if (controller == null || !controller.CanControlHeldItem(__instance))
            {
                return true;
            }

            if (RotateConfig.ScrollPushPull.Value)
            {
                controller.AdjustHoldDepth(value);
                return false;
            }

            if (controller.IsRotating)
            {
                return false;
            }

            return RotateConfig.VanillaScrollRotation.Value;
        }
    }
}
