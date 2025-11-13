using System.Collections.Generic;
using UnityEngine;

public class DiseaseToggleManager : MonoBehaviour
{
    [HideInInspector] public List<DiseaseItemToggle> toggles = new List<DiseaseItemToggle>();

    
    public void RegisterToggle(DiseaseItemToggle toggle)
    {
        if (!toggles.Contains(toggle))
        {
            toggles.Add(toggle);
            toggle.manager = this;
        }
    }

    public void SelectToggle(DiseaseItemToggle selectedToggle)
    {
        foreach (var toggle in toggles)
            toggle.SetState(toggle == selectedToggle);
    }

    public DiseaseItemToggle GetCurrentSelected()
    {
        return toggles.Find(t => t.isSelected);
    }
}
