using BepInEx;
using HarmonyLib;

namespace EasyRotate
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.skeptic043.monbazou.easyrotate";
        public const string PluginName = "Easy Rotate";
        public const string PluginVersion = "1.0.0";

        private Harmony harmony;
        private RotateController controller;

        private void Awake()
        {
            RotateConfig.Bind(Config);
            controller = new RotateController();
            harmony = new Harmony(PluginGuid);
            GameHooks.Apply(harmony, controller, Logger);
            Logger.LogInfo(PluginName + " " + PluginVersion + " loaded.");
        }

        private void Update()
        {
            if (controller != null)
            {
                controller.Update();
            }
        }

        private void OnDestroy()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }
    }
}
