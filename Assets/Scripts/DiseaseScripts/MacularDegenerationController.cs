using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the Macular Degeneration shader by interpolating between multiple
/// MacularDegenerationSettings ScriptableObjects with independent eye control.
/// </summary>
public class MacularDegenerationController : MonoBehaviour, IDiseaseController
{
    [Header("Targets")]
    [Tooltip("Assign the Left Eye Mesh Renderer here.")]
    public Renderer leftEyeRenderer;
    [Tooltip("Assign the Right Eye Mesh Renderer here.")]
    public Renderer rightEyeRenderer;

    [Header("Stages")]
    [Tooltip("Assign ScriptableObject stages in order (Stage0 → StageN).")]
    public List<MacularDegenerationSettings> stages;

    [Header("Control")]
    [Tooltip("Master slider (0–1). Changing this updates both eyes.")]
    [Range(0f, 1f)]
    public float globalSlider = 0f;

    [Tooltip("Controls only the Left Eye renderer.")]
    [Range(0f, 1f)]
    public float leftEyeSlider = 0f;

    [Tooltip("Controls only the Right Eye renderer.")]
    [Range(0f, 1f)]
    public float rightEyeSlider = 0f;

    // Runtime instances
    private Material leftMatInstance;
    private Material rightMatInstance;

    // Shader property IDs
    private int shapeScaleID;
    private int falloffPowerID;
    private int intensityID;
    private int invertID;
    private int warpStrengthID;
    private int warpScaleID;

    // Internal tracking for Inspector changes
    private float _lastGlobal;
    private int currentStageIndex = 0;

    void Start()
    {
        InitializeRenderers();
        InitializeShaderIDs();

        // Force initial update
        ApplySettingsToMaterial(leftMatInstance, leftEyeSlider);
        ApplySettingsToMaterial(rightMatInstance, rightEyeSlider);
        
        UpdateLastValues();
    }

    void InitializeRenderers()
    {
        if (leftEyeRenderer != null)
            leftMatInstance = leftEyeRenderer.material;
        else
            Debug.LogWarning("Left Eye Renderer not assigned!", this);

        if (rightEyeRenderer != null)
            rightMatInstance = rightEyeRenderer.material;
        else
            Debug.LogWarning("Right Eye Renderer not assigned!", this);

        if (stages == null || stages.Count < 2)
        {
            Debug.LogError("MacularDegenerationController: At least 2 stage assets are required!", this);
            enabled = false;
        }
    }

    void InitializeShaderIDs()
    {
        shapeScaleID = Shader.PropertyToID("_Shape_Scale");
        falloffPowerID = Shader.PropertyToID("_Falloff_Power");
        intensityID = Shader.PropertyToID("_Intensity");
        invertID = Shader.PropertyToID("_Invert");
        warpStrengthID = Shader.PropertyToID("_Warp_Strength");
        warpScaleID = Shader.PropertyToID("_Warp_Scale");
    }

    void Update()
    {
        // In runtime, check if Global changed via script or UI
        if (!Mathf.Approximately(globalSlider, _lastGlobal))
        {
            SyncGlobalToEyes();
        }

        ApplySettingsToMaterial(leftMatInstance, leftEyeSlider);
        ApplySettingsToMaterial(rightMatInstance, rightEyeSlider);

        UpdateLastValues();
    }

    private void OnValidate()
    {
        if (stages == null || stages.Count < 2) return;

        // Sync Global -> Eyes if Global moved in Inspector
        if (!Mathf.Approximately(globalSlider, _lastGlobal))
        {
            leftEyeSlider = globalSlider;
            rightEyeSlider = globalSlider;
        }
        
        UpdateLastValues();
    }

    private void SyncGlobalToEyes()
    {
        leftEyeSlider = globalSlider;
        rightEyeSlider = globalSlider;
    }

    private void UpdateLastValues()
    {
        _lastGlobal = globalSlider;
    }

    /// <summary>
    /// Core logic: Calculates settings for a specific progress value (0-1) and applies to a material.
    /// </summary>
    private void ApplySettingsToMaterial(Material mat, float progress)
    {
        if (mat == null || stages == null || stages.Count < 2) return;

        progress = Mathf.Clamp01(progress);

        // Determine fractional stage index
        float totalStageIndex = progress * (stages.Count - 1);
        int stageA_index = Mathf.FloorToInt(totalStageIndex);
        int stageB_index = Mathf.CeilToInt(totalStageIndex);

        // Clamp indices
        stageA_index = Mathf.Clamp(stageA_index, 0, stages.Count - 1);
        stageB_index = Mathf.Clamp(stageB_index, 0, stages.Count - 1);

        MacularDegenerationSettings A = stages[stageA_index];
        MacularDegenerationSettings B = stages[stageB_index];

        float t = totalStageIndex - stageA_index;

        // Interpolate values
        Vector2 lerpedShapeScale = Vector2.Lerp(A.Shape_Scale, B.Shape_Scale, t);
        float lerpedFalloffPower = Mathf.Lerp(A.Falloff_Power, B.Falloff_Power, t);
        float lerpedIntensity = Mathf.Lerp(A.Intensity, B.Intensity, t);
        float lerpedWarpStrength = Mathf.Lerp(A.Warp_Strength, B.Warp_Strength, t);
        float lerpedWarpScale = Mathf.Lerp(A.Warp_Scale, B.Warp_Scale, t);
        
        // Boolean is usually taken from the current "floor" stage
        bool invert = A.Invert;

        // Apply to material
        mat.SetVector(shapeScaleID, lerpedShapeScale);
        mat.SetFloat(falloffPowerID, lerpedFalloffPower);
        mat.SetFloat(intensityID, lerpedIntensity);
        mat.SetFloat(warpStrengthID, lerpedWarpStrength);
        mat.SetFloat(warpScaleID, lerpedWarpScale);
        mat.SetFloat(invertID, invert ? 1f : 0f);
    }

    // --- Public API for UI / External Scripts ---

    public void SetNormalizedValue(float value, string eye = "Both")
    {
        value = Mathf.Clamp01(value);
        // Debug.Log($"Macular SetNormalizedValue: {value} for {eye}");

        if (eye == "Both")
        {
            globalSlider = value;
            leftEyeSlider = value;
            rightEyeSlider = value;
        }
        else if (eye == "Left")
        {
            leftEyeSlider = value;
        }
        else if (eye == "Right")
        {
            rightEyeSlider = value;
        }

        // Apply immediately
        if (eye == "Left" || eye == "Both") ApplySettingsToMaterial(leftMatInstance, leftEyeSlider);
        if (eye == "Right" || eye == "Both") ApplySettingsToMaterial(rightMatInstance, rightEyeSlider);
        
        UpdateLastValues();
    }

    // --- Debug Helpers ---

    private void SnapToStage(int stageIndex)
    {
        if (stages == null || stageIndex < 0 || stageIndex >= stages.Count) return;

        currentStageIndex = stageIndex;
        float val = (float)currentStageIndex / (stages.Count - 1);

        // Debug buttons snap everything (Global behavior)
        globalSlider = val;
        leftEyeSlider = val;
        rightEyeSlider = val;
    }

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
}