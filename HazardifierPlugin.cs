#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using BepInEx;
using EFT;
using System.IO;
using System;
using System.Reflection;
using UnityEngine;
using Comfort.Common;
using System.Threading.Tasks;
using DrakiaXYZ.Hazardifier.Utils;
using DrakiaXYZ.Hazardifier.Patches;

namespace DrakiaXYZ.Hazardifier
{
    [BepInPlugin("xyz.drakia.hazardifier", "DrakiaXYZ-Hazardifier", "1.0.0")]
    public class HazardifierPlugin : BaseUnityPlugin
    {
        private const string LaserBundlePath = "assets/systems/effects/laserbeam/laser.bundle";
        private const string CookieBundlePath = "assets/content/textures/holemanager/glow_particle_bright.bundle";
        public static string PluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private bool _loaded = false;

        private void Awake()
        {
            Settings.Init(Config);

            new HazardifierPatch().Enable();
            new MineTriggerRaycastPatch().Enable();
            new MineInteractivePatch().Enable();
        }

        async private void Update()
        {
            if (!_loaded && Singleton<PoolManager>.Instance != null)
            {
                Logger.LogInfo("PoolManager Available, loading assets");
                _loaded = true;
                await LoadAssets();
            }
        }

        async private Task LoadAssets()
        {
            var bundlePath = Path.Combine(PluginFolder, "minetemplate.bundle");
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                throw new Exception($"Error loading bundle: {bundlePath}");
            }

            HazardifierComponent.MineTemplatePrefab = AssetUtils.LoadAsset<GameObject>(bundle, "Assets/Prefabs/MineTemplate.prefab");
            if (HazardifierComponent.MineTemplatePrefab == null)
            {
                throw new Exception("Error loading MineTemplatePrefab");
            }

            // Load laser assets
            await AssetUtils.Retain(LaserBundlePath);
            await AssetUtils.Retain(CookieBundlePath);
            Material BeamMaterial = AssetUtils.GetAsset<Material>(LaserBundlePath, "LaserBeam");
            Material PointMaterial = AssetUtils.GetAsset<Material>(LaserBundlePath, "LaserPoint");
            Texture2D CookieTexture = AssetUtils.GetAsset<Texture2D>(CookieBundlePath, "glow_particle_bright");

            // Setup the laser beam on the template
            LaserBeam[] lasers = HazardifierComponent.MineTemplatePrefab.GetComponentsInChildren<LaserBeam>();
            foreach (var laser in lasers)
            {
                laser.BeamMaterial = BeamMaterial;
                laser.PointMaterial = PointMaterial;
                laser.Cookie = CookieTexture;
            }
        }
    }

    internal class HazardifierPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchPrefix]
        public static void PatchPrefix()
        {
            HazardifierComponent.Enable();
        }
    }
}
#endif