using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwitcherManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button simpleButton;
    public Button preciseButton;

    [Header("Icons & Backgrounds")]
    public SVGImage simpleBackground;
    public SVGImage preciseBackground;
    public SVGImage simpleImage;
    public SVGImage preciseImage;

    [Header("Texts")]
    public TextMeshProUGUI simpleText;
    public TextMeshProUGUI preciseText;

    [Header("Assigned Objects")]
    public GameObject simpleObject;
    public GameObject preciseObject;
    [SerializeField] private RectTransform simulationPanelRect;

    [System.Serializable]
    public class ModeChangedEvent : UnityEvent<string> { }
    [Header("Events")]
    public ModeChangedEvent onModeChanged;
    [Header("Sliders")]
    public Slider simpleSlider;
    public Slider leftPreciseSlider;
    public Slider rightPreciseSlider;

    // Colors
    private Color selectedTextColor = new Color(1f, 1f, 1f, 0.9f);                  // white
    private Color selectedImageColor = new Color(0f / 255f, 119f / 255f, 172f / 255f, 1f); // #0077AC

    private Color unselectedTextColor = new Color(0.153f, 0.153f, 0.153f, 0.9f);      // #272727
    private Color transparent = new Color(0, 0, 0, 0);

    private Color hoverBackgroundColor = new Color(0f, 0.37f, 0.58f, 1f); // subtle blue

    // Internal state
    private bool simpleSelected = true;

    private void Start()
    {
        // Assign click listeners
        simpleButton.onClick.AddListener(() => SetMode(true));
        preciseButton.onClick.AddListener(() => SetMode(false));

        // Add hover events
        AddHover(simpleButton, OnSimpleEnter, OnSimpleExit);
        AddHover(preciseButton, OnPreciseEnter, OnPreciseExit);

        SetMode(true);
    }

    private void SetMode(bool _simpleSelected)
    {
        simpleSelected = _simpleSelected;

        if (simpleSelected)
        {
            rightPreciseSlider.value = 0f;
            leftPreciseSlider.value = 0f;
            // SIMPLE selected
            simpleText.color = selectedTextColor;
            simpleImage.color = selectedTextColor;
            simpleBackground.color = selectedImageColor;
            simulationPanelRect.sizeDelta = new Vector2(simulationPanelRect.sizeDelta.x, 200);


            preciseText.color = unselectedTextColor;
            preciseImage.color = unselectedTextColor;
            preciseBackground.color = transparent;

            simpleObject.SetActive(true);
            preciseObject.SetActive(false);

            onModeChanged?.Invoke("Simple");
        }
        else
        {
            simpleSlider.value = 0f;
            // PRECISE selected
            preciseText.color = selectedTextColor;
            preciseImage.color = selectedTextColor;
            preciseBackground.color = selectedImageColor;
            simulationPanelRect.sizeDelta = new Vector2(simulationPanelRect.sizeDelta.x, 300);

            simpleText.color = unselectedTextColor;
            simpleImage.color = unselectedTextColor;
            simpleBackground.color = transparent;

            simpleObject.SetActive(false);
            preciseObject.SetActive(true);

            onModeChanged?.Invoke("Precise");
        }
    }

    private void AddHover(Button btn, UnityAction enter, UnityAction exit)
    {
        EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

        var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener(_ => enter());
        trigger.triggers.Add(enterEntry);

        var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener(_ => exit());
        trigger.triggers.Add(exitEntry);
    }

    private void OnSimpleEnter()
    {
        if (!simpleSelected)
        {
            simpleBackground.color = hoverBackgroundColor;
        }
    }

    private void OnSimpleExit()
    {
        if (!simpleSelected)
        {
            simpleText.color = unselectedTextColor;
            simpleImage.color = unselectedTextColor;
            simpleBackground.color = transparent;
        }
    }

    private void OnPreciseEnter()
    {
        if (simpleSelected)
        {
            preciseBackground.color = hoverBackgroundColor;
        }
    }

    private void OnPreciseExit()
    {
        if (simpleSelected)
        {
            preciseText.color = unselectedTextColor;
            preciseImage.color = unselectedTextColor;
            preciseBackground.color = transparent;
        }
    }

    public void Reset() 
    {
        SetMode(false);
        leftPreciseSlider.value = 0f;
        rightPreciseSlider.value = 0f;
        SetMode(true);
        simpleSlider.value = 0f;
    }
}
