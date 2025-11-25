using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Handles simulation flow: toggling simulation/info UI and sending slider input
/// to the currently active disease controller.
/// </summary>
public class SimulationManagerNew : MonoBehaviour
{
    [Header("In Simulation")]
    [SerializeField] private Slider overallSlider;
    [SerializeField] private Slider leftEyeSlider;
    [SerializeField] private Slider rightEyeSlider;

    [SerializeField] private SimulationPanelManager simulationPanelManager;

    [Header("Input (A / Primary Button)")]
    [SerializeField] private InputActionReference toggleSimulationAction;

    private GameObject currentDiseaseObject;

    private IDiseaseController currentController;
    private bool isSimulationActive = false;

    private void Awake()
    {
        // Slider callback
        if (overallSlider != null)
            overallSlider.onValueChanged.AddListener(value => OnSliderChanged(value, "Both"));

        if (leftEyeSlider != null)
            leftEyeSlider.onValueChanged.AddListener(value => OnSliderChanged(value, "Left"));

        if (rightEyeSlider != null)
            rightEyeSlider.onValueChanged.AddListener(value => OnSliderChanged(value, "Right"));


        // Input action callback
        if (toggleSimulationAction != null)
        {
            toggleSimulationAction.action.performed += OnToggleSimulationPerformed;
            toggleSimulationAction.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (toggleSimulationAction != null)
            toggleSimulationAction.action.performed -= OnToggleSimulationPerformed;
    }

    /// <summary>
    /// Called when a disease is selected; activates its controller.
    /// </summary>
    public void InjectDisease(DiseaseData diseaseData)
    {
        GameObject diseaseRoot = diseaseData.sourceObject;
        if (!diseaseRoot)
        {
            Debug.LogWarning("SimulationManager: InjectDisease called with null root.");
            return;
        }

        currentDiseaseObject = diseaseRoot;
        currentController = diseaseRoot.GetComponent<IDiseaseController>();
        simulationPanelManager.InjectDiseaseInfo(diseaseData.previewImage, diseaseData.name, diseaseData.preivewDescription);
        if (currentController == null)
        {
            Debug.LogWarning("Selected disease root has no IDiseaseController component.");
            return;
        }

        // Immediately apply slider value to controller
        OnSliderChanged(overallSlider != null ? overallSlider.value : 1f, "Both");
    }

    /// <summary>
    /// Show simulation UI and activate the disease controller.
    /// </summary>
    public void ShowSimulation()
    {
        print("ShowSimulation called");
        if (currentController == null) return;
        
        simulationPanelManager.OpenSimulationPanel();
        currentDiseaseObject.SetActive(true);
        isSimulationActive = true;

        // Push current slider value
        OnSliderChanged(overallSlider != null ? overallSlider.value : 1f, "Both");
    }

    /// <summary>
    /// Hide simulation UI and deactivate the disease controller.
    /// </summary>
    public void HideSimulation()
    {
        if (currentController == null) return;

        simulationPanelManager.CloseSimulationPanel();
        currentDiseaseObject.SetActive(false);
        overallSlider.value = 0f;
        isSimulationActive = false;
    }

    /// <summary>
    /// Toggle simulation on/off using the A/Primary button or UI call.
    /// </summary>
    public void ToggleSimulation()
    {
        if (isSimulationActive) HideSimulation();
        else ShowSimulation();
    }

    private void OnToggleSimulationPerformed(InputAction.CallbackContext _) => HideSimulation();

    /// <summary>
    /// Called by slider; sends value to current disease controller.
    /// </summary>
    private void OnSliderChanged(float value, string eye)
    {
        print("Slider changed: " + value + " | Controller: " + currentController + " | Eye: " + eye);

        currentController?.SetNormalizedValue(value, eye);
    }

    public void ClearDisease()
    {
        currentController = null;
        currentDiseaseObject = null;
    }
}
