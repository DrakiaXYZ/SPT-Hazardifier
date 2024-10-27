#if !UNITY_EDITOR
using BepInEx.Configuration;
using System.Collections.Generic;

namespace DrakiaXYZ.Hazardifier.Utils
{
    public class Settings
    {
        private const string GeneralSectionTitle = "1. General";

        public static ConfigFile Config;

        public static ConfigEntry<bool> EnableNewMines;
        public static ConfigEntry<int> MineAmount;
        public static ConfigEntry<bool> ConvertBsgMines;
        public static ConfigEntry<bool> MakeBsgMinesShootable;
        public static ConfigEntry<float> MineDisarmTime;
        public static ConfigEntry<bool> AllowArming;
        public static ConfigEntry<bool> DisableLasers;
        public static ConfigEntry<bool> AprilFoolsMode;

        public static List<ConfigEntryBase> ConfigEntries = new List<ConfigEntryBase>();

        public static void Init(ConfigFile Config)
        {
            ConfigEntries.Add(EnableNewMines = Config.Bind(
                GeneralSectionTitle,
                "Enable New Mines",
                true,
                new ConfigDescription(
                    "Whether to enable new mines on maps",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(MineAmount = Config.Bind(
                GeneralSectionTitle,
                "Mine Amount",
                20,
                new ConfigDescription(
                    "The density of new mines. Not an actual count, but a weight based on map size. Higher = higher performance hit",
                    new AcceptableValueRange<int>(10, 100),
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(MineDisarmTime = Config.Bind(
                GeneralSectionTitle,
                "Mine Disarm Timer",
                5f,
                new ConfigDescription(
                    "The amount of time it takes to disarm a mine",
                    new AcceptableValueRange<float>(0f, 60f),
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(AllowArming = Config.Bind(
                GeneralSectionTitle,
                "Allow Arming Mines",
                true,
                new ConfigDescription(
                    "Whether to allow re-arming a mine after you've disarmed it",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(DisableLasers = Config.Bind(
                GeneralSectionTitle,
                "Disable Lasers",
                false,
                new ConfigDescription(
                    "Disable the visibility of the trip lasers on mines, giving no external indication where mines are or whether they're active",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(ConvertBsgMines = Config.Bind(
                GeneralSectionTitle,
                "Convert BSG Mines",
                true,
                new ConfigDescription(
                    "Whether to convert BSG mines to new custom mine implementation with laser indicators",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(MakeBsgMinesShootable = Config.Bind(
                GeneralSectionTitle,
                "Make BSG Mines Shootable",
                true,
                new ConfigDescription(
                    "Whether to make BSG mines explode when shot. Ignored if Convert BSG Mines is enabled",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(AprilFoolsMode = Config.Bind(
                GeneralSectionTitle,
                "April Fools Mode",
                false,
                new ConfigDescription(
                    "Whether to make the random mines harmless, but still give a concussion/blinding effect",
                    null,
                    new ConfigurationManagerAttributes { })));

            RecalcOrder();
        }

        private static void RecalcOrder()
        {
            // Set the Order field for all settings, to avoid unnecessary changes when adding new settings
            int settingOrder = ConfigEntries.Count;
            foreach (var entry in ConfigEntries)
            {
                ConfigurationManagerAttributes attributes = entry.Description.Tags[0] as ConfigurationManagerAttributes;
                if (attributes != null)
                {
                    attributes.Order = settingOrder;
                }

                settingOrder--;
            }
        }

        public enum EAlignment
        {
            Right = 0,
            Left = 1,
        }
    }
}
#endif