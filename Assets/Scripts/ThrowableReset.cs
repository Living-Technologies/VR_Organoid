using UnityEngine;

public class ThrowableReset : MonoBehaviour
{
    private Vector3 originalPosition; // Store the initial position
    private Rigidbody rb;             // Reference to the Rigidbody component
    public float resetThreshold = -50f; // Y-position threshold for resetting

    void Start()
    {
        // Save the original position and get the Rigidbody component
        originalPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the object has fallen below the threshold
        if (transform.position.y < resetThreshold)
        {
            ResetObject();
        }
    }

    private void ResetObject()
    {
        // Reset position
        transform.position = originalPosition;

        // Reset velocity if using Rigidbody
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Optionally reset rotation
        transform.rotation = Quaternion.identity;
    }
}
