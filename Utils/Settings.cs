#if !UNITY_EDITOR
using BepInEx.Configuration;
using System.Collections.Generic;

namespace DrakiaXYZ.Hazardifier.Utils
{
    public class Settings
    {
        private const string GeneralSectionTitle = "1. General";

        public static ConfigFile Config;

        public static ConfigEntry<bool> EnableNewClaymores;
        public static ConfigEntry<int> ClaymoreAmount;
        public static ConfigEntry<bool> ConvertBsgClaymores;
        public static ConfigEntry<bool> MakeBsgClaymoresShootable;
        public static ConfigEntry<float> ClaymoreDisarmTime;
        public static ConfigEntry<bool> AllowArming;
        public static ConfigEntry<bool> DisableLasers;
        public static ConfigEntry<bool> AprilFoolsMode;

        public static List<ConfigEntryBase> ConfigEntries = new List<ConfigEntryBase>();

        public static void Init(ConfigFile Config)
        {
            ConfigEntries.Add(EnableNewClaymores = Config.Bind(
                GeneralSectionTitle,
                "Enable New Claymores",
                true,
                new ConfigDescription(
                    "Whether to enable new claymores on maps",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(ClaymoreAmount = Config.Bind(
                GeneralSectionTitle,
                "Claymore Amount",
                10,
                new ConfigDescription(
                    "The density of new claymores. Not an actual count, but a weight based on map size. Higher = higher performance hit",
                    new AcceptableValueRange<int>(5, 100),
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(ClaymoreDisarmTime = Config.Bind(
                GeneralSectionTitle,
                "Claymore Disarm Timer",
                5f,
                new ConfigDescription(
                    "The amount of time it takes to disarm a claymore",
                    new AcceptableValueRange<float>(0f, 60f),
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(AllowArming = Config.Bind(
                GeneralSectionTitle,
                "Allow Arming Claymores",
                true,
                new ConfigDescription(
                    "Whether to allow re-arming a claymore after you've disarmed it",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(ConvertBsgClaymores = Config.Bind(
                GeneralSectionTitle,
                "Convert BSG Claymores",
                false,
                new ConfigDescription(
                    "Whether to convert BSG claymores to new custom claymore implementation with laser indicators",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(MakeBsgClaymoresShootable = Config.Bind(
                GeneralSectionTitle,
                "Make BSG Claymores Shootable",
                true,
                new ConfigDescription(
                    "Whether to make BSG claymores explode when shot. Ignored if Convert BSG Claymores is enabled",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(DisableLasers = Config.Bind(
                GeneralSectionTitle,
                "Disable Lasers",
                false,
                new ConfigDescription(
                    "Disable the visibility of the trip lasers on claymores, giving no external indication where claymores are or whether they're active",
                    null,
                    new ConfigurationManagerAttributes { })));

            ConfigEntries.Add(AprilFoolsMode = Config.Bind(
                GeneralSectionTitle,
                "April Fools Mode",
                false,
                new ConfigDescription(
                    "Whether to make the random claymores harmless, but still give a concussion/blinding effect",
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