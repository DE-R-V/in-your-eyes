using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class DiseaseData
{
    public string name;
    [TextArea(3, 6)]
    public string description;
    public GameObject sourceObject; 
    public Sprite illustration;
}

[System.Serializable]
public class onSimulationStarted : UnityEvent<GameObject> { } // passes the selected GameObject

public class DiseaseUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;        // First panel shown at launch
    public GameObject simulationPanel;   // Panel for the simulation
    public GameObject homeCanvas;        // Main UI canvas
    public GameObject homeRightColumnObject;    // Panel that contains the toggle list
    public GameObject homeLeftColumnObject;     // Sidebar UI panel
    public GameObject infoPanel;        // Panel that shows disease info
    public GameObject openingPanel;     // Panel that contains simulation UI

    public GameObject creditsPanel;     // Panel that shows credits
    public GameObject instructionPanel;   // Panel that shows instructions

    [Header("Info Panel UI")]
    public TMP_Text infoTitleText;       // Disease name label
    public TMP_Text infoBodyText;        // Disease description label
    public Image infoIllustration;   // Disease illustration image

    [Header("Disease Database")]
    public DiseaseData[] diseases;       // Name + description + object reference

    [Header("Toggle Generation")]
    public GameObject diseaseItemPrefab; // Prefab for each toggle item
    public Transform toggleContainer;    // Parent object for toggle instances

    private DiseaseToggleGroup toggleGroup;

    [Header("Simulation")]
    public SimulationManagerNew simulationManager;
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
        if (homeRightColumnObject) homeRightColumnObject.SetActive(false);
        if (homeLeftColumnObject) homeLeftColumnObject.SetActive(false);
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
        homeLeftColumnObject.SetActive(true);
        homeRightColumnObject.SetActive(true);
        openingPanel.SetActive(true);
        infoPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    /// <summary>
    /// Displays information for a selected disease.
    /// Called from DiseaseItemToggle.
    /// </summary>
    public void ShowDiseaseInfo(DiseaseData data)
    {
        if (infoTitleText) infoTitleText.text = data.name;
        if (infoBodyText) infoBodyText.text = data.description;
        if (infoIllustration != null) infoIllustration.sprite = data.illustration;
        simulationManager.InjectDisease(data.sourceObject);

        openingPanel.SetActive(false);
        creditsPanel.SetActive(false);
        instructionPanel.SetActive(false);
        infoPanel.SetActive(true);
    }

    /// <summary>
    /// Changes activation status
    /// </summary>
    public void ToggleMainUI(bool isActive)
    {
        homeCanvas.SetActive(isActive); 
    }

    public void BackToSelection()
    {
        if (infoPanel != null) infoPanel.SetActive(false);
        if (openingPanel != null) openingPanel.SetActive(true);
        if (homeRightColumnObject != null) homeRightColumnObject.SetActive(true);
        toggleGroup.ClearSelection();
    }
    public void ToggleCredits()
    {
        toggleGroup.ClearSelection();
        if (creditsPanel != null)
        {
            bool isActive = creditsPanel.activeSelf;
            creditsPanel.SetActive(!isActive);
            if (!isActive)
            {
                // Hiding other panels when showing credits
                if (infoPanel != null) infoPanel.SetActive(false);
                if (openingPanel != null) openingPanel.SetActive(false);
                if (instructionPanel != null) instructionPanel.SetActive(false);
            }
            else
            {
                // Restoring opening panel when hiding credits
                if (openingPanel != null) openingPanel.SetActive(true);
            }
        }
    }

    public void ToggleInstructions()
    {
        toggleGroup.ClearSelection();
        if (instructionPanel != null)
        {
            bool isActive = instructionPanel.activeSelf;
            instructionPanel.SetActive(!isActive);
            if (!isActive)
            {
                // Hiding other panels when showing instructions
                if (infoPanel != null) infoPanel.SetActive(false);
                if (openingPanel != null) openingPanel.SetActive(false);
            }
            else
            {
                // Restoring opening panel when hiding instructions
                if (openingPanel != null) openingPanel.SetActive(true);
            }
        }
    }
}
