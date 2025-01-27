using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;

public class InteractiveInstancing : MonoBehaviour
{
    private Renderer[] childRenderers;
    public GameObject meshPlaceholder; // Object in front of the camera
    private MeshFilter placeholderMeshFilter;
    private MeshRenderer placeholderMeshRenderer;

    [SerializeField] private RayInteractor leftRayInteractor; // Assign your left-hand Ray Interactor
    [SerializeField] private RayInteractor rightRayInteractor;
    [SerializeField] private Material membraneMaterial; // Assign the membrane material in the Inspector
    [SerializeField] private OVRHand leftHand;

    private void Start()
    {
        placeholderMeshFilter = meshPlaceholder.GetComponent<MeshFilter>();
        placeholderMeshRenderer = meshPlaceholder.GetComponent<MeshRenderer>();

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

            // Create a collider surface and retrieve the collider
            Collider collider = CreateCollider(renderer.gameObject);
            if (collider == null)
            {
                Debug.LogError("Failed to create collider for object: " + renderer.gameObject.name);
                continue;
            }
            ColliderSurface surfaceCollider = CreateColliderSurface(renderer.gameObject);
            surfaceCollider.InjectCollider(collider);

            // Add Ray Interactable if missing
            if (!renderer.gameObject.TryGetComponent(out RayInteractable rayInteractable))
            {
                rayInteractable = renderer.gameObject.AddComponent<RayInteractable>();
            }

            // Set the collider surface for interaction
            rayInteractable.InjectSurface(surfaceCollider);
            rayInteractable.InjectOptionalSelectSurface(surfaceCollider);
        }
    }

   private Collider CreateCollider(GameObject target)
    {
        MeshCollider meshCollider = target.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = target.AddComponent<MeshCollider>();
            meshCollider.convex = true; // Make the collider convex
        }
        return meshCollider;
    }


    private ColliderSurface CreateColliderSurface(GameObject target)
    {
        // Check if the object already has a collider surface
        ColliderSurface colliderSurface = target.GetComponent<ColliderSurface>();
        if (colliderSurface == null)
        {
            // Add a ColliderSurface if none exists
            colliderSurface = target.AddComponent<ColliderSurface>();
        }

        return colliderSurface;
    }

    private void Update()
    {
        if(leftHand.IsTracked && leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        // if (Input.GetMouseButtonDown(0) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            Ray leftRay = leftRayInteractor.Ray;
            if (Physics.Raycast(leftRay, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject.name != "Sphere")
                {
                    // Ensure the clicked object has a MeshFilter
                    MeshFilter clickedMeshFilter = clickedObject.GetComponent<MeshFilter>();
                    MeshRenderer clickedMeshRenderer = clickedObject.GetComponent<MeshRenderer>();

                    if (clickedMeshFilter != null && clickedObject.name != "Sphere")
                    {
                        // Set the mesh of the placeholder to match the clicked object
                        placeholderMeshFilter.mesh = clickedMeshFilter.mesh;

                        // Adjust the position of the placeholder based on the mesh bounds
                        AlignPlaceholderWithMeshBounds(placeholderMeshFilter.mesh, Camera.main.transform);

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
                    }
                    else
                    {
                        Debug.LogError("Clicked object doesn't have a MeshFilter.");
                    }
                }
            }
        }
    }

    private void AlignPlaceholderWithMeshBounds(Mesh mesh, Transform cameraTransform, float targetSize = 3f, float distanceFromCamera = 3f)
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh is null. Cannot align placeholder.");
            return;
        }

        // Get the bounds of the mesh and the center position
        Bounds meshBounds = mesh.bounds;
        Vector3 meshCenter = meshBounds.center;
        float largestDimension = Mathf.Max(meshBounds.size.x, meshBounds.size.y, meshBounds.size.z);

        if (largestDimension <= 0)
        {
            Debug.LogError("Mesh bounds are invalid or largest dimension is zero. Cannot align placeholder.");
            return;
        }

        Debug.Log($"Mesh bounds center: {meshCenter}");
        Debug.Log($"Mesh bounds: {meshBounds}");
        Debug.Log($"Largest dimension: {largestDimension}");

        // Calculate the scale factor to make the mesh fit the target size
        float scaleFactor = targetSize / largestDimension;

        if (float.IsNaN(scaleFactor) || float.IsInfinity(scaleFactor))
        {
            Debug.LogError("Scale factor is invalid. Cannot align placeholder.");
            return;
        }

        // Scale the alignment vector to ensure proper placement after scaling
        Vector3 alignmentVector = -meshCenter * scaleFactor;

        // Calculate the position in front of the camera
        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * distanceFromCamera;

        // Apply rotation of the camera to align the object
        Quaternion targetRotation = cameraTransform.rotation;

        // Adjust position with alignment vector
        spawnPosition += targetRotation * alignmentVector;

        if (float.IsNaN(spawnPosition.x) || float.IsNaN(spawnPosition.y) || float.IsNaN(spawnPosition.z))
        {
            Debug.LogError("Calculated spawn position is invalid. Cannot align placeholder.");
            return;
        }

        // Apply the position, rotation, and scale to the placeholder
        meshPlaceholder.transform.localScale = Vector3.one * scaleFactor; // Scale first
        meshPlaceholder.transform.position = spawnPosition;              // Align position
        meshPlaceholder.transform.rotation = targetRotation;             // Apply rotation
    }
}

