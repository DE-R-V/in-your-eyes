using UnityEngine;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine.UI;

public enum SliderSide
{
    Left,
    Right


}

public class SimulationPanelManager : MonoBehaviour
{
    [SerializeField] private XRInteractionMode xrModeManager;
    [SerializeField] private DiseaseUIManager uiManager;

    [Header("Simulation Panel")]
    [SerializeField] private GameObject simulationPanel;
    [SerializeField] private TextMeshProUGUI label;
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

    private void Awake()
    {                
        leftSliderBackground.sizeDelta = new Vector2(300, leftSliderBackground.sizeDelta.y);      
        rightSliderBackground.sizeDelta = new Vector2(300, rightSliderBackground.sizeDelta.y);
        leftSliderContent.SetActive(false);
        rightSliderContent.SetActive(false);
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
                label.text = "Do a thumbs up to close the simulation.";
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
        if (side == SliderSide.Left.ToString())
        {
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
        }
        else if (side == SliderSide.Right.ToString())
        {
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
        }
    }
}
