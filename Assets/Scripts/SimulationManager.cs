using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private DiseaseUIManager uiManager;
    [SerializeField] private GameObject container;

    private GameObject currentDiseaseObject;
    private bool isSimulationActive = false;
    public void InjectDisease(GameObject diseaseObject)
    {
        if (container == null)
        {
            Debug.LogError("Container is not assigned in SimulationManager.");
            return;
        }
        currentDiseaseObject = diseaseObject;   
    }

    public void HideSimulation()
    {
        if (currentDiseaseObject != null)
        {
            uiManager.ToggleMainUI(true);
            currentDiseaseObject.SetActive(false);
            container.SetActive(false);
            isSimulationActive = false;
        }
    }

    public void ShowSimulation()
    {
        if (currentDiseaseObject != null)
        {
            uiManager.ToggleMainUI(false);
            currentDiseaseObject.SetActive(true);
            container.SetActive(true);
            isSimulationActive = true;
        }
    }

    public void ToggleSimulation()
    {
        if (isSimulationActive) {
            HideSimulation();
        } else {
            ShowSimulation();
        }
    }
}
