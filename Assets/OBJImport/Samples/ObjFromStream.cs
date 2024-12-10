using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ObjFromStream : MonoBehaviour
{
    public string modelUrl = "https://raw.githubusercontent.com/KrijnS/3d-objects/30f17f65273d29311ff35d1a8aaf53befbc591c4/predictions_trained_0.obj";
    public Material targetMaterial; // Assign the material in the Inspector

    void Start()
    {
        StartCoroutine(DownloadAndLoadModel());
    }

    private IEnumerator DownloadAndLoadModel()
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
                    meshTransform.name = "LoadedMembraneModel";

                    // Scale the mesh object to (0.005, 0.005, 0.005)
                    meshTransform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

                    // Translate the mesh object to (-0.5, 1.0, -0.5)
                    meshTransform.position = new Vector3(0.5f, 1.0f, -0.5f);

                    // Assign the material to all renderers in the mesh
                    Renderer[] renderers = meshTransform.GetComponentsInChildren<Renderer>();
                    foreach (var renderer in renderers)
                    {
                        renderer.material = targetMaterial; // Use material from Inspector
                    }

                    Debug.Log("Model loaded, material assigned, and transformations applied.");
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
}
