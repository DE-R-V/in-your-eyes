using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Handles simulation flow: toggling simulation/info UI and sending slider input
/// to the currently active disease controller.
/// </summary>
public class SimulationManagerNew : MonoBehaviour
{
    [Header("UI / Flow")]
    [SerializeField] private DiseaseUIManager uiManager;   // optional: toggle main UI
    [SerializeField] private GameObject container;         // parent for simulation visuals
    [SerializeField] private GameObject simulationUIRoot;  // UI canvas for slider during simulation
    [SerializeField] private GameObject infoPanel;         // info panel outside simulation
    [SerializeField] private Slider slider;                // slider for controlling disease effect

    [Header("Input (A / Primary Button)")]
    [SerializeField] private InputActionReference toggleSimulationAction;
    private GameObject currentDiseaseObject;

    private IDiseaseController currentController;
    private bool isSimulationActive = false;

    private void Awake()
    {
        // Slider callback
        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderChanged);

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
    public void InjectDisease(GameObject diseaseRoot)
    {
        if (!diseaseRoot)
        {
            Debug.LogWarning("SimulationManager: InjectDisease called with null root.");
            return;
        }

        currentDiseaseObject = diseaseRoot;
        currentController = diseaseRoot.GetComponent<IDiseaseController>();
        if (currentController == null)
        {
            Debug.LogWarning("Selected disease root has no IDiseaseController component.");
            return;
        }

        // Immediately apply slider value to controller
        OnSliderChanged(slider != null ? slider.value : 1f);
    }

    /// <summary>
    /// Show simulation UI and activate the disease controller.
    /// </summary>
    public void ShowSimulation()
    {
        if (currentController == null) return;

        uiManager?.ToggleMainUI(false);
        simulationUIRoot?.SetActive(true);
        infoPanel?.SetActive(false);

        if (currentController == null) return;
        currentDiseaseObject.SetActive(true);
        container?.SetActive(true);
        isSimulationActive = true;

        // Push current slider value
        OnSliderChanged(slider != null ? slider.value : 1f);
    }

    /// <summary>
    /// Hide simulation UI and deactivate the disease controller.
    /// </summary>
    public void HideSimulation()
    {
        if (currentController == null) return;

        uiManager?.ToggleMainUI(true);
        simulationUIRoot?.SetActive(false);
        infoPanel?.SetActive(true);

        if (currentController == null) return;
        currentDiseaseObject.SetActive(false);
        container?.SetActive(false);
        slider.value = 0f;
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

    private void OnToggleSimulationPerformed(InputAction.CallbackContext _) => ToggleSimulation();

    /// <summary>
    /// Called by slider; sends value to current disease controller.
    /// </summary>
    private void OnSliderChanged(float value)
    {
        print("Slider changed: " + value + " | Controller: " + currentController);
        currentController?.SetNormalizedValue(value);
    }

    public void ClearDisease()
    {
        currentController = null;
        currentDiseaseObject = null;
    }
}
