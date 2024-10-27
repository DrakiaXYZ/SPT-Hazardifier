#if !UNITY_EDITOR
using SPT.Reflection.Patching;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace DrakiaXYZ.Hazardifier.Patches
{
    internal class MineTriggerRaycastPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.FirstMethod(typeof(MineDirectional), method =>
            {
                return (
                    !method.Name.StartsWith("On") && 
                    method.GetParameters().Length == 1 && 
                    method.GetParameters()[0].ParameterType == typeof(Collider));
            });
        }

        [PatchPrefix]
        public static bool PatchPrefix(MineDirectional __instance, Collider other, ref bool __result)
        {
            var minePosition = __instance.gameObject.transform.position + (Vector3.up * 0.2f);
            var colliderPosition = other.bounds.center;

            if (Physics.Linecast(minePosition, colliderPosition, LayerMaskClass.HighPolyWithTerrainMask))
            {
                // Line of sight blocked, skip original
                __result = true;
                return false;
            }

            return true;
        }
    }
}
#endif