using BepInEx;
using HarmonyLib;

namespace DeftHands
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.skeptic043.monbazou.defthands";
        public const string PluginName = "Deft Hands";
        public const string PluginVersion = "1.0.1";

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
