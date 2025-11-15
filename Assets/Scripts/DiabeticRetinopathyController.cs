using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Include this if you hook it up to a real UI Slider

/// <summary>
/// Controls the Diabetic Retinopathy shader material by interpolating between a list of
/// DiabeticRetinopathySettings ScriptableObjects.
/// </summary>
public class DiabeticRetinopathyController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The Renderer (e.g., a Plane) that has the Diabetic Retinopathy material on it.")]
    public Renderer targetRenderer;

    [Header("Stages")]
    [Tooltip("Assign your DiabeticRetinopathySettings ScriptableObjects here, in order. (e.g., Stage 0: Normal, Stage 1: Mild, etc.)")]
    public List<DiabeticRetinopathySettings> stages;

    [Header("Control")]
    [Tooltip("A master slider value (0 to 1) to interpolate through all stages.")]
    [Range(0f, 1f)]
    public float globalSlider = 0f;

    // The runtime instance of our material
    private Material materialInstance;

    // Shader Property IDs for performance
    private int spotAmountID;
    private int spotSizeID;
    private int darknessID;
    private int sharpnessRatioID;
    private int blurrySharpnessID;
    private int warpStrengthID;
    private int warpScaleID; // Changed from warpSeedID

    // The current discrete stage index, used by the debug buttons
    private int currentStageIndex = 0;

    void Start()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("DiabeticRetinopathyController: Target Renderer is not assigned!", this);
            enabled = false;
            return;
        }

        if (stages == null || stages.Count < 2)
        {
            Debug.LogError("DiabeticRetinopathyController: You must assign at least 2 stages to interpolate between!", this);
            enabled = false;
            return;
        }

        // Create a runtime instance of the material to avoid changing the project asset
        materialInstance = targetRenderer.material;

        // Get the integer IDs for our shader properties
        // IMPORTANT: These strings must EXACTLY match the "Reference" name in your Shader Graph properties
        spotAmountID = Shader.PropertyToID("_Spot_Amount");
        spotSizeID = Shader.PropertyToID("_Spot_Size");
        darknessID = Shader.PropertyToID("_Darkness");
        sharpnessRatioID = Shader.PropertyToID("_Sharpness_Ratio");
        blurrySharpnessID = Shader.PropertyToID("_Blurry_Sharpness");
        warpStrengthID = Shader.PropertyToID("_Warp_Strength");
        warpScaleID = Shader.PropertyToID("_Warp_Scale"); // Changed from _Warp_Seed

        // Set the initial state based on the slider
        UpdateMaterialProperties();
    }

    void Update()
    {
        // Continuously update the material properties based on the slider
        UpdateMaterialProperties();
    }

    /// <summary>
    /// This function is called in the editor when a value is changed in the Inspector.
    /// This allows you to see changes live by dragging the 'globalSlider'.
    /// </summary>
    void OnValidate()
    {
        if (materialInstance != null && stages != null && stages.Count >= 2)
        {
            UpdateMaterialProperties();
        }
    }

    /// <summary>
    /// Calculates the correct interpolation between stages based on the globalSlider
    /// and applies the final values to the material.
    /// </summary>
    private void UpdateMaterialProperties()
    {
        if (materialInstance == null || stages == null || stages.Count < 2)
            return;

        // 1. Find which two stages we are between
        float totalStageIndex = globalSlider * (stages.Count - 1);
        int stageA_index = Mathf.FloorToInt(totalStageIndex);
        int stageB_index = Mathf.CeilToInt(totalStageIndex);

        // 2. Get the two stages
        DiabeticRetinopathySettings stageA = stages[stageA_index];
        DiabeticRetinopathySettings stageB = stages[stageB_index];

        // 3. Find the 'local' interpolation value (0-1) between those two stages
        float localLerp = totalStageIndex - stageA_index;

        // 4. Interpolate each property
        float lerpedSpotAmount = Mathf.Lerp(stageA.Spot_Amount, stageB.Spot_Amount, localLerp);
        float lerpedSpotSize = Mathf.Lerp(stageA.Spot_Size, stageB.Spot_Size, localLerp);
        float lerpedDarkness = Mathf.Lerp(stageA.Darkness, stageB.Darkness, localLerp);
        float lerpedSharpnessRatio = Mathf.Lerp(stageA.Sharpness_Ratio, stageB.Sharpness_Ratio, localLerp);
        float lerpedBlurrySharpness = Mathf.Lerp(stageA.Blurry_Sharpness, stageB.Blurry_Sharpness, localLerp);
        float lerpedWarpStrength = Mathf.Lerp(stageA.Warp_Strength, stageB.Warp_Strength, localLerp);
        float lerpedWarpScale = Mathf.Lerp(stageA.Warp_Scale, stageB.Warp_Scale, localLerp); // Added this line

        // 5. Apply the interpolated values to our material instance
        materialInstance.SetFloat(spotAmountID, lerpedSpotAmount);
        materialInstance.SetFloat(spotSizeID, lerpedSpotSize);
        materialInstance.SetFloat(darknessID, lerpedDarkness);
        materialInstance.SetFloat(sharpnessRatioID, lerpedSharpnessRatio);
        materialInstance.SetFloat(blurrySharpnessID, lerpedBlurrySharpness);
        materialInstance.SetFloat(warpStrengthID, lerpedWarpStrength);
        materialInstance.SetFloat(warpScaleID, lerpedWarpScale); // Changed this line
    }

    /// <summary>
    /// A helper function to snap the material to one specific stage's settings.
    /// </summary>
    private void SnapToStage(int stageIndex)
    {
        if (materialInstance == null || stages == null || stageIndex < 0 || stageIndex >= stages.Count)
            return;
        
        currentStageIndex = stageIndex;
        DiabeticRetinopathySettings settings = stages[currentStageIndex];

        // Apply settings directly
        materialInstance.SetFloat(spotAmountID, settings.Spot_Amount);
        materialInstance.SetFloat(spotSizeID, settings.Spot_Size);
        materialInstance.SetFloat(darknessID, settings.Darkness);
        materialInstance.SetFloat(sharpnessRatioID, settings.Sharpness_Ratio);
        materialInstance.SetFloat(blurrySharpnessID, settings.Blurry_Sharpness);
        materialInstance.SetFloat(warpStrengthID, settings.Warp_Strength);
        materialInstance.SetFloat(warpScaleID, settings.Warp_Scale); // Changed this line

        // Update the global slider to match this discrete stage
        globalSlider = (float)currentStageIndex / (float)(stages.Count - 1);
    }

    // --- Debug Functions (for Inspector) ---

    [ContextMenu("Go to Next Stage")]
    public void GoToNextStage()
    {
        if (stages == null || stages.Count == 0) return;
        
        int nextStage = (currentStageIndex + 1) % stages.Count; // Wraps around
        SnapToStage(nextStage);
    }

    [ContextMenu("Go to Previous Stage")]
    public void GoToPreviousStage()
    {
        if (stages == null || stages.Count == 0) return;

        int prevStage = currentStageIndex - 1;
        if (prevStage < 0)
        {
            prevStage = stages.Count - 1; // Wraps around
        }
        SnapToStage(prevStage);
    }
}