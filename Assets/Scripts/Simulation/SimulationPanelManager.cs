using UnityEngine;
using TMPro;

public class SimulationPanelManager : MonoBehaviour
{
    [SerializeField] private XRInteractionMode xrModeManager;
    [SerializeField] private TMPro.TextMeshProUGUI label;

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
}
