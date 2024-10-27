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
using SPT.Reflection.Utils;
using System.Linq;
using System.Collections;

namespace DrakiaXYZ.Hazardifier
{
    internal class HazardifierComponent : MonoBehaviour
    {
        protected ManualLogSource Logger { get; private set; }
        public static GameObject MineTemplatePrefab;

        private GameWorld GameWorld;

        private static FieldInfo _mineDataField;
        private static FieldInfo _aiCoverPointsField;
        private static FieldInfo _groupPointWallDirectionField;
        private static FieldInfo _groupPointCoverTypeField;
        private static FieldInfo _groupPointNeighborTypeField;
        private static FieldInfo _localGameBotsControllerField;

        private static PropertyInfo _botsControllerCoverProperty;
        private static PropertyInfo _groupPointPositionProperty;

        static HazardifierComponent()
        {
            Type aiCoversType = PatchConstants.EftTypes.SingleOrDefault(x => x.Name == "AICoversData");
            Type groupPointType = PatchConstants.EftTypes.SingleOrDefault(x => x.Name == "GroupPoint");
            if (aiCoversType != null && groupPointType != null)
            {
                Type localGameType = PatchConstants.EftTypes.SingleOrDefault(x => x.Name == "LocalGame");
                _localGameBotsControllerField = AccessTools.Field(localGameType, "botsController_0");
                _botsControllerCoverProperty = AccessTools.Property(typeof(BotsController), "CoversData");
                _aiCoverPointsField = AccessTools.Field(aiCoversType, "Points");
                _groupPointPositionProperty = AccessTools.Property(groupPointType, "Position");
                _groupPointWallDirectionField = AccessTools.Field(groupPointType, "WallDirection");
                _groupPointCoverTypeField = AccessTools.Field(groupPointType, "CoverType");
                _groupPointNeighborTypeField = AccessTools.Field(groupPointType, "PointWithNeighborType");
            }

            _mineDataField = AccessTools.Field(typeof(MineDirectional), "_mineData");
        }

        private HazardifierComponent()
        {
            if (Logger == null)
            {
                Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(HazardifierComponent));
            }
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
                List<MinePoint> minePoints = GetPositions();

                // Add ambush points to a random selection of 5-15% of ambush points
                int mineAmount = Settings.MineAmount.Value / 2;
                int rangeMin = mineAmount - 5;
                int rangeMax = mineAmount + 5;
                var mineCount = Math.Ceiling((UnityEngine.Random.Range(rangeMin, rangeMax) / 100f) * minePoints.Count);
                for (int i = 0; i < mineCount; i++)
                {
                    var index = UnityEngine.Random.Range(0, minePoints.Count);
                    var minePoint = minePoints[index];
                    var rotation = Quaternion.LookRotation(minePoint.ToWallVector, Vector3.up) * Quaternion.Euler(0, 180, 0);
                    AddMine(minePoint.Position, rotation);

                    minePoints.RemoveAt(index);
                }

                Logger.LogDebug($"Created {mineCount} mines out of a potential {mineCount + minePoints.Count} points");
            }
        }

        private List<MinePoint> GetPositions()
        {
            var botGame = Singleton<IBotGame>.Instance;
            var botsController = _localGameBotsControllerField.GetValue(botGame);
            var aiCoversData = _botsControllerCoverProperty.GetValue(botsController);
            var pointsList = (IList)_aiCoverPointsField.GetValue(aiCoversData);

            var minePoints = new List<MinePoint>();
            foreach (var point in pointsList)
            {
                if ((CoverType)_groupPointCoverTypeField.GetValue(point) == CoverType.Wall &&
                    (PointWithNeighborType)_groupPointNeighborTypeField.GetValue(point) == PointWithNeighborType.ambush)
                {
                    minePoints.Add(new MinePoint()
                    {
                        Position = (Vector3)_groupPointPositionProperty.GetValue(point),
                        ToWallVector = (Vector3)_groupPointWallDirectionField.GetValue(point)
                    });
                }
            }

            return minePoints;
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