using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CataractController : MonoBehaviour, IDiseaseController
{
    [Header("Targets")]
    [Tooltip("Assign TWO renderers (e.g., two planes) that use the Cataract material.")]
    public Renderer[] targetRenderers = new Renderer[2];

    [Header("Stages")]
    public List<CataractSettings> stages;

    [Header("Control")]
    [Range(0f, 1f)]
    public float globalSlider = 0f;

    // Material instances for each renderer
    private Material[] materialInstances;

    // Shader Property IDs
    private int blurStrengthID;
    private int tintColorID;
    private int tintStrengthID;

    private int currentStageIndex = 0;

    void Start()
    {
        // Validate renderer array
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Debug.LogError("CataractController: No renderers assigned!", this);
            enabled = false;
            return;
        }

        // Validate stages
        if (stages == null || stages.Count < 2)
        {
            Debug.LogError("CataractController: You must assign at least 2 stages!", this);
            enabled = false;
            return;
        }

        // Create material instances for each renderer
        materialInstances = new Material[targetRenderers.Length];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] == null)
            {
                Debug.LogError($"CataractController: Renderer #{i} is null!", this);
                enabled = false;
                return;
            }

            materialInstances[i] = targetRenderers[i].material;
        }

        // Shader IDs
        blurStrengthID = Shader.PropertyToID("_Blur_Strength");
        tintColorID = Shader.PropertyToID("_Tint_Color");
        tintStrengthID = Shader.PropertyToID("_Tint_Strength");

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

        // Determine stage interpolation
        float totalStageIndex = globalSlider * (stages.Count - 1);
        int stageA_index = Mathf.FloorToInt(totalStageIndex);
        int stageB_index = Mathf.CeilToInt(totalStageIndex);

        CataractSettings stageA = stages[stageA_index];
        CataractSettings stageB = stages[stageB_index];

        float localLerp = totalStageIndex - stageA_index;

        // Interpolated values
        float lerpedBlur = Mathf.Lerp(stageA.Blur_Strength, stageB.Blur_Strength, localLerp);
        Color lerpedColor = Color.Lerp(stageA.Tint_Color, stageB.Tint_Color, localLerp);
        float lerpedTint = Mathf.Lerp(stageA.Tint_Strength, stageB.Tint_Strength, localLerp);

        // Apply to all materials
        foreach (Material mat in materialInstances)
        {
            mat.SetFloat(blurStrengthID, lerpedBlur);
            mat.SetColor(tintColorID, lerpedColor);
            mat.SetFloat(tintStrengthID, lerpedTint);
        }
    }

    private void SnapToStage(int stageIndex)
    {
        if (materialInstances == null || stages == null || stageIndex < 0 || stageIndex >= stages.Count)
            return;

        currentStageIndex = stageIndex;
        CataractSettings s = stages[currentStageIndex];

        // Apply values directly
        foreach (Material mat in materialInstances)
        {
            mat.SetFloat(blurStrengthID, s.Blur_Strength);
            mat.SetColor(tintColorID, s.Tint_Color);
            mat.SetFloat(tintStrengthID, s.Tint_Strength);
        }

        globalSlider = (float)currentStageIndex / (stages.Count - 1);
    }

    // Debug inspector buttons
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
