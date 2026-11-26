using BepInEx.Configuration;
using UnityEngine;

namespace EasyRotate
{
    public enum HorizontalAxis
    {
        Roll,
        Yaw
    }

    internal static class RotateConfig
    {
        private const string ControlsSection = "Controls";
        private const string InversionSection = "Inversion";

        public static ConfigEntry<bool> EnableMouseRotation { get; private set; }
        public static ConfigEntry<KeyboardShortcut> RotateKey { get; private set; }
        public static ConfigEntry<KeyboardShortcut> SwapAxisKey { get; private set; }
        public static ConfigEntry<HorizontalAxis> DefaultHorizontalAxis { get; private set; }
        public static ConfigEntry<float> RotationSpeed { get; private set; }
        public static ConfigEntry<bool> ScrollPushPull { get; private set; }
        public static ConfigEntry<bool> VanillaScrollRotation { get; private set; }

        public static ConfigEntry<bool> InvertPitch { get; private set; }
        public static ConfigEntry<bool> InvertRoll { get; private set; }
        public static ConfigEntry<bool> InvertYaw { get; private set; }

        public static void Bind(ConfigFile config)
        {
            EnableMouseRotation = config.Bind(
                ControlsSection,
                "Enable Mouse Rotation",
                true,
                new ConfigDescription("Rotate the held item with mouse movement while holding Rotate Key."));

            RotateKey = config.Bind(
                ControlsSection,
                "Rotate Key",
                new KeyboardShortcut(KeyCode.LeftAlt),
                new ConfigDescription("Key held to pause camera look and rotate the held item."));

            SwapAxisKey = config.Bind(
                ControlsSection,
                "Swap Axis Key",
                new KeyboardShortcut(KeyCode.Mouse1),
                new ConfigDescription("Key held together with Rotate Key to switch horizontal movement between Roll and Yaw."));

            DefaultHorizontalAxis = config.Bind(
                ControlsSection,
                "Default Horizontal Axis",
                HorizontalAxis.Roll,
                new ConfigDescription("Axis used for horizontal mouse movement when Swap Axis Key is not held."));

            RotationSpeed = config.Bind(
                ControlsSection,
                "Rotation Speed",
                2.0f,
                new ConfigDescription(
                    "Multiplier for mouse rotation speed.",
                    new AcceptableValueRange<float>(0.5f, 10f)));

            ScrollPushPull = config.Bind(
                ControlsSection,
                "Scroll Push/Pull",
                true,
                new ConfigDescription("Use the scroll wheel to move the held item closer or farther."));

            VanillaScrollRotation = config.Bind(
                ControlsSection,
                "Vanilla Scroll Rotation",
                true,
                new ConfigDescription("Keep Mon Bazou's default 15-degree scroll pitch when Scroll Push/Pull is off."));

            InvertPitch = config.Bind(
                InversionSection,
                "Invert Pitch",
                false,
                new ConfigDescription("Reverse vertical mouse movement."));

            InvertRoll = config.Bind(
                InversionSection,
                "Invert Roll",
                false,
                new ConfigDescription("Reverse horizontal roll."));

            InvertYaw = config.Bind(
                InversionSection,
                "Invert Yaw",
                false,
                new ConfigDescription("Reverse horizontal yaw."));
        }

        public static bool IsHeld(KeyboardShortcut shortcut)
        {
            if (shortcut.MainKey == KeyCode.None || !Input.GetKey(shortcut.MainKey))
            {
                return false;
            }

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
