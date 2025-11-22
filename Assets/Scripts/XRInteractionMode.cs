using System;
using UnityEngine;
using Oculus.Platform; // If you have the Oculus SDK imported
using Oculus.Platform.Models;

public enum XRInputMode
{
    Controllers,
    Hands
}
public class XRInteractionMode: MonoBehaviour
{
     public XRInputMode CurrentMode { get; private set; }

    /// <summary>
    /// Fired whenever the input mode changes.
    /// </summary>
    public event Action<XRInputMode> OnInputModeChanged;

    private XRInputMode lastMode;

    private void Start()
    {
        // Initialize mode on startup
        UpdateMode();
    }

    private void Update()
    {
        UpdateMode();
    }

    private void UpdateMode()
    {
        XRInputMode detectedMode = DetectInputMode();

        if (detectedMode != lastMode)
        {
            lastMode = detectedMode;
            CurrentMode = detectedMode;

            // Fire event
            OnInputModeChanged?.Invoke(CurrentMode);
        }
    }

    /// <summary>
    /// Detects the current active input mode.
    /// </summary>
    /// <returns></returns>
    private XRInputMode DetectInputMode()
    {
        var activeController = OVRInput.GetActiveController();

        if (activeController == OVRInput.Controller.Hands)
            return XRInputMode.Hands;

        return XRInputMode.Controllers;
    }
}
