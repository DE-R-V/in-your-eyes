using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SimulationManagerNew : MonoBehaviour
{
    [SerializeField] private DiseaseUIManager uiManager;
    [SerializeField] private GameObject container;

    private GameObject currentDiseaseObject;
    private bool isSimulationActive = false;

    // NEW: Pressing this action (bind it to A / Primary Button) exits the simulation.
    [Header("Input")]
    [SerializeField] private InputActionReference exitToInfoAction; // e.g. XRI RightHand / Primary Button

    [SerializeField] private GameObject simulationUIRoot;

    [SerializeField] private GameObject infoPanel;

    // NEW: Slider that directly provides absolute float values (e.g., 1..5)
    [SerializeField] private Slider slider;

    // NEW: Per-disease shader mapping (drives a float property on renderers under 'root')
    [Serializable]
    public class DiseaseEntry
    {
        public GameObject root;
        public Renderer[] renderersOverride;                 
        public string propertyName = "_Intensity";           
        public float minValue = 1f;                          
        public float maxValue = 5f;                        
        public bool invert = false;                         

        [NonSerialized] public int propertyId = -1;
        [NonSerialized] public Renderer[] renderers;
        [NonSerialized] public MaterialPropertyBlock[] mpbs;
    }


    [SerializeField] private DiseaseEntry[] diseases;

    // NEW: Cache renderers/MPBs once and hook the slider
    private void Awake()
    {
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
                for (int i = 0; i < d.mpbs.Length; i++)
                    d.mpbs[i] = new MaterialPropertyBlock();
            }
        }

        if (slider) slider.onValueChanged.AddListener(OnSliderChanged);
    }

    // NEW: Enable/disable the action and hook the callback.
    private void OnEnable()
    {
        if (exitToInfoAction != null)
        {
            exitToInfoAction.action.performed += OnExitToInfoPerformed;
            exitToInfoAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (exitToInfoAction != null)
        {
            exitToInfoAction.action.performed -= OnExitToInfoPerformed;
            exitToInfoAction.action.Disable();
        }
    }

    public void InjectDisease(GameObject diseaseObject)
    {
        if (container == null)
        {
            Debug.LogError("Container is not assigned in SimulationManager.");
            return;
        }
        currentDiseaseObject = diseaseObject;

        // NEW: Immediately sync visuals to the current slider value
        if (slider) ApplyValueToActive(slider.value);
    }

    public void HideSimulation()
    {
        if (currentDiseaseObject != null)
        {
            uiManager.ToggleMainUI(true);

  
            if (simulationUIRoot) simulationUIRoot.SetActive(false);

            currentDiseaseObject.SetActive(false);
            container.SetActive(false);
            isSimulationActive = false;
        }
    }

    public void ShowSimulation()
    {
        if (currentDiseaseObject != null)
        {
            uiManager.ToggleMainUI(false);

       
            if (infoPanel) infoPanel.SetActive(false);

        
            if (simulationUIRoot) simulationUIRoot.SetActive(true);

            currentDiseaseObject.SetActive(true);
            container.SetActive(true);
            isSimulationActive = true;

            // NEW: Push current slider value to shader on show
            if (slider) ApplyValueToActive(slider.value);
        }
    }

    public void ToggleSimulation()
    {
        if (isSimulationActive)
        {
            HideSimulation();
        }
        else
        {
            ShowSimulation();
        }
    }

    // NEW: Slider callback -> use absolute slider value (e.g., 1..5) directly
    private void OnSliderChanged(float rawValue) => ApplyValueToActive(rawValue);

    // NEW: Drive the active disease's shader property via MPB
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
            t01 = 1f - t01;
            v = Mathf.Lerp(entry.minValue, entry.maxValue, t01);
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
        if (currentDiseaseObject == null || diseases == null) return null;

        var entry = diseases.FirstOrDefault(d => d != null && d.root == currentDiseaseObject);
        if (entry != null) return entry;

        return diseases.FirstOrDefault(d =>
            d != null && d.root && currentDiseaseObject.transform.IsChildOf(d.root.transform));
    }

    // NEW: This is called when the A/Primary Button action fires.
    private void OnExitToInfoPerformed(InputAction.CallbackContext _)
    {
        // Close simulation and bring back the info panel.
        HideSimulation();
        if (infoPanel) infoPanel.SetActive(true);
    }
}
