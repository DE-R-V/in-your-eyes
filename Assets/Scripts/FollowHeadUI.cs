using UnityEngine;

public class FollowHeadUI : MonoBehaviour
{
    [Header("Referenz")]
    public Transform head;                 // XR/AR-Kamera (Kopf des Users)

    [Header("Distanz & Position")]
    [Tooltip("Wenn true, wird die Distanz beim Start aus der Szene übernommen.")]
    public bool useInitialDistance = true;

    [Tooltip("Fester Abstand vor dem Kopf (wird überschrieben, wenn useInitialDistance = true).")]
    public float distance = 1.5f;

    [Tooltip("Zusätzlicher Höhen-Offset relativ zum Kopf.")]
    public float heightOffset = 0.0f;

    [Tooltip("Wie schnell das Panel der Kopfposition folgt.")]
    public float followSpeed = 5f;

    [Header("Rotation")]
    [Tooltip("Wie schnell die Rotation nachzieht.")]
    public float rotationSpeed = 10f;

    [Tooltip("Nur um die Y-Achse drehen (kein Kippen nach oben/unten).")]
    public bool onlyYaw = true;

    [Tooltip("Falls der Text gespiegelt ist → 180° um Y drehen.")]
    public bool flipForward180 = false;

    bool initialized = false;

    void Start()
    {
        if (head == null && Camera.main != null)
            head = Camera.main.transform;

        if (head == null)
            return;

        // Distanz so übernehmen, wie du das Panel im Editor platziert hast
        if (useInitialDistance)
        {
            distance = Vector3.Distance(head.position, transform.position);
        }

        initialized = true;
    }

    void LateUpdate()
    {
        if (!initialized || head == null)
            return;

        // --- Zielposition: fester Abstand vor dem Kopf ---
        Vector3 dir = head.forward;

        if (onlyYaw)
        {
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f)
                return;
            dir.Normalize();
        }

        Vector3 targetPos = head.position + dir * distance;
        targetPos.y += heightOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );

        // --- Rotation: Panel soll zum Kopf ausgerichtet sein ---
        Vector3 lookDir = transform.position - head.position; // Richtung Kopf → Panel
        if (onlyYaw)
            lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);

            if (flipForward180)
                targetRot *= Quaternion.Euler(0f, 180f, 0f);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
