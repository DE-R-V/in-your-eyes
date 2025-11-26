using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Audio")]
    public AudioSource audioSource;          // Required
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private void Awake()
    {        
        // --- Proper Singleton Enforcement ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!audioSource)
        {
            Debug.LogError("UISoundBinder: No AudioSource assigned.");
            return;
        }

        BindAllUIElements();
    }

    private void BindAllUIElements()
    {
        // Find ALL Selectables in the scene (buttons, toggles, etc.)
        Selectable[] elements = FindObjectsOfType<Selectable>(true);

        foreach (var element in elements)
        {
            BindHoverSound(element);
            BindClickSound(element);
        }

        Debug.Log($"UISoundBinder: Bound UI sounds to {elements.Length} UI elements.");
    }

    
    public void BindUIElement(Selectable selectable)
    {
        print("Binding UI sounds to new element");
        BindHoverSound(selectable);
        BindClickSound(selectable);
    }

    private void BindHoverSound(Selectable element)
    {
        var trigger = element.gameObject.GetComponent<EventTrigger>();
        if (!trigger)
            trigger = element.gameObject.AddComponent<EventTrigger>();

        // Pointer Enter event
        AddTrigger(trigger, EventTriggerType.PointerEnter, () =>
        {
            if (hoverSound) audioSource.PlayOneShot(hoverSound);
        });
    }

    private void BindClickSound(Selectable element)
    {
        if (element is Button button)
        {
            button.onClick.AddListener(() => audioSource.PlayOneShot(clickSound));
        }
        else if (element is Toggle toggle)
        {
            toggle.onValueChanged.AddListener(_ => audioSource.PlayOneShot(clickSound));
        }
    }

    private void AddTrigger(EventTrigger trigger, EventTriggerType type, System.Action callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => callback());
        trigger.triggers.Add(entry);
    }
}
