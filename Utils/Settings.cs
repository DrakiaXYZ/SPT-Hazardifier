#if !UNITY_EDITOR
using BepInEx.Configuration;
using System.Collections.Generic;

namespace DrakiaXYZ.Hazardifier.Utils
{
    public class Settings
    {
        private const string GeneralSectionTitle = "1. General";
        private const string MapSettings = "2. Map Claymore Settings";

        public static ConfigFile Config;

        public static ConfigEntry<bool> EnableNewClaymores;
        public static ConfigEntry<bool> ConvertBsgClaymores;
        public static ConfigEntry<bool> MakeBsgClaymoresShootable;
        public static ConfigEntry<float> ClaymoreDisarmTime;
        public static ConfigEntry<bool> AllowArming;
        public static ConfigEntry<bool> DisableLasers;
        public static ConfigEntry<bool> AprilFoolsMode;

        public static ConfigEntry<int> GroundZeroClaymoreAmount;
        public static ConfigEntry<int> StreetsClaymoreAmount;
        public static ConfigEntry<int> InterchangeClaymoreAmount;
        public static ConfigEntry<int> CustomsClaymoreAmount;
        public static ConfigEntry<int> FactoryClaymoreAmount;
        public static ConfigEntry<int> WoodsClaymoreAmount;
        public static ConfigEntry<int> ReserveClaymoreAmount;
        public static ConfigEntry<int> LighthouseClaymoreAmount;
        public static ConfigEntry<int> ShorelineClaymoreAmount;
        public static ConfigEntry<int> LabsClaymoreAmount;

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

            ConfigEntries.Add(CustomsClaymoreAmount = Config.Bind(
                MapSettings,
                "Customs Claymore Count",
                75,
                new ConfigDescription(
                    "The amount of new claymores on Customs. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(FactoryClaymoreAmount = Config.Bind(
                MapSettings,
                "Factory Claymore Count",
                50,
                new ConfigDescription(
                    "The amount of new claymores on Factory. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(GroundZeroClaymoreAmount = Config.Bind(
                MapSettings,
                "Ground Zero Claymore Count",
                75,
                new ConfigDescription(
                    "The amount of new claymores on Ground Zero. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(InterchangeClaymoreAmount = Config.Bind(
                MapSettings,
                "Interchange Claymore Count",
                100,
                new ConfigDescription(
                    "The amount of new claymores on Interchange. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(LabsClaymoreAmount = Config.Bind(
                MapSettings,
                "Labs Claymore Count",
                75,
                new ConfigDescription(
                    "The amount of new claymores on Labs. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(LighthouseClaymoreAmount = Config.Bind(
                MapSettings,
                "Lighthouse Claymore Count",
                100,
                new ConfigDescription(
                    "The amount of new claymores on Lighthouse. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(ReserveClaymoreAmount = Config.Bind(
                MapSettings,
                "Reserve Claymore Count",
                75,
                new ConfigDescription(
                    "The amount of new claymores on Reserve. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(ShorelineClaymoreAmount = Config.Bind(
                MapSettings,
                "Shoreline Claymore Count",
                100,
                new ConfigDescription(
                    "The amount of new claymores on Shoreline. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(StreetsClaymoreAmount = Config.Bind(
                MapSettings,
                "Streets Claymore Count",
                100,
                new ConfigDescription(
                    "The amount of new claymores on Streets. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
                    new ConfigurationManagerAttributes { })));
            ConfigEntries.Add(WoodsClaymoreAmount = Config.Bind(
                MapSettings,
                "Woods Claymore Count",
                100,
                new ConfigDescription(
                    "The amount of new claymores on Woods. Higher = higher performance hit",
                    new AcceptableValueRange<int>(0, 500),
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