using UnityEngine;
using UnityEngine.UI;

public class VideoIconOverlay : MonoBehaviour
{
    public RectTransform icon;          // The UI Image component for the icon
    public OVRCameraRig mainCamera;     // Reference to the camera

    private Vector3 initialIconPosition; // Starting position of the icon
    private Vector3 initialCameraPosition; // Starting position of the camera

    void Start()
    {
        // Get the initial position of the icon (center of the RawImage)
        initialIconPosition = icon.localPosition;
        Debug.Log($"Initial Icon Position: {initialIconPosition}");
        
        // Store the initial camera position
        initialCameraPosition = mainCamera.transform.position;
    }

    void Update()
    {
        // Calculate the offset between the camera's current position and its starting position
        float cameraZOffset = mainCamera.transform.position.z - initialCameraPosition.z;
        float cameraXOffset = mainCamera.transform.position.x - initialCameraPosition.x;

        Debug.Log($"Camera Z Offset: {cameraZOffset}, Camera X Offset: {cameraXOffset}");

        // Calculate the new icon position based on the camera's movement
        float iconX = initialIconPosition.x + (cameraZOffset / 250f) * 100f; // Z -> X movement
        float iconY = initialIconPosition.y + (cameraXOffset / 250f) * 100f; // X -> Y movement

        Debug.Log($"Icon X: {iconX}, Icon Y: {iconY}");

        // Only update if there is movement in the camera (prevents doubling on the first frame)
        if (cameraZOffset != 0 || cameraXOffset != 0)
        {
            icon.localPosition = new Vector3(iconX, iconY, initialIconPosition.z);
        }
    }
}
