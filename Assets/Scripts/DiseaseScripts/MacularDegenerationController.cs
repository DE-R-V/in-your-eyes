using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the Macular Degeneration shader by interpolating between multiple
/// MacularDegenerationSettings ScriptableObjects.
/// </summary>
public class MacularDegenerationController : MonoBehaviour, IDiseaseController
{
    [Header("Targets")]
    [Tooltip("One or more Renderers that use the Macular Degeneration shader.")]
    public Renderer[] targetRenderers;

    [Header("Stages")]
    [Tooltip("Assign ScriptableObject stages in order (Stage0 → StageN).")]
    public List<MacularDegenerationSettings> stages;

    [Header("Control")]
    [Tooltip("Master slider (0–1) controlling interpolation across stages.")]
    [Range(0f, 1f)]
    public float globalSlider = 0f;

    // Runtime instances for each renderer
    private Material[] materialInstances;

    // Shader property IDs
    private int shapeScaleID;
    private int falloffPowerID;
    private int intensityID;
    private int invertID;
    private int warpStrengthID;
    private int warpScaleID;

    // Current discrete stage index
    private int currentStageIndex = 0;

    void Start()
    {
        // --- Validate renderers ---
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Debug.LogError("MacularDegenerationController: No renderers assigned!", this);
            enabled = false;
            return;
        }

        // --- Validate stages ---
        if (stages == null || stages.Count < 2)
        {
            Debug.LogError("MacularDegenerationController: At least 2 stage assets are required!", this);
            enabled = false;
            return;
        }

        // --- Create material instances ---
        materialInstances = new Material[targetRenderers.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
                materialInstances[i] = targetRenderers[i].material; // creates an instance
        }

        CacheShaderIDs();
        UpdateMaterialProperties();
    }

    void Update()
    {
        // Continuously update based on slider
        UpdateMaterialProperties();
    }

    private void OnValidate()
    {
        if (materialInstances != null && stages != null && stages.Count >= 2)
            UpdateMaterialProperties();
    }

    // Cache shader property IDs (faster than using strings every frame)
    private void CacheShaderIDs()
    {
        shapeScaleID = Shader.PropertyToID("_Shape_Scale");
        falloffPowerID = Shader.PropertyToID("_Falloff_Power");
        intensityID = Shader.PropertyToID("_Intensity");
        invertID = Shader.PropertyToID("_Invert");
        warpStrengthID = Shader.PropertyToID("_Warp_Strength");
        warpScaleID = Shader.PropertyToID("_Warp_Scale");
    }

    /// <summary>
    /// Interpolates between stages based on globalSlider and applies to all materials.
    /// </summary>
    private void UpdateMaterialProperties(string eye = "Both")
    {
        if (materialInstances == null || materialInstances.Length == 0)
            return;

        // Determine fractional stage index
        float totalStageIndex = globalSlider * (stages.Count - 1);

        int stageA_index = Mathf.FloorToInt(totalStageIndex);
        int stageB_index = Mathf.CeilToInt(totalStageIndex);
        float localLerp = totalStageIndex - stageA_index;

        // Clamp to avoid errors
        stageA_index = Mathf.Clamp(stageA_index, 0, stages.Count - 1);
        stageB_index = Mathf.Clamp(stageB_index, 0, stages.Count - 1);

        MacularDegenerationSettings A = stages[stageA_index];
        MacularDegenerationSettings B = stages[stageB_index];

        // Interpolate values
        Vector2 lerpedShapeScale = Vector2.Lerp(A.Shape_Scale, B.Shape_Scale, localLerp);
        float lerpedFalloffPower = Mathf.Lerp(A.Falloff_Power, B.Falloff_Power, localLerp);
        float lerpedIntensity = Mathf.Lerp(A.Intensity, B.Intensity, localLerp);
        float lerpedWarpStrength = Mathf.Lerp(A.Warp_Strength, B.Warp_Strength, localLerp);
        float lerpedWarpScale = Mathf.Lerp(A.Warp_Scale, B.Warp_Scale, localLerp);
        bool invert = A.Invert;

        if(eye == "Both")
        {
            // Apply to all materials
            foreach (var mat in materialInstances)
            {
                if (mat == null) continue;

                mat.SetVector(shapeScaleID, lerpedShapeScale);
                mat.SetFloat(falloffPowerID, lerpedFalloffPower);
                mat.SetFloat(intensityID, lerpedIntensity);
                mat.SetFloat(warpStrengthID, lerpedWarpStrength);
                mat.SetFloat(warpScaleID, lerpedWarpScale);
                mat.SetFloat(invertID, invert ? 1f : 0f);
            }
        }
        else if (eye == "Left" && materialInstances.Length > 0)
        {
            print("Applying to left eye");
            // Only first material
            var mat = materialInstances[0];
            if (mat != null)
            {
                mat.SetVector(shapeScaleID, lerpedShapeScale);
                mat.SetFloat(falloffPowerID, lerpedFalloffPower);
                mat.SetFloat(intensityID, lerpedIntensity);
                mat.SetFloat(warpStrengthID, lerpedWarpStrength);
                mat.SetFloat(warpScaleID, lerpedWarpScale);
                mat.SetFloat(invertID, invert ? 1f : 0f);
            }
        } 
        else if (eye == "Right" && materialInstances.Length > 1)
        {
            
            // Only second material
            var mat = materialInstances[1];
            if (mat != null)
            {
                mat.SetVector(shapeScaleID, lerpedShapeScale);
                mat.SetFloat(falloffPowerID, lerpedFalloffPower);
                mat.SetFloat(intensityID, lerpedIntensity);
                mat.SetFloat(warpStrengthID, lerpedWarpStrength);
                mat.SetFloat(warpScaleID, lerpedWarpScale);
                mat.SetFloat(invertID, invert ? 1f : 0f);
            }
        }
    }

    /// <summary>
    /// Snap the material instantly to a specific stage.
    /// </summary>
    private void SnapToStage(int stageIndex)
    {
        if (materialInstances == null || stageIndex < 0 || stageIndex >= stages.Count)
            return;

        currentStageIndex = stageIndex;
        MacularDegenerationSettings S = stages[currentStageIndex];

        foreach (var mat in materialInstances)
        {
            if (mat == null) continue;

            mat.SetVector(shapeScaleID, S.Shape_Scale);
            mat.SetFloat(falloffPowerID, S.Falloff_Power);
            mat.SetFloat(intensityID, S.Intensity);
            mat.SetFloat(warpStrengthID, S.Warp_Strength);
            mat.SetFloat(warpScaleID, S.Warp_Scale);
            mat.SetFloat(invertID, S.Invert ? 1f : 0f);
        }

        // Update the slider accordingly
        globalSlider = (float)currentStageIndex / (float)(stages.Count - 1);
    }

    // Debug helpers
    [ContextMenu("Next Stage")]
    public void GoToNextStage()
    {
        if (stages == null || stages.Count == 0) return;

        int next = (currentStageIndex + 1) % stages.Count;
        SnapToStage(next);
    }

    [ContextMenu("Previous Stage")]
    public void GoToPreviousStage()
    {
        if (stages == null || stages.Count == 0) return;

        int prev = currentStageIndex - 1;
        if (prev < 0) prev = stages.Count - 1;

        SnapToStage(prev);
    }

    public void SetNormalizedValue(float value, string eye = "Both")
    {
        print("value received: " + value + " for eye: " + eye);
        globalSlider = Mathf.Clamp01(value);
        UpdateMaterialProperties(eye);
    }
}
