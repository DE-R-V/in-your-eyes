using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MatchColliderToRect : MonoBehaviour
{
    public RectTransform targetRect;

    private void Reset()
    {
        if (targetRect == null)
            targetRect = GetComponentInChildren<RectTransform>();
    }

    private void Start()
    {
        var box = GetComponent<BoxCollider>();
        if (targetRect == null) return;

        
        Vector2 size = targetRect.rect.size;
        Vector3 scale = targetRect.lossyScale;

        box.size = new Vector3(size.x * scale.x, size.y * scale.y, 0.01f);
        box.center = Vector3.zero;
    }
}
