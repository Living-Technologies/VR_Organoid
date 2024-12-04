using UnityEngine;

public class MeshVolumeCalculator : MonoBehaviour
{
    // Method to calculate the volume of the mesh
    public static float CalculateMeshVolume(Mesh mesh)
    {
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

    // Example usage
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;  // Get the mesh from the MeshFilter component
        float volume = CalculateMeshVolume(mesh);
        Debug.Log("Mesh Volume: " + volume);
    }
}
