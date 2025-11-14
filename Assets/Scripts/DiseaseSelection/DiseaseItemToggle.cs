using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Per-item toggle logic: visuals (circle & tick) + click handling.
/// </summary>
public class DiseaseItemToggle : MonoBehaviour
{
    [Header("State Sprites")]
    public Sprite circleSprite;       // unselected circle sprite
    public Sprite fullCircleSprite;   // selected circle sprite

    [Header("State Visual Components")]
    public Image circleImage;         // Image that displays the circle sprite
    public Image tickImage;           // Tick image (enable/disable)

    // runtime data
    private string diseaseName;
    private string diseaseDescription;
    private DiseaseData diseaseData;
    private int index;

    private DiseaseUIManager uiManager;
    private DiseaseToggleGroup toggleGroup;

    private bool isSelected = false;

    [SerializeField] private Button button;
    /// <summary>
    /// Initialize the toggle with data, register with group, and wire the click
    /// </summary>
    public void Setup(DiseaseData data, int idx,
                      DiseaseToggleGroup group, DiseaseUIManager manager)
    {
        diseaseData = data;
        diseaseName = data.name;
        diseaseDescription = data.description;
        index = idx;
        toggleGroup = group;
        uiManager = manager;

        // Register in the group for exclusive selection
        if (toggleGroup != null) toggleGroup.Register(this);

        // Ensure the label matches if it wasn't set by the spawner for some reason:
        TMPro.TextMeshProUGUI tmpLabel = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpLabel != null && string.IsNullOrEmpty(tmpLabel.text))
            tmpLabel.text = diseaseName;

        // Wire up button click (avoid duplicate listeners)
        if (button != null)
        {
            button.onClick.RemoveListener(OnClicked);
            button.onClick.AddListener(OnClicked);
        }
        else
        {
            Debug.LogWarning($"DiseaseItemToggle on '{name}' has no Button component.");
        }

        // Start unselected visually
        SetState(false);
    }

    /// <summary>
    /// Called when this item's Button is clicked.
    /// </summary>
    public void OnClicked()
    {
        // Select this in the group (will call SetState for all toggles)
        if (toggleGroup != null) toggleGroup.Select(this);
        else Debug.LogWarning("ToggleGroup is null on click.");

        // Update the info panel
        if (uiManager != null)
            uiManager.ShowDiseaseInfo(diseaseData);
        else
            Debug.LogWarning("UI Manager not assigned for DiseaseItemToggle.");

        Debug.Log($"Clicked toggle: {diseaseName} (index {index})");
    }

    /// <summary>
    /// Changes visuals for selected/unselected.
    /// </summary>
    public void SetState(bool selected)
    {
        isSelected = selected;

        if (circleImage != null)
        {
            circleImage.sprite = selected ? fullCircleSprite : circleSprite;
        }
        else
        {
            Debug.LogWarning($"circleImage missing on toggle '{diseaseName}'");
        }

        if (tickImage != null)
        {
            tickImage.gameObject.SetActive(selected);
        }
        else
        {
            Debug.LogWarning($"tickImage missing on toggle '{diseaseName}'");
        }
    }

    public bool IsSelected => isSelected;
}
