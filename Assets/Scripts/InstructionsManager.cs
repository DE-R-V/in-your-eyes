using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] instructionsPanels;
    private int currentPanelIndex = 0;

    public void ShowNextPanel()
    {
        if (currentPanelIndex < instructionsPanels.Length - 1)
        {
            currentPanelIndex++;
        } else {
            currentPanelIndex = 0;
        }
        UpdatePanelVisibility();
    }

    public void ShowPreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
        } else {
            currentPanelIndex = instructionsPanels.Length - 1;
        }
        UpdatePanelVisibility();
    }

    private void UpdatePanelVisibility()
    {
        for (int i = 0; i < instructionsPanels.Length; i++)
        {
            instructionsPanels[i].SetActive(i == currentPanelIndex);
        }
    }
}
