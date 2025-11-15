using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class CataractController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The Renderer (e.g., a Plane or Sphere) that has the Cataract material on it.")]
    public Renderer targetRenderer;

    [Header("Stages")]
    [Tooltip("Assign your CataractSettings ScriptableObjects here, in order. (e.g., Stage 0: Normal, Stage 1: Mild, etc.)")]
    public List<CataractSettings> stages;

    [Header("Control")]
    [Tooltip("A master slider value (0 to 1) to interpolate through all stages.")]
    [Range(0f, 1f)]
    public float globalSlider = 0f;

    // The runtime instance of our material
    private Material materialInstance;

    // Shader Property IDs for performance
    private int blurStrengthID;
    private int tintColorID;
    private int tintStrengthID;

    // The current discrete stage index, used by the debug buttons
    private int currentStageIndex = 0;

    void Start()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("CataractController: Target Renderer is not assigned!", this);
            enabled = false;
            return;
        }

        if (stages == null || stages.Count < 2)
        {
            Debug.LogError("CataractController: You must assign at least 2 stages to interpolate between!", this);
            enabled = false;
            return;
        }

        // Create a runtime instance of the material to avoid changing the project asset
        materialInstance = targetRenderer.material;

        // Get the integer IDs for our shader properties
        blurStrengthID = Shader.PropertyToID("_Blur_Strength");
        tintColorID = Shader.PropertyToID("_Tint_Color");
        tintStrengthID = Shader.PropertyToID("_Tint_Strength");

        // Set the initial state based on the slider
        UpdateMaterialProperties();
    }

    void Update()
    {
        // Continuously update the material properties based on the slider
        // In a real application, you might only call this when the slider value changes
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
        // 'totalStageIndex' is a float value, e.g., 2.5 (halfway between stage 2 and 3)
        float totalStageIndex = globalSlider * (stages.Count - 1);
        int stageA_index = Mathf.FloorToInt(totalStageIndex);
        int stageB_index = Mathf.CeilToInt(totalStageIndex);

        // 2. Get the two stages
        CataractSettings stageA = stages[stageA_index];
        CataractSettings stageB = stages[stageB_index];

        // 3. Find the 'local' interpolation value (0-1) between those two stages
        // If totalStageIndex is 2.5, localLerp is 0.5
        float localLerp = totalStageIndex - stageA_index;

        // 4. Interpolate each property
        float lerpedBlur = Mathf.Lerp(stageA.Blur_Strength, stageB.Blur_Strength, localLerp);
        Color lerpedColor = Color.Lerp(stageA.Tint_Color, stageB.Tint_Color, localLerp);
        float lerpedTint = Mathf.Lerp(stageA.Tint_Strength, stageB.Tint_Strength, localLerp);

        // 5. Apply the interpolated values to our material instance
        materialInstance.SetFloat(blurStrengthID, lerpedBlur);
        materialInstance.SetColor(tintColorID, lerpedColor);
        materialInstance.SetFloat(tintStrengthID, lerpedTint);
    }

    /// <summary>
    /// A helper function to snap the material to one specific stage's settings.
    /// </summary>
    private void SnapToStage(int stageIndex)
    {
        if (materialInstance == null || stages == null || stageIndex < 0 || stageIndex >= stages.Count)
            return;
        
        currentStageIndex = stageIndex;
        CataractSettings settings = stages[currentStageIndex];

        // Apply settings directly
        materialInstance.SetFloat(blurStrengthID, settings.Blur_Strength);
        materialInstance.SetColor(tintColorID, settings.Tint_Color);
        materialInstance.SetFloat(tintStrengthID, settings.Tint_Strength);

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