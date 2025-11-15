using UnityEngine;

[CreateAssetMenu(fileName = "CataractSettings", menuName = "Vision Effects/Cataract Settings")]
public class CataractSettings : ScriptableObject
{
    [Header("Blur")]

    [Tooltip("The screen-space offset for the blur. Small values (0.001 - 0.01) are best.")]
    [Range(0, 0.02f)]
    public float Blur_Strength = 0.005f;

    [Header("Tint")]

    [Tooltip("The color of the tint overlay (non-HDR).")]
    // Defaulting to new Color(1, 1, 0.784f) which is R:255, G:255, B:200
    public Color Tint_Color = new Color(1, 1, 0.784f, 1);

    [Tooltip("How strongly the tint is applied over the blurred image.")]
    [Range(0, 1)]
    public float Tint_Strength = 0.5f;
}