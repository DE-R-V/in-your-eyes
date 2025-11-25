using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiabeticRetinopathyController : MonoBehaviour, IDiseaseController
{
    [Header("Targets")]
    [Tooltip("Assign the Left Eye Mesh Renderer here.")]
    public Renderer leftEyeRenderer;
    [Tooltip("Assign the Right Eye Mesh Renderer here.")]
    public Renderer rightEyeRenderer;

    [Header("Stages")]
    public List<DiabeticRetinopathySettings> stages;

    [Header("Control")]
    [Range(0f, 1f)]
    [Tooltip("Master slider. Changing this updates both eyes.")]
    public float globalSlider = 0f;

    [Range(0f, 1f)]
    [Tooltip("Controls only the Left Eye renderer.")]
    public float leftEyeSlider = 0f;

    [Range(0f, 1f)]
    [Tooltip("Controls only the Right Eye renderer.")]
    public float rightEyeSlider = 0f;

    // Runtime material instances
    private Material leftMatInstance;
    private Material rightMatInstance;

    // Shader Property IDs
    private int spotAmountID;
    private int spotSizeID;
    private int darknessID;
    private int sharpnessRatioID;
    private int blurrySharpnessID;
    private int warpStrengthID;
    private int warpScaleID;

    // Floater Property IDs
    private int floaterOpacityID;
    private int floaterDensityID;
    private int floaterSpeedID;
    private int floaterWidthID;

    // Internal tracking for Inspector changes
    private float _lastGlobal;
    private float _lastLeft;
    private float _lastRight;

    private int currentStageIndex = 0;

    void Start()
    {
        InitializeRenderers();
        InitializeShaderIDs();
        
        // Force initial update
        ApplySettingsToMaterial(leftMatInstance, leftEyeSlider);
        ApplySettingsToMaterial(rightMatInstance, rightEyeSlider);
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
            Debug.LogError("DiabeticRetinopathyController: Need at least 2 stages!", this);
            enabled = false;
        }
    }

    void InitializeShaderIDs()
    {
        spotAmountID = Shader.PropertyToID("_Spot_Amount");
        spotSizeID = Shader.PropertyToID("_Spot_Size");
        darknessID = Shader.PropertyToID("_Darkness");
        sharpnessRatioID = Shader.PropertyToID("_Sharpness_Ratio");
        blurrySharpnessID = Shader.PropertyToID("_Blurry_Sharpness");
        warpStrengthID = Shader.PropertyToID("_Warp_Strength");
        warpScaleID = Shader.PropertyToID("_Warp_Scale");

        floaterOpacityID = Shader.PropertyToID("_Floater_Opacity");
        floaterDensityID = Shader.PropertyToID("_Floater_Density");
        floaterSpeedID = Shader.PropertyToID("_Floater_Speed");
        floaterWidthID = Shader.PropertyToID("_FLoater_Width"); // Note the capital L based on your shader
    }

    void Update()
    {
        // In runtime, we assume values might change via script or UI
        // If Global changed via script, sync others
        if (!Mathf.Approximately(globalSlider, _lastGlobal))
        {
            SyncGlobalToEyes();
        }

        ApplySettingsToMaterial(leftMatInstance, leftEyeSlider);
        ApplySettingsToMaterial(rightMatInstance, rightEyeSlider);

        UpdateLastValues();
    }

    void OnValidate()
    {
        // This handles Inspector interaction
        if (stages == null || stages.Count < 2) return;

        // If user moved Global Slider in Inspector, update both eyes
        if (!Mathf.Approximately(globalSlider, _lastGlobal))
        {
            leftEyeSlider = globalSlider;
            rightEyeSlider = globalSlider;
        }

        // We assume materials might not be created in Editor if not playing, 
        // but we can try to grab them from the renderer if accessible.
        // (MaterialPropertyBlocks are better for Editor usage, but we stick to Materials for simplicity here)
        
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
        _lastLeft = leftEyeSlider;
        _lastRight = rightEyeSlider;
    }

    /// <summary>
    /// Core logic: Calculates settings for a specific progress value (0-1) and applies to a material.
    /// </summary>
    private void ApplySettingsToMaterial(Material mat, float progress)
    {
        if (mat == null || stages == null || stages.Count < 2) return;

        progress = Mathf.Clamp01(progress);

        float totalStageIndex = progress * (stages.Count - 1);
        int stageA_index = Mathf.FloorToInt(totalStageIndex);
        int stageB_index = Mathf.CeilToInt(totalStageIndex);

        DiabeticRetinopathySettings stageA = stages[stageA_index];
        DiabeticRetinopathySettings stageB = stages[stageB_index];

        float t = totalStageIndex - stageA_index;

        // Interpolate Properties
        float spotAmount = Mathf.Lerp(stageA.Spot_Amount, stageB.Spot_Amount, t);
        float spotSize = Mathf.Lerp(stageA.Spot_Size, stageB.Spot_Size, t);
        float darkness = Mathf.Lerp(stageA.Darkness, stageB.Darkness, t);
        float sharpness = Mathf.Lerp(stageA.Sharpness_Ratio, stageB.Sharpness_Ratio, t);
        float blurrySharpness = Mathf.Lerp(stageA.Blurry_Sharpness, stageB.Blurry_Sharpness, t);
        float warpStrength = Mathf.Lerp(stageA.Warp_Strength, stageB.Warp_Strength, t);
        float warpScale = Mathf.Lerp(stageA.Warp_Scale, stageB.Warp_Scale, t);

        float fOpacity = Mathf.Lerp(stageA.Floater_Opacity, stageB.Floater_Opacity, t);
        float fDensity = Mathf.Lerp(stageA.Floater_Density, stageB.Floater_Density, t);
        float fSpeed = Mathf.Lerp(stageA.Floater_Speed, stageB.Floater_Speed, t);
        float fWidth = Mathf.Lerp(stageA.Floater_Width, stageB.Floater_Width, t);

        // Apply
        mat.SetFloat(spotAmountID, spotAmount);
        mat.SetFloat(spotSizeID, spotSize);
        mat.SetFloat(darknessID, darkness);
        mat.SetFloat(sharpnessRatioID, sharpness);
        mat.SetFloat(blurrySharpnessID, blurrySharpness);
        mat.SetFloat(warpStrengthID, warpStrength);
        mat.SetFloat(warpScaleID, warpScale);

        mat.SetFloat(floaterOpacityID, fOpacity);
        mat.SetFloat(floaterDensityID, fDensity);
        mat.SetFloat(floaterSpeedID, fSpeed);
        mat.SetFloat(floaterWidthID, fWidth);
    }

    // --- Public API for UI / External Scripts ---

    public void SetNormalizedValue(float value, string eye = "Both")
    {
        value = Mathf.Clamp01(value);

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

        // Update happens in Update loop automatically, or you can force it here:
        if (eye == "Left" || eye == "Both") ApplySettingsToMaterial(leftMatInstance, leftEyeSlider);
        if (eye == "Right" || eye == "Both") ApplySettingsToMaterial(rightMatInstance, rightEyeSlider);
        
        UpdateLastValues();
    }

    // --- Debug / Context Menu Helper ---

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

    [ContextMenu("Go to Next Stage")]
    public void GoToNextStage()
    {
        if (stages == null || stages.Count == 0) return;
        int nextStage = (currentStageIndex + 1) % stages.Count;
        SnapToStage(nextStage);
    }

    [ContextMenu("Go to Previous Stage")]
    public void GoToPreviousStage()
    {
        if (stages == null || stages.Count == 0) return;
        int prevStage = currentStageIndex - 1;
        if (prevStage < 0) prevStage = stages.Count - 1;
        SnapToStage(prevStage);
    }
}