using System;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float RayStart = 0.1f;

    public float MaxDistance = 100f;

    public bool UsePointLight = true;

    public Material BeamMaterial;

    public Material PointMaterial;

    public LayerMask Mask;

    public float BeamSize;

    public float PointSizeClose;

    public float PointSizeFar;

    public float SurfaceOffsetForLight;

    public float LightRange;

    public float LightIntensity;

    [SerializeField]
    private float IntensityFactor = 1f;

    public Texture Cookie;

    public Vector2 AngleCloseFar = new Vector2(4f, 200f);
}
