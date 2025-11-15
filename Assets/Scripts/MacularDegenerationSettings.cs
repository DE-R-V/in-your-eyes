using UnityEngine;

[CreateAssetMenu(fileName = "MacularDegenerationSettings", menuName = "Vision Effects/Macular Degeneration Settings")]
public class MacularDegenerationSettings : ScriptableObject
{
    [Header("Shape Control")]

    [Tooltip("X and Y radii of the central shape (1,1 is a circle)")]
    public Vector2 Shape_Scale = new Vector2(1, 1);

    [Tooltip("Hardness of the shape's edge. Low=blurry, High=sharp.")]
    [Min(0.1f)]
    public float Falloff_Power = 2f;

    [Tooltip("Overall opacity of the black shape.")]
    [Range(0, 10)]
    public float Intensity = 1f;

    [Tooltip("Toggles between a black center (default) and a transparent center (vignette).")]
    public bool Invert = false;

    [Header("Warp Distortion")]

    [Tooltip("How much to distort the shape's edges. 0 = no distortion.")]
    [Range(0, 1)] // Kept low, as high values are extreme
    public float Warp_Strength = 0f;

    [Tooltip("WarpScale")]
    [Range(0.1f, 200f)]
    public float Warp_Scale = 0f;
}