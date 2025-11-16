using UnityEngine;

public interface IDiseaseController
{
    /// <summary>
    /// Sets a normalized value (0–1) to control the disease effect.
    /// Each controller interprets this value according to its ScriptableObjects / shader logic.
    /// </summary>
    /// <param name="value">Normalized slider value</param>
    void SetNormalizedValue(float value);
}
