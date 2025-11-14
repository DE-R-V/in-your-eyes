using System.Collections.Generic;
using UnityEngine;

public class DiseaseToggleGroup : MonoBehaviour
{
    private List<DiseaseItemToggle> toggles = new List<DiseaseItemToggle>();

    public void Register(DiseaseItemToggle toggle)
    {
        if (!toggles.Contains(toggle))
            toggles.Add(toggle);
    }

    public void Select(DiseaseItemToggle selected)
    {
        foreach (var t in toggles)
        {
            bool isCurrent = t == selected;
            t.SetState(isCurrent);
        }
    }

    public void ClearSelection()
    {
        foreach (var t in toggles)
            t.SetState(false);
    }
}
