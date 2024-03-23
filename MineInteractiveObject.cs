using UnityEngine;
using EFT.Interactive;
using System.Reflection;
using HarmonyLib;
using System.Linq;
using DrakiaXYZ.Hazardifier.Utils;

namespace DrakiaXYZ.Hazardifier
{
    public class MineInteractiveObject
#if !UNITY_EDITOR
        : InteractableObject
#else
        : MonoBehaviour
#endif
    {
#if !UNITY_EDITOR
        private MineDirectional mineDirectional;

        // We'll get the first boolean field, this should hopefully be the isArmed field
        private static FieldInfo _isDisarmedField = AccessTools.GetDeclaredFields(typeof(MineDirectional)).FirstOrDefault(x => x.FieldType == typeof(bool));

        public void Awake()
        {
            mineDirectional = GetComponentInParent<MineDirectional>();
            this.InteractionDirection = Vector3.zero;
        }

        public bool IsArmed()
        {
            return !(bool)_isDisarmedField.GetValue(mineDirectional);
        }

        public void DisarmMine()
        {
            mineDirectional.SetArmed(false);

            var lasers = mineDirectional.GetComponentsInChildren<LaserBeam>();
            foreach (var laser in lasers)
            {
                laser.enabled = false;
            }
        }

        public void ArmMine()
        {
            mineDirectional.SetArmed(true);

            var lasers = mineDirectional.GetComponentsInChildren<LaserBeam>();
            foreach (var laser in lasers)
            {
                laser.enabled = true;
            }
        }
#else

        // For the Unity Editor, we just need a stub class to get access to the properties
        public enum ESpecificInteractionContext
        {
            None,
            LighthouseKeeperDoor
        }

        public ESpecificInteractionContext specificInteractionContext;
        public float InteractionDot;
        public Vector3 InteractionDirection;
#endif
    }
}
