using UnityEngine;

public class SphereRotationController : MonoBehaviour
{
    [SerializeField] private Transform targetObject; // The object to control
    private Vector3 lastHandPosition;

    void Update()
    {
        if (Input.GetMouseButton(0)) // Replace with VR input check
        {
            // Get the current hand position (or controller in VR)
            Vector3 currentHandPosition = Input.mousePosition; // Replace with VR hand/controller position

            if (lastHandPosition != Vector3.zero)
            {
                // Calculate rotation delta based on hand movement
                Vector3 delta = currentHandPosition - lastHandPosition;
                float rotationX = delta.y; // Vertical movement rotates around X-axis
                float rotationY = -delta.x; // Horizontal movement rotates around Y-axis

                // Apply rotation to the sphere
                transform.Rotate(rotationX, rotationY, 0, Space.World);

                // Map the sphere's rotation to the target object
                if (targetObject != null)
                {
                    targetObject.rotation = transform.rotation;
                }
            }

            lastHandPosition = currentHandPosition;
        }
        else
        {
            lastHandPosition = Vector3.zero; // Reset when not interacting
        }
    }
}
