using UnityEngine;
using UnityEngine.UI;

public class PanelHoverColor : MonoBehaviour
{
    [Header("Target")]
    public Image targetImage;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;

    private void Awake()
    {
        if (targetImage != null)
            targetImage.color = normalColor;
    }

    // Diese beiden Methoden rufen wir aus den XR-Events auf
    public void OnHoverEnter()
    {
        if (targetImage != null)
            targetImage.color = hoverColor;
    }

    public void OnHoverExit()
    {
        if (targetImage != null)
            targetImage.color = normalColor;
    }
}
