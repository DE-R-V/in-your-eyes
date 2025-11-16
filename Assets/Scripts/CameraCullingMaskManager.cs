using UnityEngine;

public class CameraCullingMaskManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerPick layer;
    // Update is called once per frame
    void Update()
    {
        switch (layer)
        {
            case LayerPick.everything:
                mainCamera.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "LeftEyeFX", "RightEyeFX");
                break;
            case LayerPick.left:
                mainCamera.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "LeftEyeFX");
                break;
            case LayerPick.right:
                mainCamera.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "RightEyeFX");
                break;
            case LayerPick.noFX:
                mainCamera.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Water", "UI");
                break;
        }
    }
}

enum LayerPick
{
    everything,
    left,
    right,
    noFX
}