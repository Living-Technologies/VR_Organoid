using UnityEngine;

public class InteractiveInstancing : MonoBehaviour
{
    private Renderer[] childRenderers;
    public GameObject meshPlaceholder; // Object in front of the camera
    private MeshFilter placeholderMeshFilter;

    private void Start()
    {
        placeholderMeshFilter = meshPlaceholder.GetComponent<MeshFilter>();

        // Ensure the placeholder is active and visible
        meshPlaceholder.SetActive(true);

        childRenderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < childRenderers.Length; i++)
        {
            Renderer renderer = childRenderers[i];

            // Ensure colliders are present for interaction
            if (!renderer.gameObject.TryGetComponent(out Collider _))
            {
                renderer.gameObject.AddComponent<BoxCollider>();
            }
        }

        Debug.Log($"Optimized instancing with interaction for {childRenderers.Length} objects.");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                Debug.Log($"Clicked on object: {clickedObject.name}");
                if(clickedObject.name != "Sphere")
                {
                    // Ensure the clicked object has a MeshFilter
                    MeshFilter clickedMeshFilter = clickedObject.GetComponent<MeshFilter>();

                    if (clickedMeshFilter != null && clickedObject.name != "Sphere")
                    {
                        // Set the mesh of the placeholder to match the clicked object
                        placeholderMeshFilter.mesh = clickedMeshFilter.mesh;

                        // Adjust the position of the placeholder based on the mesh bounds
                        AlignPlaceholderWithMeshBounds(placeholderMeshFilter.mesh);

                        // Now ensure that the collider on the clicked object is convex
                        Collider clickedCollider = clickedObject.GetComponent<Collider>();
                        if (clickedCollider != null && clickedCollider is MeshCollider meshCollider)
                        {
                            meshCollider.convex = true; // Make the collider convex
                        }
                        else
                        {
                            Debug.LogWarning("Clicked object doesn't have a MeshCollider or other collider.");
                        }

                        Debug.Log($"Mesh of placeholder updated to {clickedObject.name}");
                    }
                    else
                    {
                        Debug.LogError("Clicked object doesn't have a MeshFilter.");
                    }
                }
            }
        }
    }

    private void AlignPlaceholderWithMeshBounds(Mesh mesh)
    {
        // Get the bounds of the mesh and the center position
        Bounds meshBounds = mesh.bounds;
        Vector3 meshCenter = meshBounds.center;

        Debug.Log($"Mesh bounds center: {meshCenter}");
        Debug.Log($"Mesh bounds: {meshBounds}");

        // Calculate the alignment vector by considering the inverse rotation
        Vector3 alignmentVector = -meshCenter;

        // Apply the rotation that you want (-90 on X, 0 on Y, -180 on Z)
        Quaternion targetRotation = Quaternion.Euler(-90, 0, -180);

        // Apply the desired rotation to adjust the position of the placeholder
        Vector3 adjustedPosition = targetRotation * alignmentVector;
        adjustedPosition.z += 2;

        // Apply the position and rotation to the placeholder
        meshPlaceholder.transform.position = adjustedPosition;
        meshPlaceholder.transform.rotation = targetRotation;
    }


}
