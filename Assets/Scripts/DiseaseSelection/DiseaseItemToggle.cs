using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class DiseaseItemToggle : MonoBehaviour
{
    [Header("State Images")]
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private Sprite fullCircleImage;

    [Header("State Objects")]
    [SerializeField] private Image circleImage;
    [SerializeField] private Image tickImage;

    [HideInInspector] public string diseaseName;
    [HideInInspector] public UnityEvent<string> onItemSelected;

    [HideInInspector] public DiseaseToggleManager manager; // reference to manager
    [HideInInspector] public bool isSelected = false;

    private Button button;

    
    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClick);

        SetState(false); // start deselected
    }

    public void OnClick()
    {
        if (manager != null)
            manager.SelectToggle(this); // manager will set this toggle selected, others deselected

        onItemSelected?.Invoke(diseaseName); // invoke any listeners
    }

    public void SetState(bool selected)
    {
        isSelected = selected;

        if (circleImage != null)
            circleImage.sprite = selected ? fullCircleImage : circleSprite;

        if (tickImage != null)
            tickImage.gameObject.SetActive(selected);
    }
}
