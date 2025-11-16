using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VectorGraphics;

/// <summary>
/// Per-item toggle logic: visuals (circle & tick) + click handling + hover highlighting.
/// </summary>
public class DiseaseItemToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SVGImage backgroundImage;
    public TMPro.TextMeshProUGUI labelText;
    public SVGImage statusImage;

    [Header("State Sprites")]
    public Sprite uncheckedSprite; 
    public Sprite checkedSprite;   

    private string diseaseName;
    private string diseaseDescription;
    private DiseaseData diseaseData;
    private int index;

    private DiseaseUIManager uiManager;
    private DiseaseToggleGroup toggleGroup;

    private bool isSelected = false;

    [SerializeField] private Button button;

    private static readonly Color whiteOpaque = new Color(1f, 1f, 1f, 1f);
    private static readonly Color whiteTransparent = new Color(1f, 1f, 1f, 0f);

    public void Setup(DiseaseData data, int idx,
                      DiseaseToggleGroup group, DiseaseUIManager manager)
    {
        diseaseData = data;
        diseaseName = data.name;
        diseaseDescription = data.description;
        index = idx;
        toggleGroup = group;
        uiManager = manager;

        if (toggleGroup != null) toggleGroup.Register(this);

        TMPro.TextMeshProUGUI tmpLabel = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpLabel != null && string.IsNullOrEmpty(tmpLabel.text))
            tmpLabel.text = diseaseName;

        if (button != null)
        {
            button.onClick.RemoveListener(OnClicked);
            button.onClick.AddListener(OnClicked);
        }

        SetState(false);
    }

    public void OnClicked()
    {
        toggleGroup?.Select(this);
        uiManager?.ShowDiseaseInfo(diseaseData);
        Debug.Log($"Clicked toggle: {diseaseName} (index {index})");
    }

    /// <summary>
    /// Set visuals for selected/unselected.
    /// </summary>
    public void SetState(bool selected)
    {
        isSelected = selected;

        if (backgroundImage == null)
            return;

        labelText.color = selected ? Color.white : new Color32(39, 39, 39, 255);
        statusImage.sprite = selected ? checkedSprite : uncheckedSprite;
        backgroundImage.color = selected ? new Color32(0, 95, 150, 255) : whiteTransparent;
    }

    public bool IsSelected => isSelected;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage == null) return;

        // Highlight on hover (even if not selected)
        backgroundImage.color = new Color32(90, 180, 230, 255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage == null) return;

        // If selected, stay opaque.  
        // If not selected, go back to transparent.
        backgroundImage.color = isSelected ? new Color32(0, 95, 150, 255) : whiteTransparent;
    }
}
