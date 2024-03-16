#if !UNITY_EDITOR
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.Ballistics;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using DrakiaXYZ.Hazardifier.Utils;

namespace DrakiaXYZ.Hazardifier
{
    internal class HazardifierComponent : MonoBehaviour
    {
        protected ManualLogSource Logger { get; private set; }
        public static GameObject MineTemplatePrefab;

        private GameWorld GameWorld;
        private static FieldInfo _mineDataField;

        private HazardifierComponent()
        {
            if (Logger == null)
            {
                Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(HazardifierComponent));
            }

            _mineDataField = AccessTools.Field(typeof(MineDirectional), "_mineData");
        }

        public void Awake()
        {
            this.GameWorld = Singleton<GameWorld>.Instance;

            // Replace BSG mines with our implementation
            var baseMines = MineDirectional.Mines.ToArray();
            foreach (var mine in baseMines)
            {
                if (Settings.ConvertBsgMines.Value)
                {
                    ReplaceMine(mine);
                }
                else if (Settings.MakeBsgMinesShootable.Value)
                {
                    AddMineBallisticCollider(mine);
                }
            }

            if (Settings.EnableNewMines.Value)
            {
                // Add our own custom mines
                var botZones = LocationScene.GetAllObjects<BotZone>();
                List<CustomNavigationPoint> allAmbushPoints = new List<CustomNavigationPoint>();
                foreach (var botZone in botZones)
                {
                    allAmbushPoints.AddRange(botZone.AmbushPoints);
                }

                // Add ambush points to a random selection of 5-15% of ambush points
                int mineAmount = Settings.MineAmount.Value / 2;
                int rangeMin = mineAmount - 5;
                int rangeMax = mineAmount + 5;
                var mineCount = Math.Ceiling((UnityEngine.Random.Range(rangeMin, rangeMax) / 100f) * allAmbushPoints.Count);
                for (int i = 0; i < mineCount; i++)
                {
                    var index = UnityEngine.Random.Range(0, allAmbushPoints.Count);
                    var ambushPoint = allAmbushPoints[index];
                    var rotation = Quaternion.LookRotation(ambushPoint.ToWallVector, Vector3.up) * Quaternion.Euler(0, 180, 0);
                    AddMine(ambushPoint.Position, rotation);

                    allAmbushPoints.RemoveAt(index);
                }

                Logger.LogDebug($"Created {mineCount} mines out of a potential {mineCount + allAmbushPoints.Count} points");
            }
        }

        private MineDirectional AddMine(Vector3 position, Quaternion rotation)
        {
            // Don't add the mine if it's within 10m of the player position
            var distance = (this.GameWorld.MainPlayer.Position - position).sqrMagnitude;
            if (distance < 100)
            {
                return null;
            }

            GameObject mineObject = Instantiate(MineTemplatePrefab);
            mineObject.transform.SetParent(this.gameObject.transform);
            mineObject.transform.rotation = rotation;
            mineObject.transform.position = position;

            MineDirectional mine = mineObject.GetComponent<MineDirectional>();
            mine.SetArmed(true);

            BallisticCollider collider = mineObject.GetComponentInChildren<BallisticCollider>();
            collider.OnHitAction += OnMineShot;

            return mine;
        }

        private void ReplaceMine(MineDirectional originalMine)
        {
            var mineSettings = (MineDirectional.MineSettings)_mineDataField.GetValue(originalMine);
            var newMine = AddMine(originalMine.transform.position, originalMine.transform.rotation);
            if (newMine != null)
            {
                _mineDataField.SetValue(newMine, mineSettings);
                Destroy(originalMine.gameObject);
            }
        }

        private void AddMineBallisticCollider(MineDirectional mine)
        {
            var collider = mine.gameObject.transform.Find("Collider");
            if (collider != null)
            {
                var ballisticCollider = collider.gameObject.AddComponent<BallisticCollider>();
                ballisticCollider.OnHitAction += OnMineShot;
                ballisticCollider.TypeOfMaterial = MaterialType.MetalThick;
                ballisticCollider.PenetrationLevel = 100;
                ballisticCollider.PenetrationChance = 1;
            }
            else
            {
                Logger.LogError($"Unable to find collider for {mine}");
            }
        }

        private void OnMineShot(DamageInfo damage)
        {
            var mine = damage.HittedBallisticCollider.GetComponentInParent<MineDirectional>();
            mine.Explosion();
        }

        public static void Enable()
        {
            if (Singleton<IBotGame>.Instantiated)
            {
                var gameWorld = Singleton<GameWorld>.Instance;
                HazardifierComponent component = gameWorld.gameObject.GetComponent<HazardifierComponent>();
                if (component == null)
                {
                    gameWorld.gameObject.AddComponent<HazardifierComponent>();
                }
            }
        }
    }
}
#endif