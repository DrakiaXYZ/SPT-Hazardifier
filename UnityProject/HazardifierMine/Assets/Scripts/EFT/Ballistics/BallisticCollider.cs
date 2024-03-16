using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace EFT.Ballistics
{
    public class BallisticCollider : MonoBehaviour
    {
        public int NetId;

        [SerializeField]
        private MaterialType _typeOfMaterial;

        public float PenetrationLevel;

        [Range(0f, 1f)]
        public float PenetrationChance;

        [Range(0f, 1f)]
        public float RicochetChance;

        [Range(0f, 1f)]
        public float FragmentationChance;

        [Range(0f, 1f)]
        public float TrajectoryDeviationChance;

        [Range(0f, 1f)]
        public float TrajectoryDeviation;
    }
}
