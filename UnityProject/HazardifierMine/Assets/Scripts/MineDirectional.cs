using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using EFT;

public class MineDirectional : MonoBehaviour
{
    [SerializeField]
    private MineDirectional.MineSettings _mineData;

    [Serializable]
    public struct MineSettings
    {
        [SerializeField]
        private Vector3 _blindness;

        [SerializeField]
        private Vector3 _contusion;

        [SerializeField]
        private Vector3 _armorDistanceDistanceDamage;

        [SerializeField]
        private float _minExplosionDistance;

        [SerializeField]
        private float _maxExplosionDistance;

        [SerializeField]
        private int _fragmentsCount;

        [SerializeField]
        private float _strength;

        [SerializeField]
        private string _tag;

        [SerializeField]
        private float _armorDamage;

        [SerializeField]
        private float _staminaBurnRate;

        [SerializeField]
        private float _penetrationPower;

        [SerializeField]
        private string _fragmentType;

        [SerializeField]
        private string _fxName;

        [SerializeField]
        private WildSpawnType _ignoreRole;

        [SerializeField]
        private float _directionalDamageAngle;

        [SerializeField]
        private float _directionalDamageMultiplier;
    }
}

