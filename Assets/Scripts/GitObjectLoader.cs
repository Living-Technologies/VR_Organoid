using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Oculus.Interaction; // Make sure this namespace is included
using System.Collections;
using Newtonsoft.Json;  // Include Newtonsoft.Json for JSON parsing
using System;

public class ObjFromGitHub : MonoBehaviour
{
    public string githubRepoUrl = "https://api.github.com/repos/KrijnS/3d-objects/contents"; // URL for listing files in the GitHub repo
    public Material targetMaterial; // Assign the material in the Inspector
    private int objectCount = 0; // Keep track of the number of objects loaded
    public float radius = 1.0f; // Radius for positioning objects in a circle

    // Define the target BoxCollider size
    public Vector3 targetColliderSize = new Vector3(0.5f, 0.5f, 0.5f);

    void Start()
    {
        StartCoroutine(ListFilesInGitHubRepo());
    }

    private IEnumerator ListFilesInGitHubRepo()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(githubRepoUrl))
        {
            // Send the GET request to GitHub API
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to list files: {request.error}");
                yield break;
            }

            Debug.Log("Files listed:");

            // Parse the response JSON using Newtonsoft.Json (Json.NET)
            string jsonResponse = request.downloadHandler.text;

            // Deserialize the JSON response to a list of GitHubFile objects
            var files = JsonConvert.DeserializeObject<GitHubFile[]>(jsonResponse);

            // Loop through the files and load OBJ models
            foreach (var file in files)
            {
                if (file.type == "file" && file.name.EndsWith(".obj")) // Only process OBJ files
                {
                    Debug.Log($"Downloading and loading model: {file.name}");
                    string modelUrl = file.download_url; // GitHub API provides a direct URL to the raw file
                    StartCoroutine(DownloadAndLoadModel(modelUrl, file.name));
                }
            }
        }
    }

    private IEnumerator DownloadAndLoadModel(string modelUrl, string modelName)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(modelUrl))
        {
            // Start the request and wait for it to complete
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download model: {request.error}");
                yield break;
            }

            Debug.Log("Downloaded");

            // Create a stream from the downloaded data
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(request.downloadHandler.text));

            // Load the OBJ and assign it to a GameObject
            GameObject loadedObj = new OBJLoader().Load(textStream);
            if (loadedObj != null)
            {
                // Find the child with the mesh (usually the first child in the hierarchy)
                Transform meshTransform = loadedObj.transform.GetChild(0);
                if (meshTransform != null)
                {
                    // Rename the child GameObject that holds the mesh
                    meshTransform.name = modelName; // Use the passed model name

                    // Scale the model to fit inside the target BoxCollider
                    FitToCollider(meshTransform);

                    // Calculate the angle for the current object
                    float angle = objectCount * (2 * Mathf.PI / 10); // Adjust the divisor to change the number of objects in the circle

                    // Calculate the position based on the angle and radius
                    float xPosition = Mathf.Cos(angle) * radius;
                    float zPosition = Mathf.Sin(angle) * radius;

                    // Set the position of the mesh object
                    meshTransform.position = new Vector3(xPosition, 1.0f, zPosition);

                    // Increment the object count for the next object
                    objectCount++;

                    // Assign the material to all renderers in the mesh
                    Renderer[] renderers = meshTransform.GetComponentsInChildren<Renderer>();
                    foreach (var renderer in renderers)
                    {
                        renderer.material = targetMaterial; // Use material from Inspector
                    }

                    // Add a BoxCollider to the meshTransform (child object)
                    BoxCollider boxCollider = meshTransform.gameObject.AddComponent<BoxCollider>();
                    boxCollider.size = targetColliderSize; // Make sure the BoxCollider is sized correctly

                    // Add the OVRGrabInteractable to make the object interactable with Meta SDK
                    // OVRGrabInteractable grabInteractable = meshTransform.gameObject.AddComponent<OVRGrabInteractable>();

                    Debug.Log("Model loaded, material assigned, transformations applied, BoxCollider and OVRGrabInteractable added.");
                }
                else
                {
                    Debug.LogError("No mesh found in the loaded model.");
                }
            }

            else
            {
                Debug.LogError("Failed to load the OBJ model.");
            }
        }
    }

    // Method to scale the model to fit inside the target BoxCollider size
    private void FitToCollider(Transform meshTransform)
    {
        // Get the renderer to access the bounds
        Renderer renderer = meshTransform.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            // Get the bounds of the model
            Bounds bounds = renderer.bounds;

            // Calculate the scaling factor for each axis to fit inside the target collider size
            float scaleX = targetColliderSize.x / bounds.size.x;
            float scaleY = targetColliderSize.y / bounds.size.y;
            float scaleZ = targetColliderSize.z / bounds.size.z;

            // Calculate the uniform scaling factor (the smallest scaling factor)
            float uniformScale = Mathf.Min(scaleX, scaleY, scaleZ);

            // Apply the uniform scaling to the model
            meshTransform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);

            // Optionally, adjust the model's position to center it inside the collider
            meshTransform.position -= bounds.center - targetColliderSize / 2;
        }
    }
}

// Class to hold the structure of the GitHub API JSON response
[Serializable]
public class GitHubFile
{
    public string name;
    public string path;
    public string type;  // "file" or "dir"
    public string download_url;  // URL to the raw file (used to download the model)
}