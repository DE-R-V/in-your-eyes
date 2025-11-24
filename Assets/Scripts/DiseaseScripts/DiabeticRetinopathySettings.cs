using UnityEngine;

[CreateAssetMenu(fileName = "DiabeticRetinopathySettings", menuName = "Vision Effects/Diabetic Retinopathy Settings")]
public class DiabeticRetinopathySettings : ScriptableObject
{
    [Header("Spot Generation")]

    [Tooltip("The frequency/tiling of the spot pattern. Higher = more spots.")]
    [Range(0, 10)]
    public float Spot_Amount = 3f;

    [Tooltip("Controls the base threshold for spots to appear. Higher = larger spots.")]
    [Range(0.01f, 1f)]
    public float Spot_Size = 0.2f;

    [Tooltip("The overall opacity of the spots.")]
    [Range(0, 1)]
    public float Darkness = 1f;

    [Header("Spot Appearance")]

    [Tooltip("Blends between blurry and sharp spots. 0 = all blurry, 1 = all sharp.")]
    [Range(0, 1)]
    public float Sharpness_Ratio = 0.5f;

    [Tooltip("Controls the edge falloff of the blurry spots. Higher = sharper edge.")]
    [Range(0.1f, 50f)]
    public float Blurry_Sharpness = 1f;

    [Header("Warp Distortion")]

    [Tooltip("How much to distort the spot pattern.")]
    [Range(0, 0.2f)]
    public float Warp_Strength = 0.05f;

    [Tooltip("WarpScale")]
    [Range(0.1f, 50f)]
    public float Warp_Scale = 0f;

    [Header("Floaters")]

    [Tooltip("Opacity of the floaters (separate from spots).")]
    [Range(0, 1)]
    public float Floater_Opacity = 0.5f;

    [Tooltip("Size/Amount of floaters. Higher value = smaller and more floaters.")]
    [Range(5, 200)]
    public float Floater_Density = 15f;

    [Tooltip("How fast the floaters drift.")]
    [Range(0, 1)]
    public float Floater_Speed = 0.1f;

    [Tooltip("Thickness of the floaters.")]
    [Range(0.1f, 50f)]
    public float Floater_Width = 5f;
}