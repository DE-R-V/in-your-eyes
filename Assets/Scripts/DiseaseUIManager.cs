using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class DiseaseData
{
    public string name;
    [TextArea(3, 6)]
    public string description;
    public GameObject sourceObject; 
}

[System.Serializable]
public class onSimulationStarted : UnityEvent<GameObject> { } // passes the selected GameObject

public class DiseaseUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;        // First panel shown at launch
    public GameObject selectionPanel;    // Panel that contains the toggle list
    public GameObject infoPanel;         // Panel with disease name + description
    public GameObject simulationPanel;   // Panel for the simulation
    public GameObject homeCanvas;        // Main UI canvas

    [Header("Info Panel UI")]
    public TMP_Text infoTitleText;       // Disease name label
    public TMP_Text infoBodyText;        // Disease description label

    [Header("Disease Database")]
    public DiseaseData[] diseases;       // Name + description + object reference

    [Header("Toggle Generation")]
    public GameObject diseaseItemPrefab; // Prefab for each toggle item
    public Transform toggleContainer;    // Parent object for toggle instances

    private DiseaseToggleGroup toggleGroup;

    [Header("Simulation")]
    public SimulationManager simulationManager;
    public onSimulationStarted onSimulationStarted;

    private void Awake()
    {
        // Get the toggle manager controlling exclusive selection
        toggleGroup = GetComponent<DiseaseToggleGroup>();
    }

    private void Start()
    {
        SetupPanels();
        GenerateToggles();
    }

    /// <summary>
    /// Initializes panel visibility at startup.
    /// </summary>
    private void SetupPanels()
    {
        if (startPanel) startPanel.SetActive(true);
        if (selectionPanel) selectionPanel.SetActive(false);
        if (infoPanel) infoPanel.SetActive(false);
    }

    /// <summary>
    /// Creates the toggle items dynamically based on the DiseaseData array.
    /// </summary>
    private void GenerateToggles()
    {
        // Clear existing children (in case of refresh or editor testing)
        foreach (Transform child in toggleContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < diseases.Length; i++)
        {
            DiseaseData data = diseases[i];

            // instantiate item
            GameObject item = Instantiate(diseaseItemPrefab, toggleContainer);

            // --- SET THE LABEL FROM DiseaseData ---
            TMPro.TextMeshProUGUI tmpLabel = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpLabel != null)
            {
                tmpLabel.text = data.name;
            }
            else
            {
                UnityEngine.UI.Text uiText = item.GetComponentInChildren<UnityEngine.UI.Text>();
                if (uiText != null)
                    uiText.text = data.name;
            }

            // --- PASS DATA + REFERENCES TO THE TOGGLE ---
            DiseaseItemToggle toggle = item.GetComponent<DiseaseItemToggle>();
            if (toggle != null)
            {
                toggle.Setup(data, i, toggleGroup, this);
            }
            else
            {
                Debug.LogWarning($"Prefab {diseaseItemPrefab.name} missing DiseaseItemToggle component");
            }
        }

    }

    /// <summary>
    /// Called by the Start button to move from intro → selection panel.
    /// </summary>
    public void OnStartPressed()
    {
        startPanel.SetActive(false);
        selectionPanel.SetActive(true);
        infoPanel.SetActive(false);
    }

    /// <summary>
    /// Displays information for a selected disease.
    /// Called from DiseaseItemToggle.
    /// </summary>
    public void ShowDiseaseInfo(DiseaseData data)
    {
        if (infoTitleText) infoTitleText.text = data.name;
        if (infoBodyText) infoBodyText.text = data.description;
        simulationManager.InjectDisease(data.sourceObject);

        infoPanel.SetActive(true);
    }

    /// <summary>
    /// Hides the info panel and resets the toggle group.
    /// </summary>
    public void CloseInfo()
    {
        infoPanel.SetActive(false);
        toggleGroup.ClearSelection();
    }

    /// <summary>
    /// Changes activation status
    /// </summary>
    public void ToggleMainUI(bool isActive)
    {
        homeCanvas.SetActive(isActive); 
    }
}
