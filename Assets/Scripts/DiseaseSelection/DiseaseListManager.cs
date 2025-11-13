using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiseaseListManager : MonoBehaviour
{
    [Header("Disease objects to handle (under main camera)")]
    [SerializeField] private List<GameObject> diseasesList = new List<GameObject>();

    [Header("Main camera")]
    [SerializeField] private GameObject mainCameraObject;

    [Header("Diseases List")]
    [SerializeField] private GameObject diseasesListContainer;
    [SerializeField] private GameObject diseaseItemPrefab;

    [Header("Toggle Manager")]
    [SerializeField] private DiseaseToggleManager toggleManager; // assign in inspector or find in scene

        private void Start()
    {
        // fallback to main camera children if list empty
        if (diseasesList == null || diseasesList.Count == 0)
        {
            if (mainCameraObject == null)
                mainCameraObject = Camera.main?.gameObject;

            if (mainCameraObject != null)
            {
                foreach (Transform child in mainCameraObject.transform)
                    diseasesList.Add(child.gameObject);
            }
            else
            {
                Debug.LogWarning("No diseases provided and no main camera found!");
                return;
            }
        }

        // clear previous items
        foreach (Transform child in diseasesListContainer.transform)
            Destroy(child.gameObject);

        // instantiate prefabs
        foreach (GameObject disease in diseasesList)
        {
            GameObject item = Instantiate(diseaseItemPrefab, diseasesListContainer.transform);

            string displayName = disease.name.Replace("_", " ");

            // assign label
            TextMeshProUGUI tmpLabel = item.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpLabel != null)
                tmpLabel.text = displayName;
            else
            {
                Text uiText = item.GetComponentInChildren<Text>();
                if (uiText != null) uiText.text = displayName;
            }

            // setup toggle
            DiseaseItemToggle toggle = item.GetComponent<DiseaseItemToggle>();
            if (toggle != null)
            {
                toggle.diseaseName = displayName;

                // register with manager
                if (toggleManager != null)
                    toggleManager.RegisterToggle(toggle);

                // add optional listener
                toggle.onItemSelected.AddListener(name => Debug.Log($"Selected disease: {name}"));
            }
        }

        Debug.Log($"Populated {diseasesList.Count} disease items into {diseasesListContainer.name}");
    }
}
