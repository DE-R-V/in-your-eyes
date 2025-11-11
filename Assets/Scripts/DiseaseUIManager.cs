using UnityEngine;
using UnityEngine.UI;
using TMPro;   // falls du TextMeshPro verwendest

[System.Serializable]
public class DiseaseData
{
    public string name;
    [TextArea(3, 6)]
    public string description;
}

public class DiseaseUIManager : MonoBehaviour
{
    [Header("Flow Panels")]
    public GameObject startPanel;       // z.B. dein erstes Panel mit Einleitung
    public GameObject selectionPanel;   // Panel mit den Toggles
    public GameObject infoPanel;        // Panel mit Titel + Beschreibung

    [Header("Auswahl-Toggles")]
    [Tooltip("Toggles in der gleichen Reihenfolge wie die Disease-Daten.")]
    public Toggle[] diseaseToggles;

    [Header("Info UI")]
    public TMP_Text infoTitleText;      // Überschrift im InfoPanel
    public TMP_Text infoBodyText;       // Beschreibung im InfoPanel

    [Header("Disease Daten")]
    public DiseaseData[] diseases;      // Name + Beschreibung pro Krankheit

    private void Start()
    {
        // Startzustand
        if (startPanel != null) startPanel.SetActive(true);
        if (selectionPanel != null) selectionPanel.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);

        // Alle Toggles erstmal aus
        if (diseaseToggles != null)
        {
            foreach (var t in diseaseToggles)
            {
                if (t != null) t.isOn = false;
            }
        }

        // Optional: Toggle-Labels automatisch mit Krankheitsnamen füllen
        if (diseaseToggles != null && diseases != null)
        {
            int count = Mathf.Min(diseaseToggles.Length, diseases.Length);
            for (int i = 0; i < count; i++)
            {
                var label = diseaseToggles[i].GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = diseases[i].name;
            }
        }
    }

    // Vom Start-Button aufgerufen
    public void OnStartButtonPressed()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (selectionPanel != null) selectionPanel.SetActive(true);
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    // Von jedem Toggle (OnValueChanged, mit Index 0–3) aufgerufen
    public void OnDiseaseSelected(int index)
    {
        // Sicherheitschecks
        if (diseaseToggles == null || index < 0 || index >= diseaseToggles.Length)
            return;

        // Nur reagieren, wenn der Toggle gerade AN ist
        if (!diseaseToggles[index].isOn)
            return;

        if (diseases == null || index >= diseases.Length)
            return;

        var d = diseases[index];

        if (infoTitleText != null)
            infoTitleText.text = d.name;

        if (infoBodyText != null)
            infoBodyText.text = d.description;

        if (infoPanel != null)
            infoPanel.SetActive(true);
    }

    // Optional: InfoPanel schließen + Auswahl zurücksetzen
    public void CloseInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (diseaseToggles != null)
        {
            foreach (var t in diseaseToggles)
            {
                if (t != null) t.isOn = false;
            }
        }
    }
}
