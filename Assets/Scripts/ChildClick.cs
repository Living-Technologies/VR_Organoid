using UnityEngine;
using UnityEngine.UI; // For UI Text component
using System.Collections.Generic;

public class ChildClick : MonoBehaviour
{
    public ParentController parentController;
    public Text infoText; // Reference to a Text UI element to display information

    void OnMouseDown()
    {
        // Notify the parent controller that this child was clicked
        parentController.HandleChildClick(gameObject);

        // Get object name
        string objectName = gameObject.name;

        // Calculate the mesh volume (Check for MeshFilter first)
        float objectVolume = CalculateMeshVolume(gameObject);

        // Display name, and volume (either in UI or Console)
        DisplayObjectInfo(objectName, objectVolume.ToString());
    }

    void DisplayObjectInfo(string name, string volume)
    {
        if (infoText != null)
        {
            infoText.text = $"Nucleus: {name}\nVolume: {volume}";
        }
        else
        {
            // If you don't have a UI Text element, log it to the console
            Debug.Log($"Object Name: {name}\nVolume: {volume}");
        }
    }

    // Method to calculate the volume of the mesh
    public static float CalculateMeshVolume(GameObject obj)
    {
        // Check if MeshFilter is attached
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogWarning($"MeshFilter component missing on {obj.name}. Cannot calculate volume.");
            return 0f; // Return 0 if no MeshFilter is attached
        }

        Mesh mesh = meshFilter.mesh;  // Get the mesh from the MeshFilter component
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        float volume = 0f;

        // Loop through all the triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Get the indices of the triangle's vertices
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            // Get the actual vertex positions
            Vector3 v1 = vertices[index1];
            Vector3 v2 = vertices[index2];
            Vector3 v3 = vertices[index3];

            // Calculate the volume of the tetrahedron formed by the triangle and the origin
            volume += SignedVolumeOfTriangle(v1, v2, v3);
        }

        // Return the absolute value of the total volume
        return Mathf.Abs(volume);
    }

    // Method to calculate the signed volume of a single tetrahedron (using the triangle vertices and the origin)
    private static float SignedVolumeOfTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        return Vector3.Dot(v1, Vector3.Cross(v2, v3)) / 6f;
    }
}
