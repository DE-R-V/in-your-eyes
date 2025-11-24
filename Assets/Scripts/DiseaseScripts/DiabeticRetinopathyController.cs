using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiabeticRetinopathyController : MonoBehaviour, IDiseaseController
{
    [Header("Targets")]
    [Tooltip("Assign renderers (e.g., planes) that use the Diabetic Retinopathy material.")]
    public Renderer[] targetRenderers = new Renderer[2];

    [Header("Stages")]
    public List<DiabeticRetinopathySettings> stages;

    [Header("Control")]
    [Range(0f, 1f)]
    public float globalSlider = 0f;

    // Runtime material instances (one per renderer)
    private Material[] materialInstances;

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

    private int currentStageIndex = 0;

    void Start()
    {
        // Validate
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Debug.LogError("DiabeticRetinopathyController: No renderers assigned!", this);
            enabled = false;
            return;
        }

        if (stages == null || stages.Count < 2)
        {
            Debug.LogError("DiabeticRetinopathyController: Need at least 2 stages!", this);
            enabled = false;
            return;
        }

        // Create material instances for EACH renderer
        materialInstances = new Material[targetRenderers.Length];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] == null)
            {
                Debug.LogError($"Renderer #{i} is null!", this);
                enabled = false;
                return;
            }

            materialInstances[i] = targetRenderers[i].material; // runtime instance
        }

        // Shader IDs
        spotAmountID = Shader.PropertyToID("_Spot_Amount");
        spotSizeID = Shader.PropertyToID("_Spot_Size");
        darknessID = Shader.PropertyToID("_Darkness");
        sharpnessRatioID = Shader.PropertyToID("_Sharpness_Ratio");
        blurrySharpnessID = Shader.PropertyToID("_Blurry_Sharpness");
        warpStrengthID = Shader.PropertyToID("_Warp_Strength");
        warpScaleID = Shader.PropertyToID("_Warp_Scale");

        // Floater IDs
        floaterOpacityID = Shader.PropertyToID("_Floater_Opacity");
        floaterDensityID = Shader.PropertyToID("_Floater_Density");
        floaterSpeedID = Shader.PropertyToID("_Floater_Speed");
        floaterWidthID = Shader.PropertyToID("_Floater_Width");

        UpdateMaterialProperties();
    }

    void Update()
    {
        UpdateMaterialProperties();
    }

    void OnValidate()
    {
        if (materialInstances != null && stages != null && stages.Count >= 2)
            UpdateMaterialProperties();
    }

    private void UpdateMaterialProperties()
    {
        if (materialInstances == null || stages == null || stages.Count < 2)
            return;

        // Stage interpolation logic
        float totalStageIndex = globalSlider * (stages.Count - 1);
        int stageA_index = Mathf.FloorToInt(totalStageIndex);
        int stageB_index = Mathf.CeilToInt(totalStageIndex);

        DiabeticRetinopathySettings stageA = stages[stageA_index];
        DiabeticRetinopathySettings stageB = stages[stageB_index];

        float localLerp = totalStageIndex - stageA_index;

        // Interpolate Standard Properties
        float lerpedSpotAmount = Mathf.Lerp(stageA.Spot_Amount, stageB.Spot_Amount, localLerp);
        float lerpedSpotSize = Mathf.Lerp(stageA.Spot_Size, stageB.Spot_Size, localLerp);
        float lerpedDarkness = Mathf.Lerp(stageA.Darkness, stageB.Darkness, localLerp);
        float lerpedSharpnessRatio = Mathf.Lerp(stageA.Sharpness_Ratio, stageB.Sharpness_Ratio, localLerp);
        float lerpedBlurrySharpness = Mathf.Lerp(stageA.Blurry_Sharpness, stageB.Blurry_Sharpness, localLerp);
        float lerpedWarpStrength = Mathf.Lerp(stageA.Warp_Strength, stageB.Warp_Strength, localLerp);
        float lerpedWarpScale = Mathf.Lerp(stageA.Warp_Scale, stageB.Warp_Scale, localLerp);

        // Interpolate Floater Properties
        float lerpedFloaterOpacity = Mathf.Lerp(stageA.Floater_Opacity, stageB.Floater_Opacity, localLerp);
        float lerpedFloaterDensity = Mathf.Lerp(stageA.Floater_Density, stageB.Floater_Density, localLerp);
        float lerpedFloaterSpeed = Mathf.Lerp(stageA.Floater_Speed, stageB.Floater_Speed, localLerp);
        float lerpedFloaterWidth = Mathf.Lerp(stageA.Floater_Width, stageB.Floater_Width, localLerp);

        // APPLY TO BOTH MATERIAL INSTANCES
        foreach (Material mat in materialInstances)
        {
            mat.SetFloat(spotAmountID, lerpedSpotAmount);
            mat.SetFloat(spotSizeID, lerpedSpotSize);
            mat.SetFloat(darknessID, lerpedDarkness);
            mat.SetFloat(sharpnessRatioID, lerpedSharpnessRatio);
            mat.SetFloat(blurrySharpnessID, lerpedBlurrySharpness);
            mat.SetFloat(warpStrengthID, lerpedWarpStrength);
            mat.SetFloat(warpScaleID, lerpedWarpScale);

            // Set Floater Properties
            mat.SetFloat(floaterOpacityID, lerpedFloaterOpacity);
            mat.SetFloat(floaterDensityID, lerpedFloaterDensity);
            mat.SetFloat(floaterSpeedID, lerpedFloaterSpeed);
            mat.SetFloat(floaterWidthID, lerpedFloaterWidth);
        }
    }

    private void SnapToStage(int stageIndex)
    {
        if (materialInstances == null || stages == null || stageIndex < 0 || stageIndex >= stages.Count)
            return;

        currentStageIndex = stageIndex;
        DiabeticRetinopathySettings s = stages[currentStageIndex];

        foreach (Material mat in materialInstances)
        {
            mat.SetFloat(spotAmountID, s.Spot_Amount);
            mat.SetFloat(spotSizeID, s.Spot_Size);
            mat.SetFloat(darknessID, s.Darkness);
            mat.SetFloat(sharpnessRatioID, s.Sharpness_Ratio);
            mat.SetFloat(blurrySharpnessID, s.Blurry_Sharpness);
            mat.SetFloat(warpStrengthID, s.Warp_Strength);
            mat.SetFloat(warpScaleID, s.Warp_Scale);

            // Set Floater Properties
            mat.SetFloat(floaterOpacityID, s.Floater_Opacity);
            mat.SetFloat(floaterDensityID, s.Floater_Density);
            mat.SetFloat(floaterSpeedID, s.Floater_Speed);
            mat.SetFloat(floaterWidthID, s.Floater_Width);
        }

        globalSlider = (float)currentStageIndex / (stages.Count - 1);
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

    public void SetNormalizedValue(float value)
    {
        globalSlider = Mathf.Clamp01(value);
        UpdateMaterialProperties();
    }
}