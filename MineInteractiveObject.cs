using UnityEngine;
using EFT.Interactive;

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
        public void Awake()
        {
            mineDirectional = GetComponentInParent<MineDirectional>();
            this.InteractionDirection = Vector3.zero;
        }

        public bool IsArmed()
        {
            // The actual armed state is harder to determine, as it's a private bool
            // so we'll just use the lasers as an indicator
            var lasers = mineDirectional.GetComponentsInChildren<LaserBeam>();
            foreach (var laser in lasers)
            {
                if (laser.enabled)
                {
                    return true;
                }
            }

            return false;
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
