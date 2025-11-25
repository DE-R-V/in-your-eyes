using UnityEngine;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine.UI;

public class SimulationPanelManager : MonoBehaviour
{
    [SerializeField] private XRInteractionMode xrModeManager;
    [SerializeField] private DiseaseUIManager uiManager;

    [Header("Simulation Panel")]
    [SerializeField] private GameObject simulationPanel;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private SwitcherManager switcherManager;
    [Header("Overall Slider")]
    [SerializeField] private RectTransform overallSliderBackground;
    [SerializeField] private GameObject overallSliderContent;
    [Header("Left Slider")]
    [SerializeField] private RectTransform leftSliderBackground;
    [SerializeField] private GameObject leftSliderContent;
    [Header("Right Slider")]
    [SerializeField] private RectTransform rightSliderBackground;
    [SerializeField] private GameObject rightSliderContent;
    
    [Header("Disclaimer Panel")]
    [SerializeField] private GameObject disclaimerPanel;
    [SerializeField] private Image diseasePhoto;
    [SerializeField] private TextMeshProUGUI diseaseName;
    [SerializeField] private TextMeshProUGUI diseaseDescription;


    private bool isLeftSliderOpen = false;
    private bool isRightSliderOpen = false;
    private bool isOverallSliderOpen = false;

    private void Awake()
    {                
        leftSliderBackground.sizeDelta = new Vector2(300, leftSliderBackground.sizeDelta.y);      
        rightSliderBackground.sizeDelta = new Vector2(300, rightSliderBackground.sizeDelta.y);
        overallSliderBackground.sizeDelta = new Vector2(300, overallSliderBackground.sizeDelta.y);
        leftSliderContent.SetActive(false);
        rightSliderContent.SetActive(false);
        overallSliderContent.SetActive(false);
    }
    private void OnEnable()
    {
        if (xrModeManager != null)
            xrModeManager.OnInputModeChanged += UpdateLabel;
    }

    private void OnDisable()
    {
        if (xrModeManager != null)
            xrModeManager.OnInputModeChanged -= UpdateLabel;
    }

    private void Start()
    {
        // Initialize label based on current mode
        if (xrModeManager != null)
            UpdateLabel(xrModeManager.CurrentMode);
    }

    private void UpdateLabel(XRInputMode mode)
    {
        if (label == null) return;

        switch (mode)
        {
            case XRInputMode.Controllers:
                label.text = "Press X, Y, A or B to close the simulation.";
                break;
            case XRInputMode.Hands:
                label.text = "Do a 'call me' gesture to close the simulation.";
                break;
        }
    }

    public void OpenSimulationPanel()
    {
        print("Opening simulation panel");
        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(false);
        if (simulationPanel != null)
            simulationPanel.SetActive(true);
    }

    public void OpenDiscaimerPanel()
    {
        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(true);
        uiManager.ToggleMainUI(false);
        uiManager.infoPanel.SetActive(false);
    }

    public void CloseSimulationPanel()
    {
        if (simulationPanel != null)
            simulationPanel.SetActive(false);
        uiManager.ToggleMainUI(true);
        uiManager.infoPanel.SetActive(true);
        switcherManager.Reset();
    }

    public void CloseDisclaimerPanel()
    {
        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(false);
        uiManager.ToggleMainUI(true);
        uiManager.infoPanel.SetActive(true);
    }

    public void InjectDiseaseInfo(Sprite photo, string name, string description)
    {
        if (diseasePhoto != null)
            diseasePhoto.sprite = photo;

        if (diseaseName != null)
            diseaseName.text = name;

        if (diseaseDescription != null)
            diseaseDescription.text = description;
    }

    public void ToggleSlider(string side)
    {
        print("Toggling slider: " + side);
        switch(side) 
        {
            case "Left":
                print("Toggling left slider");
                if(isLeftSliderOpen)
                {
                    leftSliderContent.SetActive(false);
                    isLeftSliderOpen = false;
                    leftSliderBackground.sizeDelta = new Vector2(300, leftSliderBackground.sizeDelta.y);
                }
                else
                {
                    leftSliderContent.SetActive(true);
                    isLeftSliderOpen = true;
                    leftSliderBackground.sizeDelta = new Vector2(800, leftSliderBackground.sizeDelta.y);
                }
                break;
            case "Right":
                print("Toggling right slider");
                if(isRightSliderOpen)
                {
                    rightSliderContent.SetActive(false);
                    isRightSliderOpen = false;
                    rightSliderBackground.sizeDelta = new Vector2(300, rightSliderBackground.sizeDelta.y);
                }
                else
                {
                    rightSliderContent.SetActive(true);
                    isRightSliderOpen = true;
                    rightSliderBackground.sizeDelta = new Vector2(800, rightSliderBackground.sizeDelta.y);
                }
                break;
            case "Overall":
                print("Toggling overall slider");
                if(isOverallSliderOpen)
                {
                    overallSliderContent.SetActive(false);
                    isOverallSliderOpen = false;
                    overallSliderBackground.sizeDelta = new Vector2(300, overallSliderBackground.sizeDelta.y);
                }
                else
                {
                    overallSliderContent.SetActive(true);
                    isOverallSliderOpen = true;
                    overallSliderBackground.sizeDelta = new Vector2(800, overallSliderBackground.sizeDelta.y);
                }
                break;
        }
    }
}
