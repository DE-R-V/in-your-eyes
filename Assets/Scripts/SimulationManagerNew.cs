using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SimulationManagerNew : MonoBehaviour
{
    [Header("UI / Flow")]
    [SerializeField] private DiseaseUIManager uiManager;   // optional: calls ToggleMainUI(bool)
    [SerializeField] private GameObject container;         // parent for your sim visuals (e.g., plane)
    [SerializeField] private GameObject simulationUIRoot;  // canvas/parent with the slider (shown IN sim)
    [SerializeField] private GameObject infoPanel;         // info panel (shown OUTSIDE sim)
    [SerializeField] private Slider slider;                // slider uses absolute values (e.g., 1..5)

    [Header("Input (A / primary button toggles)")]
    [SerializeField] private InputActionReference toggleSimulationAction; // bind to RightHand/primaryButton

    private GameObject currentDiseaseObject;
    private bool isSimulationActive = false;

    [Serializable]
    public class DiseaseEntry
    {
        [Header("Roots / Renderers")]
        public GameObject root;                   // camera-attached disease root
        public Renderer[] renderersOverride;      // optional: restrict to specific renderers

        [Header("Shader Property Mapping")]
        public string propertyName = "_Intensity"; // Shader Graph 'Reference' name
        public float minValue = 1f;                // absolute min (e.g., 1)
        public float maxValue = 5f;                // absolute max (e.g., 5)
        public bool invert = false;                // flips response inside [min..max]

        // runtime cache
        [NonSerialized] public int propertyId = -1;
        [NonSerialized] public Renderer[] renderers;
        [NonSerialized] public MaterialPropertyBlock[] mpbs;
    }

    [SerializeField] private DiseaseEntry[] diseases;

    private void Awake()
    {
        // cache renderers/MPBs once; do NOT auto SetActive() anything here
        if (diseases != null)
        {
            foreach (var d in diseases)
            {
                if (d == null) continue;

                d.propertyId = string.IsNullOrEmpty(d.propertyName) ? -1 : Shader.PropertyToID(d.propertyName);

                if (d.renderersOverride != null && d.renderersOverride.Length > 0)
                    d.renderers = d.renderersOverride.Where(r => r != null).ToArray();
                else if (d.root)
                    d.renderers = d.root.GetComponentsInChildren<Renderer>(true);
                else
                    d.renderers = Array.Empty<Renderer>();

                d.mpbs = new MaterialPropertyBlock[d.renderers.Length];
                for (int i = 0; i < d.mpbs.Length; i++) d.mpbs[i] = new MaterialPropertyBlock();
            }
        }

        if (slider) slider.onValueChanged.AddListener(OnSliderChanged);

        // NEW: keep the input action enabled for the whole app lifetime
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

    /// <summary>UI calls this when a disease was chosen; pass the camera-attached root (or a child).</summary>
    public void InjectDisease(GameObject diseaseRoot)
    {
        if (!container)
        {
            Debug.LogError("SimulationManager: 'container' is not assigned.");
            return;
        }

        currentDiseaseObject = diseaseRoot;

        // apply current slider value immediately so visuals match before showing
        if (slider) ApplyValueToActive(slider.value);
    }

    public void ShowSimulation()
    {
        if (!currentDiseaseObject) return;

        if (uiManager) uiManager.ToggleMainUI(false);
        if (simulationUIRoot) simulationUIRoot.SetActive(true); // show slider UI
        if (infoPanel) infoPanel.SetActive(false);       // hide info during sim

        currentDiseaseObject.SetActive(true);
        container.SetActive(true);
        isSimulationActive = true;

        if (slider) ApplyValueToActive(slider.value);
    }

    public void HideSimulation()
    {
        if (!currentDiseaseObject) return;

        if (uiManager) uiManager.ToggleMainUI(true);
        if (simulationUIRoot) simulationUIRoot.SetActive(false); // hide slider UI
        if (infoPanel) infoPanel.SetActive(true);         // show info again

        currentDiseaseObject.SetActive(false);
        container.SetActive(false);
        isSimulationActive = false;
    }

    /// <summary>A / primary button toggles between Simulation and Info.</summary>
    public void ToggleSimulation()
    {
        if (isSimulationActive) HideSimulation();
        else ShowSimulation();
    }

    private void OnToggleSimulationPerformed(InputAction.CallbackContext _) => ToggleSimulation();

    // slider emits absolute float (e.g., 1..5); use it directly
    private void OnSliderChanged(float rawValue) => ApplyValueToActive(rawValue);

    private void ApplyValueToActive(float value)
    {
        var entry = GetEntryForCurrent();
        if (entry == null) return;

        float min = Mathf.Min(entry.minValue, entry.maxValue);
        float max = Mathf.Max(entry.minValue, entry.maxValue);
        float v = Mathf.Clamp(value, min, max);

        if (entry.invert)
        {
            float t01 = Mathf.InverseLerp(min, max, v);
            v = Mathf.Lerp(entry.minValue, entry.maxValue, 1f - t01);
        }

        if (entry.propertyId < 0 && !string.IsNullOrEmpty(entry.propertyName))
            entry.propertyId = Shader.PropertyToID(entry.propertyName);
        if (entry.propertyId < 0 || entry.renderers == null) return;

        for (int i = 0; i < entry.renderers.Length; i++)
        {
            var r = entry.renderers[i];
            if (!r) continue;

            var b = entry.mpbs[i];
            r.GetPropertyBlock(b);
            b.SetFloat(entry.propertyId, v);
            r.SetPropertyBlock(b);
        }
    }

    private DiseaseEntry GetEntryForCurrent()
    {
        if (!currentDiseaseObject || diseases == null) return null;

        var entry = diseases.FirstOrDefault(d => d != null && d.root == currentDiseaseObject);
        if (entry != null) return entry;

        return diseases.FirstOrDefault(d =>
            d != null && d.root && currentDiseaseObject.transform.IsChildOf(d.root.transform));
    }
}

