using UnityEngine;

public class InteractiveInstancing : MonoBehaviour
{
    private Renderer[] childRenderers;
    public GameObject meshPlaceholder; // Object in front of the camera
    private MeshFilter placeholderMeshFilter;

    [SerializeField] private Material membraneMaterial; // Assign the membrane material in the Inspector

    private void Start()
    {
        placeholderMeshFilter = meshPlaceholder.GetComponent<MeshFilter>();

        // Ensure the placeholder is active and visible
        meshPlaceholder.SetActive(true);

        childRenderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < childRenderers.Length; i++)
        {
            Renderer renderer = childRenderers[i];

            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Disable shadow casting
            renderer.receiveShadows = false; // Disable receiving shadows
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera; // Disable object motion vectors
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off; // Disable light probes
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off; // Disable reflection probes

            // Clear all materials and assign only the membrane material
            if (membraneMaterial != null)
            {
                renderer.materials = new Material[] { membraneMaterial };
            }

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

    private void AlignPlaceholderWithMeshBounds(Mesh mesh, float targetSize = 3f)
    {
        // Get the bounds of the mesh and the center position
        Bounds meshBounds = mesh.bounds;
        Vector3 meshCenter = meshBounds.center;
        float largestDimension = Mathf.Max(meshBounds.size.x, meshBounds.size.y, meshBounds.size.z);

        Debug.Log($"Mesh bounds center: {meshCenter}");
        Debug.Log($"Mesh bounds: {meshBounds}");
        Debug.Log($"Largest dimension: {largestDimension}");

        // Calculate the scale factor to make the mesh fit the target size
        float scaleFactor = targetSize / largestDimension;

        // Scale the alignment vector to ensure proper placement after scaling
        Vector3 alignmentVector = -meshCenter * scaleFactor;

        // Apply the desired rotation to adjust the position of the placeholder
        Quaternion targetRotation = Quaternion.Euler(0, -90, 90);
        Vector3 adjustedPosition = targetRotation * alignmentVector;

        // Adjust position after rotation (optional offset)
        adjustedPosition.z += 3;

        // Apply the position, rotation, and scale to the placeholder
        meshPlaceholder.transform.localScale = Vector3.one * scaleFactor; // Scale first
        meshPlaceholder.transform.position = adjustedPosition;            // Align position
        meshPlaceholder.transform.rotation = targetRotation;              // Apply rotation
    }
}
