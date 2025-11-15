using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Include this if you hook it up to a real UI Slider

/// <summary>
/// Controls the Macular Degeneration shader material by interpolating between a list of
/// MacularDegenerationSettings ScriptableObjects.
/// </summary>
public class MacularDegenerationController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The Renderer (e.g., a Plane) that has the Macular Degeneration material on it.")]
    public Renderer targetRenderer;

    [Header("Stages")]
    [Tooltip("Assign your MacularDegenerationSettings ScriptableObjects here, in order. (e.g., Stage 0: Normal, Stage 1: Mild, etc.)")]
    public List<MacularDegenerationSettings> stages;

    [Header("Control")]
    [Tooltip("A master slider value (0 to 1) to interpolate through all stages.")]
    [Range(0f, 1f)]
    public float globalSlider = 0f;

    // The runtime instance of our material
    private Material materialInstance;

    // Shader Property IDs for performance
    private int shapeScaleID;
    private int falloffPowerID;
    private int intensityID;
    private int invertID;
    private int warpStrengthID;
    private int warpScaleID; // Changed from warpSeedID

    // The current discrete stage index, used by the debug buttons
    private int currentStageIndex = 0;

    void Start()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("MacularDegenerationController: Target Renderer is not assigned!", this);
            enabled = false;
            return;
        }

        if (stages == null || stages.Count < 2)
        {
            Debug.LogError("MacularDegenerationController: You must assign at least 2 stages to interpolate between!", this);
            enabled = false;
            return;
        }

        // Create a runtime instance of the material to avoid changing the project asset
        materialInstance = targetRenderer.material;

        // Get the integer IDs for our shader properties
        // IMPORTANT: These strings must EXACTLY match the "Reference" name in your Shader Graph properties
        shapeScaleID = Shader.PropertyToID("_Shape_Scale");
        falloffPowerID = Shader.PropertyToID("_Falloff_Power");
        intensityID = Shader.PropertyToID("_Intensity");
        invertID = Shader.PropertyToID("_Invert");
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
        MacularDegenerationSettings stageA = stages[stageA_index];
        MacularDegenerationSettings stageB = stages[stageB_index];

        // 3. Find the 'local' interpolation value (0-1) between those two stages
        float localLerp = totalStageIndex - stageA_index;

        // 4. Interpolate each property
        Vector2 lerpedShapeScale = Vector2.Lerp(stageA.Shape_Scale, stageB.Shape_Scale, localLerp);
        float lerpedFalloffPower = Mathf.Lerp(stageA.Falloff_Power, stageB.Falloff_Power, localLerp);
        float lerpedIntensity = Mathf.Lerp(stageA.Intensity, stageB.Intensity, localLerp);
        float lerpedWarpStrength = Mathf.Lerp(stageA.Warp_Strength, stageB.Warp_Strength, localLerp);
        float lerpedWarpScale = Mathf.Lerp(stageA.Warp_Scale, stageB.Warp_Scale, localLerp); // Added this line

        // We DO NOT interpolate Invert. We just take it from the current "from" stage.
        bool invert = stageA.Invert;

        // 5. Apply the interpolated values to our material instance
        materialInstance.SetVector(shapeScaleID, lerpedShapeScale);
        materialInstance.SetFloat(falloffPowerID, lerpedFalloffPower);
        materialInstance.SetFloat(intensityID, lerpedIntensity);
        materialInstance.SetFloat(warpStrengthID, lerpedWarpStrength);
        materialInstance.SetFloat(warpScaleID, lerpedWarpScale); // Changed this line

        // Send boolean as a float (0 or 1)
        materialInstance.SetFloat(invertID, invert ? 1f : 0f);
    }

    /// <summary>
    /// A helper function to snap the material to one specific stage's settings.
    /// </summary>
    private void SnapToStage(int stageIndex)
    {
        if (materialInstance == null || stages == null || stageIndex < 0 || stageIndex >= stages.Count)
            return;
        
        currentStageIndex = stageIndex;
        MacularDegenerationSettings settings = stages[currentStageIndex];

        // Apply settings directly
        materialInstance.SetVector(shapeScaleID, settings.Shape_Scale);
        materialInstance.SetFloat(falloffPowerID, settings.Falloff_Power);
        materialInstance.SetFloat(intensityID, settings.Intensity);
        materialInstance.SetFloat(warpStrengthID, settings.Warp_Strength);
        materialInstance.SetFloat(invertID, settings.Invert ? 1f : 0f);
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